using HotelWebApp.Data.Entities;
using HotelWebApp.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelWebApp.Controllers
{
    /// <summary>
    /// Manages promotional offers and special deals (Admin interface)
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class PromotionsController : Controller
    {
        private readonly IPromotionRepository _promotionRepo;

        public PromotionsController(IPromotionRepository promotionRepo)
        {
            _promotionRepo = promotionRepo;
        }

        // GET: Promotions
        public async Task<IActionResult> Index()
        {
            var promotions = await _promotionRepo.GetAllAsync();
            return View(promotions);
        }

        // GET: Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _promotionRepo.GetByIdAsync(id.Value);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // GET: Promotions/Create
        public IActionResult Create()
        {
            var model = new Promotion
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1)
            };
            return View(model);
        }

        // POST: Promotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,StartDate,EndDate,DiscountPercentage,IsActive,ImageUrl,Terms")] Promotion promotion)
        {
            if (promotion.EndDate <= promotion.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            if (ModelState.IsValid)
            {
                await _promotionRepo.CreateAsync(promotion);
                TempData["SuccessMessage"] = "Promotion created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _promotionRepo.GetByIdAsync(id.Value);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate,DiscountPercentage,IsActive,ImageUrl,Terms")] Promotion promotion)
        {
            if (id != promotion.Id)
            {
                return NotFound();
            }

            if (promotion.EndDate <= promotion.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date.");
            }

            if (ModelState.IsValid)
            {
                await _promotionRepo.UpdateAsync(promotion);
                TempData["SuccessMessage"] = "Promotion updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _promotionRepo.GetByIdAsync(id.Value);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _promotionRepo.DeleteAsync(id);
            TempData["SuccessMessage"] = "Promotion deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
