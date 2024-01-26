using CsvHelper;
using Microsoft.AspNetCore.Http;
using MultiSMS.BusinessLogic.Services.Interfaces;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using System.Globalization;

namespace MultiSMS.BusinessLogic.Services
{
    public class ImportExportEmployeesService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;
        private readonly IEntitiesValidationService _entitiesValidationService;

        public ImportExportEmployeesService(IEmployeeRepository employeeRepository, IEmployeeGroupRepository employeeGroupRepository, IEntitiesValidationService entitiesValidationService)
        {
            _employeeRepository = employeeRepository;
            _employeeGroupRepository = employeeGroupRepository;
            _entitiesValidationService = entitiesValidationService;
        }

        public async Task<object> ImportContactsCsv(IFormFile file)
        {
            var phoneNumbersInDb = _employeeRepository.GetAllEntries().Select(e => e.PhoneNumber);
            var records = new List<Employee>();
            var invalidRecords = new List<Employee>();
            var repeatedEntries = new List<Employee>();
            
            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using (var reader = new StreamReader(memoryStream))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                
                csvReader.Read();
                csvReader.ReadHeader();
                while (csvReader.Read())
                {
                    var record = new Employee
                    {
                        Name = csvReader.GetField<string>("osoba").Split(' ')[0],
                        Surname = csvReader.GetField<string>("osoba").Split(' ')[1],
                        PhoneNumber = csvReader.GetField<string>("tel"),
                        Department = csvReader.GetField<string>("instytucja"),
                        IsActive = true
                    };

                    if(phoneNumbersInDb.Any(p => p == record.PhoneNumber))
                    {
                        repeatedEntries.Add(record);
                    }
                    else if(_entitiesValidationService.CheckEmployeeValidity(record))
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
            return new { AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries, InvalidEmployees = invalidRecords };
        }
    }
}
