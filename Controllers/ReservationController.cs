using RoadReady.Models;
using RoadReady.DTO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using RoadReady.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RoadReady.Repositories;

namespace RoadReady.Controllers
{
    [EnableCors("Policy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var reservations = _reservationService.GetAllReservations();

                if (reservations == null || !reservations.Any())
                    return NotFound("No reservations found.");

                var reservationDtos = reservations.Select(r => new ReservationDto
                {
                    ReservationId = r.ReservationId,
                    UserId = r.UserId,
                    CarId = r.CarId,
                    PickupDate = r.PickupDate,
                    DropoffDate = r.DropoffDate,
                    TotalAmount = r.TotalAmount,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    Car = new CarDto
                    {
                        CarId = r.Car?.CarId ?? 0,
                        Make = r.Car?.Make,
                        Model = r.Car?.Model,
                        Year = r.Car?.Year ?? 0,
                        Color = r.Car?.Color,
                        PricePerDay = r.Car?.PricePerDay ?? 0,
                        AvailabilityStatus = r.Car?.AvailabilityStatus,
                        ImageUrl = r.Car?.imageUrl
                    },
                    User = new UserDto
                    {
                        UserId = r.User?.UserId ?? 0,
                        FirstName = r.User?.FirstName,
                        LastName = r.User?.LastName,
                        Email = r.User?.Email,
                        Role = r.User?.Role
                    }
                }).ToList();

                return Ok(reservationDtos);
            }
            catch (ReservationNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to retrieve all reservations.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetReservationsByUserId(int userId)
        {
            var reservations = _reservationService.GetReservationByUserId(userId);
            if (reservations == null || !reservations.Any())
            {
                return NotFound("No reservations found for this user.");
            }

            // Including related Car and User entities in the query
            var reservationDtos = reservations.Select(r => new ReservationDto
            {
                ReservationId = r.ReservationId,
                UserId = r.UserId,
                CarId = r.CarId,
                PickupDate = r.PickupDate,
                DropoffDate = r.DropoffDate,
                TotalAmount = r.TotalAmount,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                Car = new CarDto
                {
                    CarId = r.Car?.CarId ?? 0,
                    Make = r.Car?.Make,
                    Model = r.Car?.Model,
                    Year = r.Car?.Year ?? 0,
                    Color = r.Car?.Color,
                    PricePerDay = r.Car?.PricePerDay ?? 0,
                    AvailabilityStatus = r.Car?.AvailabilityStatus,
                    ImageUrl = r.Car?.imageUrl
                },
                User = new UserDto
                {
                    UserId = r.User?.UserId ?? 0,
                    FirstName = r.User?.FirstName,
                    LastName = r.User?.LastName,
                    Email = r.User?.Email,
                    Role = r.User?.Role
                }
            }).ToList();

            return Ok(reservationDtos);
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public IActionResult GetReservationById(int id)
        {
            try
            {
                var reservation = _reservationService.GetReservationById(id);
                if (reservation == null)
                    return NotFound("Reservation not found");

                var reservationDto = new ReservationDto
                {
                    ReservationId = reservation.ReservationId,
                    UserId = reservation.UserId,
                    CarId = reservation.CarId,
                    PickupDate = reservation.PickupDate,
                    DropoffDate = reservation.DropoffDate,
                    TotalAmount = reservation.TotalAmount,
                    Status = reservation.Status,
                    CreatedAt = reservation.CreatedAt,
                    UpdatedAt = reservation.UpdatedAt,
                    Car = new CarDto
                    {
                        CarId = reservation.Car?.CarId ?? 0,
                        Make = reservation.Car?.Make,
                        Model = reservation.Car?.Model,
                        Year = reservation.Car?.Year ?? 0,
                        Color = reservation.Car?.Color,
                        PricePerDay = reservation.Car?.PricePerDay ?? 0,
                        AvailabilityStatus = reservation.Car?.AvailabilityStatus,
                        ImageUrl = reservation.Car?.imageUrl
                    },
                    User = new UserDto
                    {
                        UserId = reservation.User?.UserId ?? 0,
                        FirstName = reservation.User?.FirstName,
                        LastName = reservation.User?.LastName,
                        Email = reservation.User?.Email,
                        Role = reservation.User?.Role
                    }
                };

                return Ok(reservationDto);
            }
            catch (ReservationNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to retrieve reservation.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult Post(Reservation reservation)
        {
            try
            {
                var result = _reservationService.AddReservation(reservation);
                return CreatedAtAction(nameof(GetReservationById), new { id = result }, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add reservation.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public IActionResult Put(Reservation reservation)
        {
            try
            {
                var result = _reservationService.UpdateReservation(reservation);
                if (result == null)
                {
                    return NotFound("Reservation not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update reservation.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _reservationService.DeleteReservation(id);
                if (result == null)
                {
                    return NotFound("Reservation not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete reservation.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
