namespace Dto.User
{
    public class UserRegistrationDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? StoreNumber { get; set; }
        public string? DepartmentName { get; set; }
        public string? WarehouseName { get; set; }
    }
}
