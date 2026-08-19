namespace Doctors_Web_Forum.Models
{
    public class ApplicationUser
    {
        // Primary Key
        public int Id { get; set; }

        // Basic Information
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Professional Information
        public string Specialty { get; set; }
        public string? Qualification { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Achievements { get; set; }

        // Location
        public string? City { get; set; }
        public string? Country { get; set; }

        // Contact
        public string? PhoneNumber { get; set; }

        // Profile Settings
        public bool IsProfilePublic { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        // Login Status
        public bool IsLoggedIn { get; set; } = false;

        // Account Created Date
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}