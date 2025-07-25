using HotelWebApp.Data.Repositories;
using HotelWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HotelWebApp.Data.Entities;

[Route("api/[controller]")]
[AllowAnonymous]
public class HistoryApiController : ControllerBase
{
    private readonly IReservationRepository _reservationRepo;
    private readonly UserManager<ApplicationUser> _userManager;

    public HistoryApiController(IReservationRepository reservationRepo, UserManager<ApplicationUser> userManager)
    {
        _reservationRepo = reservationRepo;
        _userManager = userManager;
    }

    // GET: /api/HistoryApi/ReservationsByGuest/{guestId}
    [HttpGet("ReservationsByGuest/{guestId}")]
    public async Task<ActionResult<IEnumerable<ReservationApiViewModel>>> GetReservationsByGuestId(string guestId)
    {
        // Valida o ID recebido
        if (string.IsNullOrEmpty(guestId))
        {
            return BadRequest("Guest ID is required."); // Retorna erro 400 se o ID for vazio
        }

        var user = await _userManager.FindByIdAsync(guestId);
        if (user == null)
        {
            return NotFound($"No guest found with ID '{guestId}'.");
        }

        var isGuest = await _userManager.IsInRoleAsync(user, "Guest");
        if (!isGuest)
        {
            // Se não for um hóspede, retorna uma mensagem específica
            return NotFound($"The user with ID '{guestId}' is not a guest and has no reservations.");
        }

        // Usa o repositório para obter as reservas do ID fornecido
        var reservations = await _reservationRepo.GetReservationsByGuestIdWithDetailsAsync(guestId);

        if (reservations == null || !reservations.Any())
        {
            // Retorna 404 se não houver reservas para este hóspede
            return NotFound($"No reservations found for guest with ID '{guestId}'.");
        }

        // Mapeia as entidades para o ViewModel de API (DTO)
        var result = reservations.Select(res =>
        {
            // lógica de cálculo de custo
            int numberOfNights = (res.CheckOutDate.Date - res.CheckInDate.Date).Days;
            if (numberOfNights <= 0) numberOfNights = 1;
            decimal stayCost = numberOfNights * (res.Room?.PricePerNight ?? 0);
            decimal amenitiesCost = res.ReservationAmenities?.Sum(ra => (ra.Amenity?.Price ?? 0) * ra.Quantity) ?? 0;

            return new ReservationApiViewModel
            {
                ReservationId = res.Id,
                RoomNumber = res.Room?.RoomNumber ?? "N/A",
                RoomType = res.Room?.Type.ToString() ?? "N/A",
                CheckInDate = res.CheckInDate,
                CheckOutDate = res.CheckOutDate,
                TotalCost = stayCost + amenitiesCost,
                Status = res.Status.ToString()
            };
        });

        // 4. Retornar os dados com um status 200 OK
        return Ok(result);
    }
}
