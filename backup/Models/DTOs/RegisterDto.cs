namespace Project_1.Models.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<int> RoleIds { get; set; } = new List<int> { 4 }; // Danh sách các RoleId
    }
}
