using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BloodBankNetwork.Controllers
{
    public class BloodBanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodBanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // UPDATED: Search parameter integrated smoothly
        public IActionResult Index(string search)
        {

           
            var banks = from b in _context.BloodBanks select b;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                banks = banks.Where(b => b.BankName.ToLower().Contains(search) || b.City.ToLower().Contains(search));
            }

            return View(banks.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BloodBank bank)
        {
            if (ModelState.IsValid)
            {
                _context.BloodBanks.Add(bank);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(bank);
        }

        public IActionResult Edit(int id)
        {
            var bank = _context.BloodBanks.Find(id);

            if (bank == null)
                return NotFound();

            return View(bank);
        }

        [HttpPost]
        public IActionResult Edit(BloodBank bank)
        {
            _context.BloodBanks.Update(bank);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var bank = _context.BloodBanks.Find(id);

            if (bank != null)
            {
                _context.BloodBanks.Remove(bank);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}