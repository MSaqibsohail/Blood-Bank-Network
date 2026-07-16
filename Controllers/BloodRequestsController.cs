using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace BloodBankNetwork.Controllers
{
    public class BloodRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // INDEX: Operational search parameters
        // ==========================================
        public IActionResult Index(string search)
        {

            
            var requestsQuery = from r in _context.BloodRequests select r;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                requestsQuery = requestsQuery.Where(r => r.HospitalName.ToLower().Contains(search) || r.BloodGroup.ToLower().Contains(search));
            }

            var requests = requestsQuery.OrderByDescending(r => r.RequestDate).ToList();
            return View(requests);
        }

        // ==========================================
        // CREATE (GET) - UPDATED: Forcefully injecting Pakistan Local Time
        // ==========================================
        public IActionResult Create()
        {
            // Server ki UTC clock ko bypass karke Pakistan Time nikalna
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo pktZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pktZone);

            var freshRequest = new BloodRequest
            {
                RequestDate = pakistanTime // Frontend par accurate current PKT time bhejega
            };

            return View(freshRequest);
        }

        // ==========================================
        // CREATE (POST) - UPDATED: Safe handling for timezone conversion
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BloodRequest request)
        {
            if (ModelState.IsValid)
            {
                // Agar user manual input change na kare, toh backend par bhi Pakistan Time ensure karna
                DateTime utcTime = DateTime.UtcNow;
                TimeZoneInfo pktZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                request.RequestDate = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pktZone);
                
                if (string.IsNullOrEmpty(request.Status))
                {
                    request.Status = "Pending";
                }

                _context.BloodRequests.Add(request);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        // ==========================================
        // AUTOMATED APPROVAL MANAGEMENT ENGINE - UPDATED with PKT
        // ==========================================
        public IActionResult Approve(int id)
        {
            var request = _context.BloodRequests.Find(id);

            if (request == null)
                return NotFound();

            if (request.Status == "Approved")
            {
                TempData["ErrorMessage"] = "This request has already been approved.";
                return RedirectToAction(nameof(Index));
            }

            var stock = _context.BloodStocks
                .FirstOrDefault(x => x.BloodGroup == request.BloodGroup && x.Units >= request.UnitsNeeded);

            if (stock == null)
            {
                TempData["ErrorMessage"] = $"Cannot approve order: Insufficient stock for blood type {request.BloodGroup}.";
                return RedirectToAction(nameof(Index));
            }

            // Subtraction logic
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo pktZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pktZone);

            stock.Units -= request.UnitsNeeded;
            stock.LastUpdated = pakistanTime; // Stock record update time in PKT

            request.Status = "Approved";
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Hospital request for {request.HospitalName} approved and inventory deducted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // REJECT ACTION
        // ==========================================
        public IActionResult Reject(int id)
        {
            var request = _context.BloodRequests.Find(id);

            if (request == null)
                return NotFound();

            request.Status = "Rejected";
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Request marked as rejected.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // DELETE ACTION
        // ==========================================
        public IActionResult Delete(int id)
        {
            var request = _context.BloodRequests.Find(id);

            if (request == null)
                return NotFound();

            _context.BloodRequests.Remove(request);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}