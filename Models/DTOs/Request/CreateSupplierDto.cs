using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Request
{
    public class CreateSupplierDto
    {
        [Required]
        [MaxLength(100)]

        public string Name { get; set; }

        [Required]
        [EmailAddress]               
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]
        public string PhoneNumber { get; set; }
    }
}
