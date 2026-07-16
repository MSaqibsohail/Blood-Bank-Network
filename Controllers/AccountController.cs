using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // 🌟 Server-side session verification ke liye zaroori hai

namespace BloodBankNetwork.Controllers
{
    [AllowAnonymous] // 🔓 Login bina log-in ke khul sakega
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // 🌟 ULTIMATE TAB-CLOSE FIX: 
            // Browser mein bhale hi cookie zinda ho, agar server session khali hai (jo tab close hone par hota hai)
            // toh user ko zabardasti SignOut karo taake wo dashboard par jump na kar sake.
            if (User.Identity!.IsAuthenticated) 
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserSessionActive")))
                {
                    // Puraani zinda cookie ko server se terminate kar do
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return View();
                }
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Account/Login (UPDATED: Error Displaying Fix Added)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Input null checking to avoid errors
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and Password fields cannot be empty.";
                return View();
            }

            // Trim extra whitespaces
            email = email.Trim();
            password = password.Trim();

            // Database lookup
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role) 
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // COOKIE STORAGE FLAGS:
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false, // Cookie permanent save nahi hogi
                    AllowRefresh = false
                };

                // Purani clean-up processing
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Fresh authentication token login process
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                // SERVER SESSION TOKEN GENERATE KAREIN:
                HttpContext.Session.SetString("UserSessionActive", "True");

                // Sahi details daalne ke baad direct Dashboard par redirect karo
                return RedirectToAction("Index", "Dashboard");
            }

            // 💡 CRITICAL ERROR BLOCK: Agar record nahi mila, to ye lines error screen par pass karengi
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            ViewBag.Error = "Invalid Email or Password. Please try again.";

            return View();
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            // Session database/memory clear karein
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}