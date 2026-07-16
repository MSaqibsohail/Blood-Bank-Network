using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodBankNetwork.Models
{
    [Table("BloodBank")]
    public class BloodBank
    {
        [Key]
        public int BankID { get; set; }

        [Required(ErrorMessage = "Blood bank name is required.")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address details are required.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City location is required.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact phone number is required.")]
        public string ContactNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Operating hours are required.")]
        public string OperatingHours { get; set; } = string.Empty;
    }
}