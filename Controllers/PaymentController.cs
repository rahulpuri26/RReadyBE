using RoadReady.Models;
using RoadReady.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;
using RoadReady.Exceptions;
using RoadReady.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using RoadReady.DTO;

namespace RoadReady.Controllers
{
    [EnableCors("Policy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ApplicationDbContext context, IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all Payments.");
                var payments = _paymentService.GetAllPayments();

                // Manually map to DTOs
                var paymentDtos = payments.Select(payment => new PaymentDTO
                {
                    PaymentId = payment.PaymentId,
                    ReservationId = payment.ReservationId,  // Include ReservationId
                    UserId = payment.UserId,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status
                }).ToList();

                // Serialize response with valid JSON keys
                var response = new
                {
                    id = "1", // Adjusted from $id to id
                    values = paymentDtos // Adjusted from $values to values
                };

                return Ok(response);
            }
            catch (PaymentNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to retrieve all payments.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetPaymentById(int id)
        {
            try
            {
                _logger.LogInformation($"Retrieving payment with ID: {id}", id);
                var payment = _paymentService.GetPaymentById(id);

                if (payment == null)
                    return NotFound("Payment not found");

                // Map Payment to PaymentDTO
                var paymentDto = new PaymentDTO
                {
                    PaymentId = payment.PaymentId,
                    ReservationId = payment.ReservationId,  // Include ReservationId
                    UserId = payment.UserId,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status
                };

                return Ok(paymentDto);
            }
            catch (PaymentNotFoundException ex)
            {
                _logger.LogError(ex, $"Failed to retrieve Payment with ID: {id}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPaymentsByUserId(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var payments = await _context.Payments
                                          .Where(p => p.UserId == userId)
                                          .ToListAsync();

            if (payments == null || payments.Count == 0)
            {
                return NotFound(new { message = "No payments found for this user." });
            }

            // Manually map to DTOs
            var paymentDtos = payments.Select(payment => new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                ReservationId = payment.ReservationId,  // Include ReservationId
                UserId = payment.UserId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status
            }).ToList();

            // Return the response with valid JSON keys
            var response = new
            {
                id = "1", // Adjusted from $id to id
                values = paymentDtos // Adjusted from $values to values
            };

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post(Payment payment)
        {
            try
            {
                _logger.LogInformation("Creating new payment...");
                var result = _paymentService.AddPayment(payment);

                // Map Payment to PaymentDTO
                var paymentDto = new PaymentDTO
                {
                    PaymentId = payment.PaymentId,
                    ReservationId = payment.ReservationId,
                    UserId = payment.UserId,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status
                };

                return CreatedAtAction(nameof(GetPaymentById), new { id = result }, paymentDto);
            }
            catch (Exception)
            {
                _logger.LogError("Failed to create new Payment...");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult Put(Payment payment)
        {
            if (payment == null || payment.PaymentId <= 0 || payment.UserId == null || payment.ReservationId <= 0)
            {
                return BadRequest("Invalid payment data. Please ensure all fields are provided.");
            }

            try
            {
                _logger.LogInformation($"Updating Payment with ID: {payment.PaymentId}");
                var result = _paymentService.UpdatePayment(payment);
                if (result == "Payment not found")
                    return NotFound(result);

                // Map Payment to PaymentDTO
                var paymentDto = new PaymentDTO
                {
                    PaymentId = payment.PaymentId,
                    ReservationId = payment.ReservationId,
                    UserId = payment.UserId,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod,
                    Status = payment.Status
                };

                return Ok(paymentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update the payment.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting payment with ID: {id}", id);
                var result = _paymentService.DeletePayment(id);
                if (result == "Payment not found")
                    return NotFound(result);

                return Ok(result);
            }
            catch (PaymentNotFoundException ex)
            {
                _logger.LogError(ex, "Failed to delete the payment.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
