using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctors_Web_Forum.Data;
using Doctors_Web_Forum.Models;
using System.Security.Claims;

namespace Doctors_Web_Forum.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password,
            string specialty, string qualification, int? yearsOfExperience, string city, string country, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Name, email, and password are required.";
                return View();
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.Error = "This email address is already registered.";
                return View();
            }

            var user = new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                Specialty = specialty,
                Qualification = qualification,
                YearsOfExperience = yearsOfExperience,
                City = city,
                Country = country,
                PhoneNumber = phoneNumber,
                IsVerified = false,
                IsProfilePublic = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            user.IsLoggedIn = true;
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userIdClaim.Value));

            if (user == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.IsOwner = true;
            ViewBag.IsAdmin = User.IsInRole("Admin");

            return View(user);
        }

        // View Doctor Profile
        [HttpGet]
        public async Task<IActionResult> ViewProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        // GET: /Account/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userIdClaim.Value));

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(int.Parse(userIdClaim.Value));

            if (user == null)
            {
                return RedirectToAction("Login");
            }
            user.FullName = model.FullName;
            user.Specialty = model.Specialty;
            user.Qualification = model.Qualification;
            user.YearsOfExperience = model.YearsOfExperience;
            user.City = model.City;
            user.Country = model.Country;
            user.PhoneNumber = model.PhoneNumber;
            user.IsProfilePublic = model.IsProfilePublic;


            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully.";

            return RedirectToAction("Profile");
        }


        // Doctors Directory
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Users
                .OrderBy(x => x.FullName)
                .ToListAsync();

            return View(doctors);
        }
        // Logout
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null)
            {
                var user = await _context.Users.FindAsync(int.Parse(userIdClaim.Value));
                if (user != null)
                {
                    user.IsLoggedIn = false;
                    await _context.SaveChangesAsync();
                }
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
