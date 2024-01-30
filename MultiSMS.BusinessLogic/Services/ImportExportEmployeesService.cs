using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using OfficeOpenXml;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace MultiSMS.BusinessLogic.Services
{
    public class ImportExportEmployeesService : IImportExportEmployeesService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;
        private readonly IEntitiesValidationService _entitiesValidationService;
        private readonly IPathProvider _pathProvider;


        public ImportExportEmployeesService(IEmployeeRepository employeeRepository, IGroupRepository groupRepository, IEmployeeGroupRepository employeeGroupRepository, IEntitiesValidationService entitiesValidationService, IPathProvider pathProvider)
        {
            _employeeRepository = employeeRepository;
            _groupRepository = groupRepository;
            _employeeGroupRepository = employeeGroupRepository;
            _entitiesValidationService = entitiesValidationService;
            _pathProvider = pathProvider;

        }

        public async Task<object> ImportContactsAsync(IFormFile file)
        {
            string[] requiredHeaders = { "osoba", "tel", "instytucja", "grupa" };

            string[] fileHeaders;

            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using (var reader = new StreamReader(memoryStream))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                fileHeaders = csvReader.HeaderRecord.Select(f => f.ToLower()).ToArray();
            }

            if (requiredHeaders.Except(fileHeaders).Any())
            {
                fileHeaders = fileHeaders[0].Split(";").ToArray();

                if (requiredHeaders.Except(fileHeaders).Any())
                {
                    return new { Status = "Failure", Message = "Struktura pliku .csv nie jest prawidłowa." };
                }
                else
                {
                    return await ImportContactsCsvByTypeAsync(file, "new");
                }
            }
            else
            {
                return await ImportContactsCsvByTypeAsync(file, "old");
            }
        }

        public async Task<object> ImportContactsCsvByTypeAsync(IFormFile file, string type)
        {
            var phoneNumbersInDb = _employeeRepository.GetAllEntries().Select(e => e.PhoneNumber);
            List<Employee> repeatedEntries = new List<Employee>();
            List<Employee> records = new List<Employee>();
            List<Employee> invalidRecords = new List<Employee>();
            List<string> groupIds = new List<string>();

            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
            };

            if (type == "new")
            {
                csvConfig.Delimiter = ";";
            }

            using (var reader = new StreamReader(memoryStream))
            using (var csvReader = new CsvReader(reader, csvConfig))
            {

                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    groupIds.Add(csvReader.GetField<string>("grupa"));
                    var sanitizedPhoneNumber = Regex.Replace(csvReader.GetField<string>("tel"), @"\s+", "");

                    var record = new Employee
                    {
                        Name = csvReader.GetField<string>("osoba").Split(' ')[0],
                        Surname = csvReader.GetField<string>("osoba").Split(' ')[1],
                        PhoneNumber = Regex.Replace(sanitizedPhoneNumber, @"(\S{3})", "$1 ").Trim(),
                        Department = csvReader.GetField<string>("instytucja"),
                        IsActive = true
                    };

                    if (type == "new")
                    {
                        record.IsActive = csvReader.GetField<string>("aktywność") == "Aktywny" ? true : false;
                        record.Email = csvReader.GetField<string>("email");
                        record.PostalNumber = csvReader.GetField<string>("kod pocztowy");
                        record.City = csvReader.GetField<string>("miasto");
                        record.HQAddress = csvReader.GetField<string>("adres miejsca pracy");
                    }

                    if (phoneNumbersInDb.Any(p => p == record.PhoneNumber))
                    {
                        repeatedEntries.Add(record);
                    }
                    else if (_entitiesValidationService.CheckEmployeeValidity(record))
                    {
                        records.Add(record);
                    }
                    else
                    {
                        invalidRecords.Add(record);
                    }
                }
            }

            var addedEmployees = await _employeeRepository.AddRangeOfEntitiesToDatabaseAsync(records);
            var addedEmployeesList = addedEmployees.ToList();

            if (addedEmployees.Count() == 0)
            {
                return new { Status = "OK", Message = "Import zakończony. Nie dodano żadnych nowych kontaktów.", RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
            }

            var groupIdsInDb = _groupRepository.GetAllGroupIds();
            var nonExistantGroupIds = new List<int>();
            var anyFailedAssigns = false;

            for (var i = 0; i < addedEmployeesList.Count(); i++)
            {
                var employeeId = addedEmployeesList[i].EmployeeId;
                foreach (var groupId in groupIds[i].Split(","))
                {
                    if (int.TryParse(groupId, out var id))
                    {
                        if (groupIdsInDb.Contains(id))
                        {
                            await _employeeGroupRepository.AddGroupMemberAsync(id, employeeId);
                        }
                        else
                        {
                            anyFailedAssigns = true;
                            nonExistantGroupIds.Add(id);
                        }
                    }
                }
            }

            if (anyFailedAssigns)
            {
                return new { Status = "Partial Success", Message = "Import zakończony. Dodano nowe kontakty, ale nie wszystkie przypisania do grup zakończyły się powodzeniem.", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords, NonExistantGroupIds = nonExistantGroupIds };
            }
            else
            {
                return new { Status = "Success", Message = "Import zakończony. Poprawnie dodano nowe kontakty i przypisano je do grup.", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
            }
        }

        public string ExportContactsExcel()
        {
            string wwwrootPath = _pathProvider.WwwRootPath;
            string filePath = Path.Combine(wwwrootPath, "Kontakty.xlsx");

            var allEmployees = _employeeRepository.GetAllEntries().ToList();
            var rowNumber = 2;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {

                var sheet = package.Workbook.Worksheets.Add("Kontaky");
                sheet.Cells["A1"].Value = "Osoba";
                sheet.Cells["B1"].Value = "Tel";
                sheet.Cells["C1"].Value = "Email";
                sheet.Cells["D1"].Value = "Instytucja";
                sheet.Cells["E1"].Value = "Kod Pocztowy";
                sheet.Cells["F1"].Value = "Miasto";
                sheet.Cells["G1"].Value = "Adres miejsca pracy";
                sheet.Cells["H1"].Value = "Aktywność";
                sheet.Cells["I1"].Value = "Grupa";
                sheet.Cells["J1"].Value = "Nazwy grup";

                foreach (var employee in allEmployees)
                {
                    var groupNamesList = _employeeGroupRepository.GetAllGroupNamesForEmployeeQueryable(employee.EmployeeId).ToList();
                    var groupIdsList = _employeeGroupRepository.GetAllGroupIdsForEmployeeQueryable(employee.EmployeeId).ToList();

                    sheet.Cells[$"A{rowNumber}"].Value = $"{employee.Name} {employee.Surname}";
                    sheet.Cells[$"B{rowNumber}"].Value = employee.PhoneNumber;
                    sheet.Cells[$"C{rowNumber}"].Value = employee.Email;
                    sheet.Cells[$"D{rowNumber}"].Value = employee.Department;
                    sheet.Cells[$"E{rowNumber}"].Value = employee.PostalNumber;
                    sheet.Cells[$"F{rowNumber}"].Value = employee.City;
                    sheet.Cells[$"G{rowNumber}"].Value = employee.HQAddress;
                    sheet.Cells[$"H{rowNumber}"].Value = !employee.IsActive ? "Nieaktywny" : "Aktywny";
                    sheet.Cells[$"I{rowNumber}"].Value = string.Join(", ", groupIdsList);
                    sheet.Cells[$"J{rowNumber}"].Value = string.Join(", ", groupNamesList);

                    rowNumber++;
                }

                var fileInfo = new FileInfo(filePath);
                package.SaveAs(fileInfo);

                return filePath;
            }
        }
    }
}
