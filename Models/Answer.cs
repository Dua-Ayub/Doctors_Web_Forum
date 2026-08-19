namespace Doctors_Web_Forum.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsVerifiedDoctor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int QuestionId { get; set; }
        public Question Question { get; set; }

         public int UserId { get; set; }
        public ApplicationUser User { get; set; }


    }
}
