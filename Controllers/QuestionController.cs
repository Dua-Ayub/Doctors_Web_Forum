using Doctors_Web_Forum.Data;
using Doctors_Web_Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctors_Web_Forum.Controllers
{
    public class QuestionController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // Questions List + Advanced Search
        // =========================
        public async Task<IActionResult> Index(
            string search,
            string city,
            string specialty,
            int? experience)
        {
            var questions = _context.Questions
                .Include(q => q.User)
                .Include(q => q.Specialty)
                .AsQueryable();

            // Search by Question Title
            if (!string.IsNullOrWhiteSpace(search))
            {
                questions = questions.Where(q => q.Title.Contains(search));
            }

            // Search by City
            if (!string.IsNullOrWhiteSpace(city))
            {
                questions = questions.Where(q => q.User.City.Contains(city));
            }

            // Search by Specialty
            if (!string.IsNullOrWhiteSpace(specialty))
            {
                questions = questions.Where(q => q.User.Specialty.Contains(specialty));
            }

            // Search by Minimum Experience
            if (experience.HasValue)
            {
                questions = questions.Where(q =>
                    q.User.YearsOfExperience >= experience.Value);
            }

            var result = await questions
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.City = city;
            ViewBag.Specialty = specialty;
            ViewBag.Experience = experience;

            return View(result);
        }

       
        // =========================
        // Ask Question (GET)
        // =========================
        [Authorize]
        [HttpGet]
        public IActionResult Ask()
        {
            return View();
        }

        // =========================
        // Ask Question (POST)
        // =========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Ask(Question model)
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            model.UserId = int.Parse(userIdClaim.Value);
            model.CreatedAt = DateTime.UtcNow;
            model.Status = "Open";
            model.View = 0;

            _context.Questions.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Question posted successfully!";

            return RedirectToAction("Ask");
        }

        // =========================
        // Question Details
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var question = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Specialty)
                .Include(q => q.Answers)
                .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            question.View++;

            await _context.SaveChangesAsync();

            return View(question);
        }

        // =========================
        // Add Answer
        // =========================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAnswer(int questionId, string description)
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var answer = new Answer
            {
                QuestionId = questionId,
                UserId = int.Parse(userIdClaim.Value),
                Description = description,
                CreatedAt = DateTime.UtcNow,
                IsVerifiedDoctor = false
            };

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Answer submitted successfully!";

            return RedirectToAction("Details", new { id = questionId });
        }

        // =========================
        // My Questions
        // =========================
        [Authorize]
        public async Task<IActionResult> MyQuestions()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim.Value);

            var questions = await _context.Questions
                .Include(q => q.Specialty)
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();

            return View(questions);
        }
    }
}