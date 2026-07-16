using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using Microsoft.AspNetCore.Mvc;

namespace BloodBankNetwork.Controllers
{
    public class DonorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search)
        {

          
            // Fetch queryable donors from the database context
            var donors = _context.Donors.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Filter based on Name, Blood Group, or City matching search criteria
                donors = donors.Where(x =>
                    x.Name.Contains(search) ||
                    x.BloodGroup.Contains(search) ||
                    x.City.Contains(search));
            }

            return View(donors.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Donor donor)
        {
            if (ModelState.IsValid)
            {
                _context.Donors.Add(donor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(donor);
        }
        public IActionResult Edit(int id)
        {
            var donor = _context.Donors.Find(id);

            if (donor == null)
                return NotFound();

            return View(donor);
        }

        [HttpPost]
        public IActionResult Edit(Donor donor)
        {
            if (ModelState.IsValid)
            {
                _context.Donors.Update(donor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(donor);
        }

        public IActionResult Delete(int id)
        {
            var donor = _context.Donors.Find(id);

            if (donor != null)
            {
                _context.Donors.Remove(donor);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }

}