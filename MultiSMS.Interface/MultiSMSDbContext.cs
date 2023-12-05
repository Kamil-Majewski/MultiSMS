﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiSMS.Interface.Entities;

namespace MultiSMS.Interface
{
    public class MultiSMSDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public MultiSMSDbContext(DbContextOptions<MultiSMSDbContext> options) : base(options) { }
        public virtual DbSet<Group> Groups { get; set; }
    }
}
