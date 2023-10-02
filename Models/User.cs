using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseLMS.Models
{
    public class User : IdentityUser
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int UserID { get; set; }

        //[Required]
        //public string Username { get; set; }

        //[Required]
        public string Email { get; set; }

        //[Required]
        public string Password { get; set; }

        //[Required]
        public string Role { get; set; }

        public User() { }

        [NotMapped]
        [BindNever]
        public ICollection<Assignment>? Assignments { get; set; }
        [NotMapped]
        [BindNever]
        public ICollection<Enrollment>? Enrollments { get; set; }
        [NotMapped]
        [BindNever]
        public ICollection<Course>? Courses { get; set; }
    }
}