using HotelWebApp.Data.Entities;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Controller responsible for the administration of employee user accounts.
    /// All actions in this controller are restricted to users with the 'Admin' role.
    /// It uses ASP.NET Core Identity's UserManager to perform user operations.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Displays a list of all users who have the 'Employee' role.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return View(employees);
        }

        /// <summary>
        /// Displays the form to create a new employee account.
        /// </summary>
        public IActionResult CreateEmployee()
        {
            return View();
        }

        // POST: /UserManagement/CreateEmployee
        /// <summary>
        /// Handles the submission for creating a new employee.
        /// Creates the user and assigns them to the 'Employee' role.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    EmailConfirmed = true // Employee accounts are confirmed automatically by the admin.
                };

                // Use UserManager to create the user with the specified password.
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // If user creation is successful, add the user to the "Employee" role.
                    await _userManager.AddToRoleAsync(user, "Employee");
                    TempData["SuccessMessage"] = "Employee created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                // If creation fails, add errors to ModelState to be displayed in the view.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Displays read-only details of a specific employee.
        /// </summary>
        /// <param name="id">The user ID (GUID) of the employee.</param>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // GET: /UserManagement/Edit/{id}
        /// <summary>
        /// Displays the form to edit an employee's details.
        /// </summary>
        /// <param name="id">The user ID (GUID) of the employee to edit.</param>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditEmployeeViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
            return View(model);
        }

        // POST: /UserManagement/Edit/{id}
        /// <summary>
        /// Handles the submission of the employee edit form.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email; // Keep UserName in sync with Email for consistency.

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Employee details updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /UserManagement/Delete/{id}
        /// <summary>
        /// Displays the confirmation page before deleting an employee account.
        /// </summary>
        /// <param name="id">The user ID of the employee to delete.</param>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /UserManagement/Delete/{id}
        /// <summary>
        /// Deletes the specified employee account after confirmation.
        /// </summary>
        /// <param name="id">The user ID of the employee to delete.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Employee deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Error deleting employee.";
                foreach (var error in result.Errors)
                {
                    TempData["ErrorMessage"] += $" {error.Description}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}

