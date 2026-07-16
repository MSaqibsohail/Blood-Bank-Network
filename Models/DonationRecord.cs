using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Need this for [ForeignKey]

namespace BloodBankNetwork.Models
{
    public class DonationRecord
    {
        [Key]
        public int DonationID { get; set; }

        [Required]
        public int DonorID { get; set; }

        [Required]
        public int BankID { get; set; }

        public string? BloodGroup { get; set; }

        [Required]
        public int UnitsDonated { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DonationDate { get; set; }

        // Explicitly map the foreign keys to prevent EF from guessing names like 'BloodBankBankID'
        [ForeignKey("DonorID")]
        public virtual Donor? Donor { get; set; }

        [ForeignKey("BankID")]
        public virtual BloodBank? BloodBank { get; set; }
    }
}