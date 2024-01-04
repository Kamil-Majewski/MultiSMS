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
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<SMSMessageTemplate> SMSMessageTemplates { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<SMSMessage > SMSMessages { get; set; }
        public virtual DbSet<EmployeeGroup> EmployeeGroups { get; set; }

        public MultiSMSDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
