﻿using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface
{
    public class LocalDbContext : DbContext
    {
        public virtual DbSet<ApiToken> ApiTokens { get; set; } = default!;
        public virtual DbSet<ApiSmsSender> ApiSmsSenders { get; set; } = default!;
        public virtual DbSet<ApiSmsSenderUser> ApiSmsSenderUsers { get; set; } = default!;

        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApiSmsSender>()
                .HasOne(s => s.ApiToken)
                .WithMany()
                .HasForeignKey(s => s.ApiTokenId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApiSmsSenderUser>()
                .HasKey(x => new { x.ApiSmsSenderId, x.UserId }); // Composite Primary Key

            builder.Entity<ApiSmsSenderUser>()
                .HasIndex(x => x.UserId)
                .IsUnique();
        }
    }
}
