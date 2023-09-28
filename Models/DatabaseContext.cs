using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseLMS.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> opts) : base(opts) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
