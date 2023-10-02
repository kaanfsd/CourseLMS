using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseLMS.Models
{
    public class DatabaseContext : IdentityDbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>().HasData(
            //    new User { UserID=1, Username="Kaan", Role="Admin", Password="kaan123", Email="kaanfsd9@gmail.com"},
            //    new User { UserID = 2, Username = "Ahmet", Role = "Instructor", Password = "ahmet123", Email = "ahmet@gmail.com" },
            //    new User { UserID = 3, Username = "Mehmet", Role = "User", Password = "mehmet123", Email = "mehmet@gmail.com" }
            //    );


            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.Id)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseID)
            .OnDelete(DeleteBehavior.NoAction);

        }
    }
}