using HotelWebApp.Data.Entities;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Pede ao UserManager a lista de utilizadores que pertencem à role "Employee".
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            return View(employees);
        }


        public IActionResult CreateEmployee()
        {
            // Apenas mostra o formulário vazio
            return View();
        }

        // POST: /UserManagement/CreateEmployee
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
                    EmailConfirmed = true // Confirmamos o email do funcionário automaticamente
                };

                // Cria o utilizador com a password fornecida
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Se a criação for bem-sucedida, atribui o role "Employee"
                    await _userManager.AddToRoleAsync(user, "Employee");
                    return RedirectToAction(nameof(Index)); // Redireciona para a lista de utilizadores
                }

                // Se houver erros (ex: email já existe), adiciona-os ao ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Se o modelo não for válido, devolve a view com os dados preenchidos
            return View(model);
        }

        // TODO: Implementar ações de Details, Edit e Delete
    }

}
