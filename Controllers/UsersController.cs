using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using System.Linq;

namespace BloodBankNetwork.Controllers
{
    [Authorize(Roles = "Admin")] // 🔒 DOUBLE LOCK: Sirf Admin is area mein aa sakta hai
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users (List of all employees)
        public IActionResult Index()
        {

          
            var users = _context.Users.ToList();
            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User newUser)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "This Email address is already assigned to a team member.");
            }

            if (ModelState.IsValid)
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(newUser);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User editedUser)
        {
            if (_context.Users.Any(u => u.Email == editedUser.Email && u.UserID != editedUser.UserID))
            {
                ModelState.AddModelError("Email", "This Email address is already assigned to another team member.");
            }

            if (ModelState.IsValid)
            {
                _context.Users.Update(editedUser);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Employee details updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(editedUser);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Safety check: Prevent logged-in user from deleting themselves
            var currentUserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (user.Email == currentUserEmail)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account while logged in.";
                return RedirectToAction(nameof(Index));
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Employee removed successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}