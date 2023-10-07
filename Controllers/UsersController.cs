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

public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: User/Index
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
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
    public async Task<IActionResult> Create(string email, string password, string roleName)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = email,
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
