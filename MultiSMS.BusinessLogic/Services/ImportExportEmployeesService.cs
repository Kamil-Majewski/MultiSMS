using CsvHelper;
using Microsoft.AspNetCore.Http;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Repositories.Interfaces;
using System.Globalization;
using System.Text.Json.Nodes;

namespace MultiSMS.BusinessLogic.Services
{
    public class ImportExportEmployeesService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeGroupRepository _employeeGroupRepository;

        public ImportExportEmployeesService(IEmployeeRepository employeeRepository, IEmployeeGroupRepository employeeGroupRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeGroupRepository = employeeGroupRepository;
        }

        public async Task<object> ImportContactsCsv(IFormFile file)
        {
            var phoneNumbersInDb = _employeeRepository.GetAllEntries().Select(e => e.PhoneNumber);
            var records = new List<Employee>();
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
                        Department = csvReader.GetField<string>("instytucja")
                    };

                    if(phoneNumbersInDb.Any(p => p == record.PhoneNumber))
                    {
                        repeatedEntries.Add(record);
                    }
                    else
                    {
                        records.Add(record);
                    }
                }
            }
            var addedEmployees = await _employeeRepository.AddRangeOfEntitiesToDatabaseAsync(records);
            return new { AddedEmployees = addedEmployees, RepeatedEmployees = repeatedEntries };
        }
    }
}
