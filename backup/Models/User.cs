namespace Project_1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public List<UserRole>? UserRoles { get; set; }
    }
}
