using RoadReady.Models;

namespace RoadReady.DTO
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public int? UserId { get; set; }
        public int? CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CarDto? Car { get; set; }
        public UserDto? User { get; set; }
    }

    public class ReservationCreateDto
    {
        public int? UserId { get; set; }
        public int? CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = null!;
    }
}
