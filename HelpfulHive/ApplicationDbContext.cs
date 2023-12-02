using HelpfulHive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static HelpfulHive.Pages.AddRecordDialog;
using static System.Collections.Specialized.BitVector32;

namespace HelpfulHive
{

        public class ApplicationUser : IdentityUser
        {
            public string ProfileImagePath { get; set; }
        }

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RecordModel> Records { get; set; }
        public DbSet<TabItem> Tabs { get; set; }
        public DbSet<RecordContent> RecordsContent { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }  // Добавить эту строку



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  // Этот вызов очень важен!

            modelBuilder.Entity<RecordModel>()
          .HasOne(r => r.SubTab)
          .WithMany(t => t.Records)
          .HasForeignKey(r => r.SubTabId)
          .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<RecordContent>()
         .Property(e => e.ImageUrls)
         .HasColumnType("jsonb")
         .HasConversion(new JsonbListConverter())
         .Metadata
         .SetValueComparer(new ValueComparer<List<string>>(
             (c1, c2) => Enumerable.SequenceEqual(c1, c2),
             c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
             c => c.ToList()));




            modelBuilder.Entity<UserPreferences>()
 .HasOne(up => up.User)
 .WithMany()
 .HasForeignKey(up => up.UserId)
 .IsRequired()
 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreferences>()
                .HasOne(up => up.Record)
                .WithMany()
                .HasForeignKey(up => up.RecordId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreferences>()
      .Property(up => up.HasViewedNewCommonRecord)
      .IsRequired(); // Добавление нового поля HasViewed



            modelBuilder.Entity<IdentityUser>()
            .Property<string>("ProfileImagePath")
            .IsRequired(false); // У

        }

       

    }

}