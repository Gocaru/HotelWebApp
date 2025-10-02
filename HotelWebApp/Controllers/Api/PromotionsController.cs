using HotelWebApp.Data;
using HotelWebApp.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelWebApp.Controllers.Api
{
    /// <summary>
    /// Manages hotel promotions and special offers available to guests
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly HotelWebAppContext _context;

        public PromotionsController(HotelWebAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all currently active promotions
        /// </summary>
        /// <returns>List of promotions valid for today's date</returns>
        // GET: api/promotions
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<PromotionDto>>>> GetPromotions()
        {
            try
            {
                var today = DateTime.Today;
                var promotions = await _context.Promotions
                    .Where(p => p.IsActive && p.StartDate <= today && p.EndDate >= today)
                    .OrderByDescending(p => p.StartDate)
                    .ToListAsync();

                var promotionDtos = promotions.Select(p => new PromotionDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    DiscountPercentage = p.DiscountPercentage,
                    ImageUrl = p.ImageUrl,
                    Terms = p.Terms
                }).ToList();

                return Ok(new ApiResponse<List<PromotionDto>>
                {
                    Success = true,
                    Data = promotionDtos,
                    Message = $"Retrieved {promotionDtos.Count} active promotions"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<PromotionDto>>
                {
                    Success = false,
                    Message = "Error retrieving promotions",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retrieves details of a specific promotion by ID
        /// </summary>
        /// <param name="id">The promotion ID</param>
        /// <returns>Complete promotion details including terms and conditions</returns>
        // GET: api/promotions/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PromotionDto>>> GetPromotion(int id)
        {
            try
            {
                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

                if (promotion == null)
                {
                    return NotFound(new ApiResponse<PromotionDto>
                    {
                        Success = false,
                        Message = "Promotion not found"
                    });
                }

                var promotionDto = new PromotionDto
                {
                    Id = promotion.Id,
                    Title = promotion.Title,
                    Description = promotion.Description,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    DiscountPercentage = promotion.DiscountPercentage,
                    ImageUrl = promotion.ImageUrl,
                    Terms = promotion.Terms
                };

                return Ok(new ApiResponse<PromotionDto>
                {
                    Success = true,
                    Data = promotionDto,
                    Message = "Promotion retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PromotionDto>
                {
                    Success = false,
                    Message = "Error retrieving promotion",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
