using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Request
{
    public class CreatePOLineItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
