using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Request
{
    public class CreatePurchaseOrderDto
    {
        [Required]
        public int SupplierId { get; set; }

        [Required, MinLength(1, ErrorMessage = "PO must have at least one line item")]
        public List<CreatePOLineItemDto> LineItems { get; set; }
    }
}
