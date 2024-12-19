using RoadReady.Models;

namespace RoadReady.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserCreateDto
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = null!;
    }
}
