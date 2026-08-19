using Doctors_Web_Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Doctors_Web_Forum.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> Users {  get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Question> Questions {  get; set; }
        public DbSet<Answer> Answers {  get; set; }
        public DbSet<Specialty> Specialities {  get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Answer>()
          .HasOne(a => a.User)
          .WithMany(u => u.Answers)
          .HasForeignKey(a => a.UserId)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Specialty>().HasData(
                new Specialty { Id = 1, Name = "Cardiology" },
                new Specialty { Id = 2, Name = "Dermotology" },
                new Specialty { Id = 3, Name = "Pediatries" },
                new Specialty { Id = 4, Name = "General Medicine" }
                );
        }
        
    }
}
