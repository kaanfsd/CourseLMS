using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using CourseLMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

[Authorize(Roles = StaticDetail.Role_Admin)]
public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DatabaseContext _dbContext;


    public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, DatabaseContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    // GET: User/Index
    public async Task<IActionResult> Index(string searchString)
    {
        var _users = from u in _userManager.Users
                     select u;

        var users = _userManager.Users.ToList();

        if (!String.IsNullOrEmpty(searchString))
        {
            var filteredDbContext = _userManager.Users.Where(u => u.Email!.Contains(searchString));

            return View(await filteredDbContext.ToListAsync());

        }


        return View(users);
    }

    // GET: User/Create
    public IActionResult Create()
    {
        // Provide a list of available roles to the view
        var roles = _roleManager.Roles.ToList();
        ViewBag.Roles = new SelectList(roles, "Name", "Name");

        return View();
    }

    // POST: User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string email, string username, string roleName, string password, string name, string surname, string phoneNumber)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = username,
                Name = name,
                Surname = surname,
                PhoneNumber = phoneNumber,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Check if the role exists before adding the user to it
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
                // User creation successful
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Provide a list of available roles to the view
        var roles = _roleManager.Roles.ToList();
        ViewBag.Roles = new SelectList(roles, "Name", "Name");

        return View();
    }

    // GET: User/Edit/{id}
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: User/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, string email)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = email;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(user);
    }

    // GET: User/Delete/{id}
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: User/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var enrollments = _dbContext.Enrollments.Where(e => e.User.Id == id).ToList();

        foreach (var enrollment in enrollments)
        {
            _dbContext.Enrollments.Remove(enrollment);
        }

        await _dbContext.SaveChangesAsync();

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(user);
    }

    // GET: User/Details/{id}
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
    [HttpGet]
    [Authorize(Roles = StaticDetail.Role_Admin)]
    public async Task<IActionResult> ExportExcel()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var stream = new MemoryStream();
        // Get the users from the database.
        var users = await _userManager.Users.ToListAsync();

        // Create an Excel file.
        using (var excelPackage = new ExcelPackage(stream))
        {
            // Add a worksheet to the Excel file.
            var worksheet = excelPackage.Workbook.Worksheets.Add("Users");

            worksheet.Cells["A1"].Value = "First Name";
            worksheet.Cells["B1"].Value = "Last Name";
            worksheet.Cells["C1"].Value = "E-mail";
            //worksheet.Cells["A1"].Value = user.Name;
            //worksheet.Cells["A2"].Value = user.Surname;
            int row = 2;
            foreach (var user in users)
            {
                //worksheet.Cells[row, 1].Value = user.Name;
                //worksheet.Cells[row, 2].Value = user.Surname;
                worksheet.Cells[row, 3].Value = user.Email;
                row++;
            }

            // Save the Excel file.
            excelPackage.SaveAs("users.xlsx");
        }

        stream.Position = 0;
        string excelName = "UsersRecord.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    }
}