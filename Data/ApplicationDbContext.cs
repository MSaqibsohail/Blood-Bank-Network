using BloodBankNetwork.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodBankNetwork.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Donor> Donors { get; set; }
        public DbSet<BloodBank> BloodBanks { get; set; }
        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<DonationRecord> DonationRecords { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Maps the DonationRecord model to the singular table
            modelBuilder.Entity<DonationRecord>().ToTable("DonationRecord");
            
            // 2. Maps your C# BloodStock model directly to your singular table "BloodStock"
            modelBuilder.Entity<BloodStock>().ToTable("BloodStock"); 

            // 3. Optional safety mapping for BloodRequest if it follows the singular pattern
            modelBuilder.Entity<BloodRequest>().ToTable("BloodRequest");
        }
    }
}