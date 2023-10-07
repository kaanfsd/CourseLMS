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

namespace CourseLMS.Controllers
{
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
    }
}
