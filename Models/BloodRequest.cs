using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankNetwork.Models
{
    [Table("BloodRequest")]
    public class BloodRequest
    {
        [Key]
        public int RequestID { get; set; }

        [Required(ErrorMessage = "Hospital name is required.")]
        public string HospitalName { get; set; }

        [Required(ErrorMessage = "Please select a blood group.")]
        public string BloodGroup { get; set; }

        [Required(ErrorMessage = "Please specify units needed.")]
        [Range(1, 100, ErrorMessage = "Requested volume must be between 1 and 100 units.")]
        public int UnitsNeeded { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Please select the urgency tier.")]
        public string Urgency { get; set; } // "High" / "Normal"

        public string Status { get; set; } = "Pending";
    }
}