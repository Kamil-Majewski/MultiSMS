using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;
using MultiSMS.Interface.Seeding;

namespace MultiSMS.Interface
{
    public class MultiSMSDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public virtual DbSet<User> AspNetUsers { get; set; } = default!;
        public virtual DbSet<Employee> Employees { get; set; } = default!;
        public virtual DbSet<Group> Groups { get; set; } = default!;
        public virtual DbSet<SMSMessageTemplate> SMSMessageTemplates { get; set; } = default!;
        public virtual DbSet<Log> Logs { get; set; } = default!;
        public virtual DbSet<EmployeeGroup> EmployeeGroups { get; set; } = default!;
        public virtual DbSet<ApiSettings> ApiSettings { get; set; } = default!;

        public MultiSMSDbContext(DbContextOptions<MultiSMSDbContext> options) : base(options)
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
