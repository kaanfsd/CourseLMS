using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseLMS.Models
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnrollmentID { get; set; }

        [Required]
        [DisplayName("UserName")]
        public string? Id { get; set; }

        [ForeignKey("Id")]
        [InverseProperty("Enrollments")]
        public User? User { get; set; }

        [Required]
        [DisplayName("Course Title")]
        public int? CourseID { get; set; }

        [ForeignKey("CourseID")]
        [InverseProperty("Enrollments")]
        public Course? Course { get; set; }

        //[Required]
        [DisplayName("Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        public Enrollment() { }
    }
}