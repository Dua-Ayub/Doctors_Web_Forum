using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_Web_Forum.Data;
using System.Security.Claims;

namespace Doctors_Web_Forum.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Models.Admin> _passwordHasher = new();

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);

            if (admin == null || _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard(string searchTerm)
        {
            if (User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return RedirectToAction("Login");

            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                usersQuery = usersQuery.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            var users = await usersQuery.ToListAsync();

            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.VerifiedUsers = await _context.Users.CountAsync(u => u.IsVerified);
            ViewBag.PendingUsers = await _context.Users.CountAsync(u => !u.IsVerified);
            ViewBag.SearchTerm = searchTerm;

            return View(users);
        }

        // POST: /Admin/Verify/5
        [HttpPost]
        public async Task<IActionResult> Verify(int id)
        {
            if (User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return RedirectToAction("Login");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsVerified = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        // POST: /Admin/Delete/5

        [HttpPost]
        public async Task<IActionResult>
        Delete(int id)
        {
            if (User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return RedirectToAction("Login");

            var user = await _context.Users
            .Include(u => u.Questions)
            .Include(u => u.Answers)
            .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                _context.Answers.RemoveRange(user.Answers);
                _context.Questions.RemoveRange(user.Questions);

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User deleted successfully.";
            }

            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}