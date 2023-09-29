    using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseLMS.Models
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DisplayName("Instructor")]
        public int InstructorID { get; set; }

        [ForeignKey("InstructorID")]
        [InverseProperty("Courses")]
        public User? User { get; set; }

        [Required]
        public string Category { get; set; }
        [DisplayName("Enrollment Limit")]
        public int EnrollmentCount { get; set; }

        [Required]
        public string ImageURL { get; set; }

        public Course() { }
        [NotMapped]
        [BindNever]
        public ICollection<Assignment>? Assignments { get; set; }
        [NotMapped]
        [BindNever]
        public ICollection<Enrollment>? Enrollments { get; set; }

    }
}