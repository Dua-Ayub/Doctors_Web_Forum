namespace Doctors_Web_Forum.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
         public string Description { get; set; }
        public string Status { get; set; } = "Open";
        public int View { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}
