using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseLMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;

namespace CourseLMS.Controllers
{
    [Authorize]
    

    public class CoursesController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public CoursesController(DatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Courses
        
        public async Task<IActionResult> Index(string searchString)
        { 
            var _courses= from c in _context.Courses
                              select c;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEnrollments = _context.Enrollments.Where(enrollment => enrollment.Id == userId).ToList();
            var instructorCourses = _context.Courses.Where(teaching => teaching.InstructorID == userId).ToList();
            var courseIDs = userEnrollments.Select(enrollment => enrollment.CourseID).ToList();
            var taughtCourses = instructorCourses.Select(teaching => teaching.CourseID).ToList();
            var courses = await _context.Courses
                .Where(course => courseIDs.Contains(course.CourseID) || taughtCourses.Contains(course.CourseID) || User.IsInRole(StaticDetail.Role_Admin))
                .Include(c => c.User)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                var filteredDbContext = await _context.Courses
                .Where(course => courseIDs.Contains(course.CourseID) || taughtCourses.Contains(course.CourseID) || User.IsInRole(StaticDetail.Role_Admin))
                .Where(c => c.Title!.Contains(searchString))
                .Include(c => c.User).ToListAsync();

                return View(filteredDbContext);

            }
            
            ViewData["Instructor"] = userId;

            return View(courses);
        }

        // GET: Courses/Details/5
        
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            // Retrieve assignments related to the course
            var CourseAssignments = await _context.Assignments
                .Where(assignment => assignment.CourseID == id)
                .ToListAsync();

            // Pass assignments to the view
            ViewData["Assignments"] = CourseAssignments;
            ViewData["Instructor"] = userId;

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Policy = "AdminOrInstructor")]
        public IActionResult Create()
        {
            var instructors = _userManager.GetUsersInRoleAsync(StaticDetail.Role_Instructor).Result.ToList();
            string userId = _userManager.GetUserId(User);
            if (User.IsInRole(StaticDetail.Role_Instructor))
            { ViewData["InstructorID"] = new SelectList(_context.Users.Where(i => i.Id == userId), "Id", "UserName"); }
            if (User.IsInRole(StaticDetail.Role_Admin))
            {
                ViewData["Instructors"] = new SelectList(instructors, "Id", "UserName");
            }

            //ViewData["InstructorID"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrInstructor")]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Description,InstructorID,Category,EnrollmentCount,ImageURL")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["InstructorID"] = new SelectList(_context.Users, "Id", "UserName", course.InstructorID);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Policy = "AdminOrInstructor")]
        public async Task<IActionResult> Edit(int? id)
        {
            var instructors = _userManager.GetUsersInRoleAsync(StaticDetail.Role_Instructor).Result.ToList();

            
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            if (User.IsInRole(StaticDetail.Role_Instructor))
            { ViewData["InstructorID"] = new SelectList(_context.Users.Where(i => i.Id == course.InstructorID), "Id", "UserName", course.InstructorID); }
            if (User.IsInRole(StaticDetail.Role_Admin))
            {
                ViewData["Instructors"] = new SelectList(instructors, "Id", "UserName");
            }
                
                return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrInstructor")]
        public async Task<IActionResult> Edit(int id, [Bind("CourseID,Title,Description,InstructorID,Category,EnrollmentCount,ImageURL")] Course course)
        {
            if (id != course.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Users, "Id", "UserName", course.InstructorID);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Policy = "AdminOrInstructor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOrInstructor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'DatabaseContext.Courses'  is null.");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
          return (_context.Courses?.Any(e => e.CourseID == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize(Roles = StaticDetail.Role_Admin)]
        public async Task<IActionResult> ExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();
            // Get the users from the database.
            var courses = await _context.Courses.ToListAsync();

            // Create an Excel file.
            using (var excelPackage = new ExcelPackage(stream))
            {
                // Add a worksheet to the Excel file.
                var worksheet = excelPackage.Workbook.Worksheets.Add("Courses");

                worksheet.Cells["A1"].Value = "Title";
                worksheet.Cells["B1"].Value = "Description";
                worksheet.Cells["C1"].Value = "Instructor";
                worksheet.Cells["D1"].Value = "Category";
                worksheet.Cells["E1"].Value = "Enrollment limit";
                worksheet.Cells["F1"].Value = "Image Url";

                int row = 2;
                foreach (var cours in courses)
                {
                    worksheet.Cells[row, 1].Value = cours.Title;
                    worksheet.Cells[row, 2].Value = cours.Description;
                    worksheet.Cells[row, 3].Value = cours.InstructorID;
                    worksheet.Cells[row, 4].Value = cours.Category;
                    worksheet.Cells[row, 5].Value = cours.EnrollmentCount;
                    worksheet.Cells[row, 6].Value = cours.ImageURL;
                    row++;
                }

                // Save the Excel file.
                excelPackage.SaveAs("courses.xlsx");
            }

            stream.Position = 0;
            string excelName = "CoursesRecord.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
