﻿using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IGroupRepository _groupRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;
        private readonly IPathProvider _pathProvider;


        public ImportExportEmployeesService(IEmployeeRepository employeeRepository, IGroupRepository groupRepository, IEmployeeGroupRepository employeeGroupRepository, IPathProvider pathProvider)
        {
            _employeeRepository = employeeRepository;
            _groupRepository = groupRepository;
            _employeeGroupRepository = employeeGroupRepository;
            _pathProvider = pathProvider;

        }

        private bool CheckEmployeeValidity(Employee employee)
        {
            var phoneNumberPattern = "^(\\+[0-9]{2} )?\\d{3} \\d{3} \\d{3}$";
            Regex regex = new Regex(phoneNumberPattern);

            if (employee.Name.IsNullOrEmpty() || employee.Surname.IsNullOrEmpty() || employee.PhoneNumber.IsNullOrEmpty())
            {
                return false;
            }

            Match match = regex.Match(employee.PhoneNumber);

            if (!match.Success)
            {
                return false;
            }

            return true;
        }

        public async Task<ImportResult> ImportContactsAsync(IFormFile file)
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
                    return new ImportResult { ImportStatus = "Failure", ImportMessage = "Struktura pliku .csv nie jest prawidłowa" };
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

        public async Task<ImportResult> ImportContactsCsvByTypeAsync(IFormFile file, string type)
        {
            var phoneNumbersInDb = _employeeRepository.GetAllEntries().Select(e => e.PhoneNumber);
            List<Employee> allRecords = new List<Employee>();
            List<Employee> repeatedEntries = new List<Employee>();
            List<Employee> validRecords = new List<Employee>();
            List<Employee> invalidRecords = new List<Employee>();
            List<string> groupIds = new List<string>();

            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                Delimiter = type == "new" ? ";" : ","
            };

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
                        record.IsActive = csvReader.GetField<string>("aktywność") == "Aktywny";
                        record.Email = csvReader.GetField<string>("email");
                        record.PostalNumber = csvReader.GetField<string>("kod pocztowy");
                        record.City = csvReader.GetField<string>("miasto");
                        record.HQAddress = csvReader.GetField<string>("adres miejsca pracy");
                    }

                    if (phoneNumbersInDb.Any(p => p == record.PhoneNumber))
                    {
                        repeatedEntries.Add(record);
                    }
                    else if (CheckEmployeeValidity(record))
                    {
                        validRecords.Add(record);
                    }
                    else
                    {
                        invalidRecords.Add(record);
                    }

                    allRecords.Add(record);
                }
            }

            var addedEmployees = await _employeeRepository.AddRangeOfEntitiesToDatabaseAsync(validRecords);
            var addedEmployeesList = addedEmployees.ToList();

            if (addedEmployees.Count() == 0)
            {
                return new ImportResult { ImportStatus = "OK", ImportMessage = "Brak nowych kontaktów w pliku csv", RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
            }

            var groupIdsInDb = _groupRepository.GetDictionaryWithGroupIdsAndNames();
            var nonExistentGroupIds = new List<List<string>>();
            var anyFailedAssigns = false;

            for (int i = 0; i < addedEmployeesList.Count(); i++)
            {
                var employeeId = addedEmployeesList[i].EmployeeId;
                var indexOfAddedEmployeeInRecords = allRecords.IndexOf(addedEmployeesList[i]);

                foreach (var groupId in groupIds[indexOfAddedEmployeeInRecords].Split(","))
                {
                    if (int.TryParse(groupId, out var groupIdInt) && groupIdsInDb.TryGetValue(groupIdInt, out var groupName))
                    {
                        await _employeeGroupRepository.AddGroupMemberAsync(groupIdInt, employeeId);
                        addedEmployeesList[i].EmployeeGroupNames.Add(groupName);

                        if (nonExistentGroupIds.Count <= i)
                        {
                            nonExistentGroupIds.Add(new List<string>());
                        }
                    }
                    else
                    {
                        anyFailedAssigns = true;

                        if (nonExistentGroupIds.Count <= i)
                        {
                            nonExistentGroupIds.Add(new List<string> { groupId });
                        }
                        else
                        {
                            nonExistentGroupIds[i].Add(groupId);
                        }
                    }
                }
            }

            if (anyFailedAssigns)
            {
                return new ImportResult { ImportStatus = "Partial Success", ImportMessage = "Dodano nowe kontakty, ale nie wszystkie grupy były prawidłowe.", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords, NonExistantGroupIds = nonExistentGroupIds };
            }
            else
            {
                return new ImportResult { ImportStatus = "Success", ImportMessage = "Poprawnie dodano nowe kontakty i przypisano je do grup.", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
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
                sheet.Cells["A:J"].Style.Numberformat.Format = "@";

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
