using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Seeding;

namespace MultiSMS.Interface
{
    public class MultiSMSDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public virtual DbSet<User> AspNetUsers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<SMSMessageTemplate> SMSMessageTemplates { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<EmployeeGroup> EmployeeGroups { get; set; }
        public virtual DbSet<ApiSettings> ApiSettings { get; set; }

        public MultiSMSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasData(SeedUsers.GetUserSeed());
            builder.Entity<ApiSettings>().HasData(SeedApiSettings.GetApiSettingsSeed());
            builder.Entity<IdentityRole<int>>().HasData(SeedRoles.Seed());
            builder.Entity<IdentityUserRole<int>>().HasData(AssignRoles.GrantRoles());
        }
    }
}
