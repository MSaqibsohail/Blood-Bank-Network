using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BloodBankNetwork.Controllers
{
    // 🌟 FIX: Browser ko bolo ke back button ke liye is page ko cache mein save MAT kare
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel
            // ... aapka baqi dashboard ka code waise hi rahega ...
            {
                TotalDonors = _context.Donors.Count(),
                TotalBanks = _context.BloodBanks.Count(),
                TotalUnits = _context.BloodStocks.Sum(x => (int?)x.Units) ?? 0,
                TotalRequests = _context.BloodRequests.Count(),
                TotalDonations = _context.DonationRecords.Count(),

                // Pull the 5 most recent donations (Include Donor to show names)
                RecentDonations = _context.DonationRecords
                    .Include(d => d.Donor)
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .ToList(),

                // Pull the 5 most recent blood requests
                RecentRequests = _context.BloodRequests
                    .OrderByDescending(r => r.RequestDate)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }
    }
}