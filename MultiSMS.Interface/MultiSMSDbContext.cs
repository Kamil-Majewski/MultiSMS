using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using System.Reflection.Emit;

namespace MultiSMS.Interface
{
    public class MultiSMSDbContext : IdentityDbContext<Administrator, IdentityRole<int>, int>
    {
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeesGroup> EmployeeGroups { get; set; }
        public virtual DbSet<EmployeesRole> EmployeeRoles { get; set; }
        public virtual DbSet<SMSMessage> SMSMessageLogs { get; set; }
        public virtual DbSet<SMSMessageTemplate> SMSMessageTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
