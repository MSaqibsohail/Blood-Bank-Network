using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBankNetwork.Controllers
{
    public class BloodStocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodStocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BloodStocks
        public IActionResult Index(string search)
        {
            var stocksQuery = _context.BloodStocks.Include(x => x.BloodBank).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                stocksQuery = stocksQuery.Where(s => s.BloodGroup.ToLower().Contains(search));
            }

            var stocks = stocksQuery.ToList();
            return View(stocks);
        }

        // GET: BloodStocks/Create
        public IActionResult Create()
        {
            ViewBag.BankID = new SelectList(_context.BloodBanks.OrderBy(b => b.BankName), "BankID", "BankName");

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo pktZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pktZone);

            var freshStock = new BloodStock
            {
                LastUpdated = pakistanTime
            };

            return View(freshStock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BloodStock bloodStock)
        {
            if (ModelState.IsValid)
            {
                // Pakistan Standard Time setup
                DateTime utcTime = DateTime.UtcNow;
                TimeZoneInfo pktZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pktZone);

                // Input ko clean aur standard uppercase karein taake matching mein koi shak na rahe
                var searchGroup = bloodStock.BloodGroup.Trim().ToUpper();
                bloodStock.BloodGroup = searchGroup;

                // 1. Database se strict matching se check karein
                var existingStock = _context.BloodStocks
                    .FirstOrDefault(s => s.BankID == bloodStock.BankID && s.BloodGroup == searchGroup);

                try
                {
                    if (existingStock != null)
                    {
                        existingStock.Units += bloodStock.Units;
                        existingStock.LastUpdated = pakistanTime;
                        _context.BloodStocks.Update(existingStock);
                    }
                    else
                    {
                        bloodStock.LastUpdated = pakistanTime;
                        _context.BloodStocks.Add(bloodStock);
                    }

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // 2. BACKUP SAFETY (CATCH BLOCK): Agar pehle se entry maujood thi lekin kisi wajah se select query use dhoond nahi saki
                    // aur Unique Constraint ne crash karne ki koshish ki, to ye block crash nahi hone dega!

                    _context.ChangeTracker.Clear(); // Failed tracks saaf karein

                    // Case-insensitive database rules ke tehat dobara find karein
                    var fallbackStock = _context.BloodStocks
                        .AsEnumerable() // Memory mein la kar match karein taake string spaces/cases bypass ho sakein
                        .FirstOrDefault(s => s.BankID == bloodStock.BankID &&
                                             s.BloodGroup.Trim().Equals(searchGroup, StringComparison.OrdinalIgnoreCase));

                    if (fallbackStock != null)
                    {
                        fallbackStock.Units += bloodStock.Units;
                        fallbackStock.LastUpdated = pakistanTime;
                        _context.BloodStocks.Update(fallbackStock);
                        _context.SaveChanges();
                    }
                    else
                    {
                        // Agar koi bilkul hi alag database validation error hai tabhi throw karein
                        var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        return Content($"Asal Error Yeh Hai: {innerError} \n\n StackTrace: {ex.StackTrace}");
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.BankID = new SelectList(_context.BloodBanks.OrderBy(b => b.BankName), "BankID", "BankName", bloodStock.BankID);
            return View(bloodStock);
        }
        // GET: BloodStocks/Edit
        public IActionResult Edit(int id)
        {
            var stock = _context.BloodStocks.Find(id);

            if (stock == null)
                return NotFound();

            ViewBag.Banks = new SelectList(
                _context.BloodBanks,
                "BankID",
                "BankName",
                stock.BankID);

            return View(stock);
        }

        // POST: BloodStocks/Edit
        [HttpPost]
        public IActionResult Edit(BloodStock stock)
        {
            var existing = _context.BloodStocks.Find(stock.StockID);

            if (existing == null)
                return NotFound();

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo pktZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime pakistanTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, pktZone);

            existing.BankID = stock.BankID;
            existing.BloodGroup = stock.BloodGroup;
            existing.Units = stock.Units;
            existing.LastUpdated = pakistanTime;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: BloodStocks/Delete
        public IActionResult Delete(int id)
        {
            var stock = _context.BloodStocks.Find(id);

            if (stock == null)
                return NotFound();

            _context.BloodStocks.Remove(stock);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}