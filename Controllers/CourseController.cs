using CourseLMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLMS.Controllers
{
    public class CourseController : Controller
    {
        private ICourseService courseService;
        public CourseController(ICourseService courseService)
        {
            this.courseService = courseService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
