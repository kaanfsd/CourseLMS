using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseLMS.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> opts) : base(opts) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserID)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseID)
            .OnDelete(DeleteBehavior.NoAction);

        }
    }
}