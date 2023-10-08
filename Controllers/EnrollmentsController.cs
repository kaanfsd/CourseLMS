using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseLMS.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using System.Security.Claims;

namespace CourseLMS.Controllers
{
    [Authorize]
    public class EnrollmentsController : Controller
    {
        private readonly DatabaseContext _context;

        public EnrollmentsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(string searchString)
        {
            var enrollments = from e in _context.Enrollments
                              select e;

            var databaseContext = _context.Enrollments.Include(e => e.Course).Include(e => e.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                var filteredDbContext = _context.Enrollments.Include(e => e.Course).Include(e => e.User).Where(e => e.Course.Title!.Contains(searchString));
                return View(await filteredDbContext.ToListAsync());
            }


            return View(await databaseContext.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title");
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,Id,CourseID,EnrollmentDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName", enrollment.Id);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName", enrollment.Id);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("EnrollmentID,Id,CourseID,EnrollmentDate")] Enrollment enrollment)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "Title", enrollment.CourseID);
            //ViewData["Id"] = new SelectList(_context.Users, "Id", "UserName", enrollment.Id);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enrollments == null)
            {
                return Problem("Entity set 'DatabaseContext.Enrollments'  is null.");
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
          return (_context.Enrollments?.Any(e => e.EnrollmentID == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize(Roles = StaticDetail.Role_Admin)]
        public async Task<IActionResult> ExportExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();
            // Get the users from the database.
            var enrollments = await _context.Enrollments.ToListAsync();

            // Create an Excel file.
            using (var excelPackage = new ExcelPackage(stream))
            {
                // Add a worksheet to the Excel file.
                var worksheet = excelPackage.Workbook.Worksheets.Add("Enrollments");

                worksheet.Cells["A1"].Value = "UserName";
                worksheet.Cells["B1"].Value = "Course Title";
                worksheet.Cells["C1"].Value = "Enrollment Date";

                int row = 2;
                foreach (var enrollment in enrollments)
                {
                    worksheet.Cells[row, 1].Value = enrollment.Id;
                    worksheet.Cells[row, 2].Value = enrollment.CourseID;
                    worksheet.Cells[row, 3].Value = enrollment.EnrollmentDate.Day.ToString() + "/" + enrollment.EnrollmentDate.Month.ToString() + "/" + enrollment.EnrollmentDate.Year.ToString();
                    row++;
                }

                // Save the Excel file.
                excelPackage.SaveAs("enrollment.xlsx");
                stream.Position = 0;
                string excelName = "EnrollmentsRecord.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

       
        public async Task<IActionResult> Enroll(int? id)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get the current user's ID from the authentication system.
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming you use Identity for authentication.

                // Check if the user is already enrolled in the course asynchronously.
                bool isEnrolled = await IsUserEnrolledAsync(userId, id);

                if (isEnrolled)
                {
                    ViewBag.Message = "You are already enrolled in this course.";
                }
                else
                {
                    // Enroll the user in the course asynchronously.
                    await EnrollUserAsync(userId, id);
                    ViewBag.Message = "Enrollment successful!";
                }

                return RedirectToAction("Details", "Courses", new { id = id });
            }
            return RedirectToAction("Index", "Home");
        }

        // Check if a user is already enrolled in a course asynchronously.
        private async Task<bool> IsUserEnrolledAsync(string userId, int? courseId)
        {
            // Implement your logic to check if the user is already enrolled in the course asynchronously.
            // You may need to query your database or use your data access layer asynchronously.
            // Return true if enrolled, false otherwise.
            // Example:
            var enrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.Id == userId && e.CourseID == courseId);
            return enrollment != null;
        }

        // Enroll a user in a course asynchronously.
        private async Task EnrollUserAsync(string userId, int? courseId)
        {
            // Implement your logic to enroll the user in the course asynchronously.
            // You may need to add a new Enrollment record to your database or use your data access layer asynchronously.
            // Example:
            var enrollment = new Enrollment { Id = userId, CourseID = courseId };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

    }
}
