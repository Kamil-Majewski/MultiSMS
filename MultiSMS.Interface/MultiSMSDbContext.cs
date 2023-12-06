using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface
{
    public class MultiSMSDbContext : IdentityDbContext<Administrator, IdentityRole<int>, int>
    {
        public MultiSMSDbContext(DbContextOptions<MultiSMSDbContext> options) : base(options) { }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeesGroup> EmployeeGroups { get; set; }
        public virtual DbSet<EmployeesRole> EmployeeRoles{  get; set; }
    }
}
