using Doctors_Web_Forum.Data;
using Doctors_Web_Forum.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Doctors_Web_Forum.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new HomeDashboardViewModel
            {
                TotalDoctors = _context.Users.Count(),
                LoggedInDoctors = _context.Users.Count(u => u.IsLoggedIn),
                TotalQuestions = _context.Questions.Count(),
                TotalAnswers = _context.Answers.Count()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}