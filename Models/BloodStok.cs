using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankNetwork.Models
{
    public class BloodStock
    {
        [Key]
        public int StockID { get; set; }

        [Required]
        public int BankID { get; set; }

        [Required]
        public string BloodGroup { get; set; } = string.Empty;

        [Required]
        public int Units { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        // Explicitly map the foreign key here too
        [ForeignKey("BankID")]
        public virtual BloodBank? BloodBank { get; set; }
    }
}