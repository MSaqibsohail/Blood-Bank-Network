using BloodBankNetwork.Data;
using BloodBankNetwork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BloodBankNetwork.Controllers
{
    public class DonationRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================
        // INDEX: Warning-Free Fixed Search Query
        // ==========================
        public IActionResult Index(string search)
        {

            var recordsQuery = _context.DonationRecords
                .Include(d => d.Donor)
                .Include(d => d.BloodBank)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                
                // FIX (CS8602): Added null safety checks to prevent possibly null reference dereferencing
                recordsQuery = recordsQuery.Where(d => 
                    (d.Donor != null && d.Donor.Name != null && d.Donor.Name.ToLower().Contains(search)) || 
                    (d.BloodGroup != null && d.BloodGroup.ToLower().Contains(search))
                );
            }

            var records = recordsQuery.ToList();
            return View(records);
        }

        // ==========================
        // CREATE GET
        // ==========================
        public IActionResult Create()
        {
            ViewBag.DonorID = new SelectList(_context.Donors.OrderBy(d => d.Name), "DonorID", "Name");
            ViewBag.BankID = new SelectList(_context.BloodBanks.OrderBy(b => b.BankName), "BankID", "BankName");

            return View();
        }

        // ==========================
        // CREATE POST
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DonationRecord donation)
        {
            var donorProfile = _context.Donors.FirstOrDefault(d => d.DonorID == donation.DonorID);

            if (donorProfile != null)
            {
                donation.BloodGroup = donorProfile.BloodGroup;
                donorProfile.TotalDonations += 1;
                donorProfile.LastDonationDate = donation.DonationDate;
            }

            if (ModelState.IsValid)
            {
                _context.DonationRecords.Add(donation);

                var existingStock = _context.BloodStocks
                    .FirstOrDefault(s => s.BankID == donation.BankID && s.BloodGroup == donation.BloodGroup);

                if (existingStock != null)
                {
                    existingStock.Units += donation.UnitsDonated;
                    existingStock.LastUpdated = DateTime.Now;
                }
                else
                {
                    var newStock = new BloodStock
                    {
                        BankID = donation.BankID,
                        BloodGroup = donation.BloodGroup ?? "O+", 
                        Units = donation.UnitsDonated,
                        LastUpdated = DateTime.Now
                    };
                    _context.BloodStocks.Add(newStock);
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DonorID = new SelectList(_context.Donors.OrderBy(d => d.Name), "DonorID", "Name", donation.DonorID);
            ViewBag.BankID = new SelectList(_context.BloodBanks.OrderBy(b => b.BankName), "BankID", "BankName", donation.BankID);
            return View(donation);
        }

        // ==========================
        // EDIT GET
        // ==========================
        public IActionResult Edit(int id)
        {
            var donation = _context.DonationRecords.Find(id);

            if (donation == null)
                return NotFound();

            ViewBag.DonorID = new SelectList(
                _context.Donors.OrderBy(d => d.Name),
                "DonorID",
                "Name",
                donation.DonorID);

            ViewBag.BankID = new SelectList(
                _context.BloodBanks.OrderBy(b => b.BankName),
                "BankID",
                "BankName",
                donation.BankID);

            return View(donation);
        }

        // ==========================
        // EDIT POST
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DonationRecord donation)
        {
            var existing = _context.DonationRecords.Find(donation.DonationID);

            if (existing == null)
                return NotFound();

            existing.DonorID = donation.DonorID;
            existing.BankID = donation.BankID;
            existing.UnitsDonated = donation.UnitsDonated;
            existing.DonationDate = donation.DonationDate;

            var donor = _context.Donors.Find(donation.DonorID);
            if (donor != null)
            {
                existing.BloodGroup = donor.BloodGroup;
            }

            if (ModelState.IsValid)
            {
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DonorID = new SelectList(_context.Donors.OrderBy(d => d.Name), "DonorID", "Name", donation.DonorID);
            ViewBag.BankID = new SelectList(_context.BloodBanks.OrderBy(b => b.BankName), "BankID", "BankName", donation.BankID);
            return View(donation);
        }

        // ==========================
        // DELETE
        // ==========================
        public IActionResult Delete(int id)
        {
            var donation = _context.DonationRecords.Find(id);

            if (donation == null)
                return NotFound();

            _context.DonationRecords.Remove(donation);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}