using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankNetwork.Models
{
    [Table("Donor")]
    public class Donor
    {
        [Key]
        public int DonorID { get; set; }

        [Required(ErrorMessage = "Donor name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a blood group.")]
        public string BloodGroup { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        public DateTime? LastDonationDate { get; set; }

        public int TotalDonations { get; set; }
    }
}