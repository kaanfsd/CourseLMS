using CourseLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CourseLMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Get the list of course IDs that the user is enrolled in.
                var enrolledCourseIds = await _context.Enrollments
                    .Where(e => e.Id == userId)
                    .Select(e => e.CourseID)
                    .ToListAsync();

                // Query for all courses.
                var courses = await _context.Courses
                    .Include(c => c.User)
                    .ToListAsync();

                // Create a dictionary to store whether each course is enrolled.
                var courseEnrollmentStatus = new Dictionary<int, bool>();

                // Check if each course is enrolled and store the result in the dictionary.
                foreach (var course in courses)
                {
                    courseEnrollmentStatus[course.CourseID] = enrolledCourseIds.Contains(course.CourseID);
                }

                // Pass the course list and enrollment status to the view.
                ViewData["Courses"] = courses;
                ViewData["CourseEnrollmentStatus"] = courseEnrollmentStatus;

                return View();
            }
            else
            {
                var courses = await _context.Courses
                    .Include(c => c.User)
                    .ToListAsync();

                return View(courses);
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}