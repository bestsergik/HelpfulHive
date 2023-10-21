﻿using HelpfulHive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace HelpfulHive
{

    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RecordModel> Records { get; set; }
        public DbSet<TabItem> Tabs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Этот вызов очень важен!

            modelBuilder.Entity<RecordModel>()
                .HasOne(r => r.SubTab)
                .WithMany(t => t.Records)
                .HasForeignKey(r => r.SubTabId)
                .OnDelete(DeleteBehavior.Restrict);

      
        }
    }

}