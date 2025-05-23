﻿using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MultiSMS.BusinessLogic.Helpers;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using OfficeOpenXml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MultiSMS.BusinessLogic.Services
{
    public class ImportExportEmployeesService : IImportExportEmployeesService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IGroupService _groupService;
        private readonly IEmployeeGroupService _employeeGroupService;
        private readonly IPathProvider _pathProvider;
        private readonly IProgressRelay _progressRelay;

        public ImportExportEmployeesService(IEmployeeService employeeService,
                                            IGroupService groupService,
                                            IEmployeeGroupService employeeGroupService,
                                            IPathProvider pathProvider,
                                            IProgressRelay progressRelay)
        {
            _employeeService = employeeService;
            _groupService = groupService;
            _employeeGroupService = employeeGroupService;
            _pathProvider = pathProvider;
            _progressRelay = progressRelay;
        }

        private bool CheckEmployeeValidity(Employee employee)
        {
            ValidationHelper.ValidateObject(employee, nameof(employee));

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

        private string ParsePhoneNumber(string phoneNumber)
        {
            ValidationHelper.ValidateString(phoneNumber, nameof(phoneNumber));

            if (phoneNumber.Count() == 9)
            {
                return $"+48{phoneNumber}";
            }
            else if (phoneNumber.Count() == 11)
            {
                return $"+{phoneNumber}";
            }
            else
            {
                return phoneNumber;
            }
        }

        private (string, string) GetNameAndSurname(string[] personField)
        {
            ValidationHelper.ValidateCollection(personField, nameof(personField));

            if (personField.Length == 1)
            {
                return (personField[0], "Nie podano");
            }
            else if (personField.Length == 2)
            {
                return (personField[0], personField[1]);
            }
            else if (personField.Length == 3)
            {
                return (personField[1], personField[2]);
            }
            else
            {
                return ("Nieprawidłowa wartosć", "");
            }
        }

        public async Task<ImportResult> ImportContactsAsync(IFormFile file)
        {
            ValidationHelper.ValidateObject(file, nameof(file));

            string[] requiredHeaders = { "osoba", "tel", "instytucja", "grupa" };
            int rows = 0;
            string[] fileHeaders;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };

            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using (var reader = new StreamReader(memoryStream))
            using (var csvReader = new CsvReader(reader, config))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                fileHeaders = csvReader.HeaderRecord!.Select(f => f.ToLower()).ToArray();

                while (csvReader.Read())
                {
                    rows++;
                }
            }

            if (requiredHeaders.Except(fileHeaders).Any())
            {
                return await ImportContactsCsvByTypeAsync(file, rows, "new");
            }
            else
            {
                return await ImportContactsCsvByTypeAsync(file, rows, "old");
            }
        }

        public async Task<ImportResult> ImportContactsCsvByTypeAsync(IFormFile file, int totalRows, string type)
        {
            ValidationHelper.ValidateObject(file, nameof(file));
            ValidationHelper.ValidateId(totalRows, nameof(totalRows));
            ValidationHelper.ValidateString(type, nameof(type));

            var phoneNumbersInDb = _employeeService.GetAllEntriesQueryable().Select(e => e.PhoneNumber);
            List<Employee> allRecords = new List<Employee>();
            List<Employee> repeatedEntries = new List<Employee>();
            List<Employee> validRecords = new List<Employee>();
            List<Employee> invalidRecords = new List<Employee>();
            List<string> groupIds = new List<string>();
            var processedRows = 0;

            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLowerInvariant(),
                Delimiter = ";"
            };

            using (var reader = new StreamReader(memoryStream))
            using (var csvReader = new CsvReader(reader, csvConfig))
            {
                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {
                    var groupId = csvReader.GetField<string>("grupa")?.Trim();
                    var rawPhoneNumber = csvReader.GetField<string>("tel")?.Trim();
                    var personRaw = csvReader.GetField<string>("osoba")?.Trim();

                    groupIds.Add(groupId ?? "");

                    var personParts = personRaw?.Split(" ") ?? Array.Empty<string>();

                    if (personParts.Length < 2)
                    {
                        invalidRecords.Add(new Employee
                        {
                            Name = personRaw ?? "",
                            Surname = "",
                            PhoneNumber = rawPhoneNumber ?? "",
                            Department = csvReader.GetField<string>("instytucja")?.Trim(),
                            IsActive = false
                        });
                        continue;
                    }

                    var (name, surname) = GetNameAndSurname(personParts);

                    if (string.IsNullOrWhiteSpace(rawPhoneNumber))
                    {
                        invalidRecords.Add(new Employee
                        {
                            Name = name,
                            Surname = surname,
                            PhoneNumber = rawPhoneNumber ?? "",
                            Department = csvReader.GetField<string>("instytucja")?.Trim(),
                            IsActive = false
                        });
                        continue;
                    }

                    var sanitizedPhoneNumber = Regex.Replace(rawPhoneNumber, @"\s+", "");
                    var phoneNumber = ParsePhoneNumber(sanitizedPhoneNumber);

                    var record = new Employee
                    {
                        Name = name,
                        Surname = surname,
                        PhoneNumber = Regex.Replace(phoneNumber, @"(\S{3})", "$1 ").Trim(),
                        Department = csvReader.GetField<string>("instytucja")?.Trim(),
                        IsActive = true
                    };

                    if (type == "new")
                    {
                        record.IsActive = csvReader.GetField<string>("aktywność")?.Trim() == "Aktywny";
                        record.Email = csvReader.GetField<string>("email")?.Trim();
                        record.PostalNumber = csvReader.GetField<string>("kod pocztowy")?.Trim();
                        record.City = csvReader.GetField<string>("miasto")?.Trim();
                        record.HQAddress = csvReader.GetField<string>("adres miejsca pracy")?.Trim();
                    }

                    if (phoneNumbersInDb.Any(p => p == record.PhoneNumber) || validRecords.Any(r => r.PhoneNumber == record.PhoneNumber))
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
                    processedRows++;

                    int progress = (int)((double)processedRows / totalRows * 70);
                    await _progressRelay.RelayProgressAsync("ImportContactsProgress", progress.ToString());
                }
            }

            var addedEmployees = await _employeeService.AddRangeOfEntitiesToDatabaseAsync(validRecords);
            var addedEmployeesList = addedEmployees.ToList();

            if (addedEmployees.Count() == 0)
            {
                await _progressRelay.RelayProgressAsync("ImportContactsProgress", "100");
                return new ImportResult { ImportStatus = "OK", ImportMessage = "Brak nowych kontaktów w pliku csv", RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
            }

            var groupIdsInDb = await _groupService.GetDictionaryWithGroupIdsAndNamesAsync();
            var nonExistentGroupIds = new List<List<string>>();
            var anyFailedAssigns = false;

            processedRows = 0;
            var addedEmployeesCount = addedEmployeesList.Count();

            for (int i = 0; i < addedEmployeesCount; i++)
            {
                var employeeId = addedEmployeesList[i].EmployeeId;
                var indexOfAddedEmployeeInRecords = allRecords.IndexOf(addedEmployeesList[i]);

                foreach (var groupId in groupIds[indexOfAddedEmployeeInRecords].Split(","))
                {
                    if (int.TryParse(groupId, out var groupIdInt) && groupIdsInDb.TryGetValue(groupIdInt, out var groupName))
                    {
                        await _employeeGroupService.AddGroupMemberAsync(groupIdInt, employeeId);
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
                processedRows++;
                int progress = 70 + (processedRows / addedEmployeesCount) * 25;

                await _progressRelay.RelayProgressAsync("ImportContactsProgress", progress.ToString());
            }

            if (anyFailedAssigns)
            {
                await _progressRelay.RelayProgressAsync("ImportContactsProgress", "100");
                return new ImportResult { ImportStatus = "Partial Success", ImportMessage = "Dodano nowe kontakty, ale nie wszystkie grupy były prawidłowe.", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords, NonExistantGroupIds = nonExistentGroupIds };
            }
            else
            {
                await _progressRelay.RelayProgressAsync("ImportContactsProgress", "100");
                return new ImportResult { ImportStatus = "Success", ImportMessage = "Poprawnie dodano nowe kontakty i przypisano je do grup.", AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords, NonExistantGroupIds = nonExistentGroupIds };
            }
        }

        public async Task<string> ExportContactsExcelAsync()
        {
            string wwwrootPath = _pathProvider.WwwRootPath;
            string filePath = Path.Combine(wwwrootPath, "Kontakty.xlsx");

            var allEmployees = await _employeeService.GetAllEntriesAsync();
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
                    var groupNamesList = await _employeeGroupService.GetAllGroupNamesForEmployeeListAsync(employee.EmployeeId);
                    var groupIdsList = await _employeeGroupService.GetAllGroupNamesForEmployeeListAsync(employee.EmployeeId);

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
