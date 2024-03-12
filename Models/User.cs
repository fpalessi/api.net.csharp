namespace apinet.Models
{
    public class User
    {
        public int Id { get; set; }

        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }

        public List<Order>? Orders { get; set; }

        public User()
        {
            Role = "User";
        }
    }
}