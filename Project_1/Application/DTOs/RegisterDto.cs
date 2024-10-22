namespace Project_1.Application.DTOs
{
    public class RegisterDto
    {
        public String Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }        
        public string ConfirmPassword { get; set; }
        public List<int> RoleIds { get; set; } = new List<int> { 4 }; // Danh sách các RoleId
    }
}
