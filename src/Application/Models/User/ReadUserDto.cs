using Domain.Enums;

namespace Application.Models.User
{
    public class ReadUserDto
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public sbyte IsActive { get; set; } = 0;
    }
}
