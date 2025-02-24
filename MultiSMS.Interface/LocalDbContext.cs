using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface
{
    public class LocalDbContext : DbContext
    {
        public virtual DbSet<ApiSmsSender> ApiSmsSenders { get; set; } = default!;

        public LocalDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
