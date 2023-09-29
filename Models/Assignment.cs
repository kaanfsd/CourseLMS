using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseLMS.Models
{
    public class Assignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentID { get; set; }

        [Required]
        [DisplayName("Course Title")]
        public int CourseID { get; set; }

        [ForeignKey("CourseID")]
        [InverseProperty("Assignments")]
        public Course? Course { get; set; }

        [Required]
        [DisplayName("Assignment Title")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        public Assignment() { }

    }
}