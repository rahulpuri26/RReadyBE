using RoadReady.Models;

namespace RoadReady.DTO
{
    public class CarDto
    {
        public int CarId { get; set; }
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string? Color { get; set; }
        public decimal PricePerDay { get; set; }
        public string AvailabilityStatus { get; set; } = null!;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CarCreateDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string? Color { get; set; }
        public decimal PricePerDay { get; set; }
        public string AvailabilityStatus { get; set; } = null!;
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
