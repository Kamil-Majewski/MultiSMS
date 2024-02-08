using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Seeding;

namespace MultiSMS.Interface
{
    public class MultiSMSDbContext : IdentityDbContext<Administrator, IdentityRole<int>, int>
    {
        public virtual DbSet<Administrator> AspNetUsers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<SMSMessageTemplate> SMSMessageTemplates { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<EmployeeGroup> EmployeeGroups { get; set; }

        public MultiSMSDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Administrator>().HasData(SeedAdmin.GetAdminSeed());
        }
    }
}
