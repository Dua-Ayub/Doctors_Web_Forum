namespace Doctors_Web_Forum.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; }
        public string Username { get; internal set; }
    }
}
