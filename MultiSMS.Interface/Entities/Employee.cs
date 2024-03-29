﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiSMS.Interface.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = default!;
        public string Surname { get; set; } = default!;
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = default!;
        public string? Department { get; set; }
        public string? PostalNumber { get; set; }
        public string? City { get; set; }
        public string? HQAddress { get; set; }
        public bool IsActive { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public ICollection<EmployeeGroup>? EmployeeGroups { get; set; }

        [NotMapped]
        public ICollection<int> EmployeeGroupIds { get; set; } = new List<int>();

        [NotMapped]
        public ICollection<string> EmployeeGroupNames { get; set; } = new List<string>();
    }
}
