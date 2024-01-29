using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using OfficeOpenXml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MultiSMS.BusinessLogic.Services
{
    public class ImportExportEmployeesService : IImportExportEmployeesService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;
        private readonly IEntitiesValidationService _entitiesValidationService;
        private readonly IPathProvider _pathProvider;


        public ImportExportEmployeesService(IEmployeeRepository employeeRepository, IEmployeeGroupRepository employeeGroupRepository, IEntitiesValidationService entitiesValidationService, IPathProvider pathProvider)
        {
            _employeeRepository = employeeRepository;
            _employeeGroupRepository = employeeGroupRepository;
            _entitiesValidationService = entitiesValidationService;
            _pathProvider = pathProvider;

        }

        public async Task<object> ImportContactsCsvAsync(IFormFile file)
        {
            var phoneNumbersInDb = _employeeRepository.GetAllEntries().Select(e => e.PhoneNumber);
            var records = new List<Employee>();
            var invalidRecords = new List<Employee>();
            var repeatedEntries = new List<Employee>();

            string[] requiredHeaders = { "osoba", "tel", "instytucja", "grupa"};

            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower()
            };

            using (var reader = new StreamReader(memoryStream))
            using (var csvReader = new CsvReader(reader, csvConfig))
            {

                csvReader.Read();
                csvReader.ReadHeader();

                var fileHeaders = csvReader.HeaderRecord;
                if (requiredHeaders.Except(fileHeaders).Any())
                {
                    return new { Status = "Failure", Message = "Struktura pliku .csv nie jest prawidłowa." };
                }

                while (csvReader.Read())
                {
                    var record = new Employee
                    {
                        Name = csvReader.GetField<string>("osoba").Split(' ')[0],
                        Surname = csvReader.GetField<string>("osoba").Split(' ')[1],
                        PhoneNumber = Regex.Replace(csvReader.GetField<string>("tel"), @"(\S{3})", "$1 ").Trim(),
                        Department = csvReader.GetField<string>("instytucja"),
                        IsActive = true
                    };

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
            return new {Status = "Success", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
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
                sheet.Cells["D1"].Value = "Departament";
                sheet.Cells["E1"].Value = "Kod Pocztowy";
                sheet.Cells["F1"].Value = "Miasto";
                sheet.Cells["G1"].Value = "Adres miejsca pracy";
                sheet.Cells["H1"].Value = "Aktywność";
                sheet.Cells["I1"].Value = "Grupa";
                sheet.Cells["J1"].Value = "Nazwy grup";

                foreach(var employee in allEmployees)
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
