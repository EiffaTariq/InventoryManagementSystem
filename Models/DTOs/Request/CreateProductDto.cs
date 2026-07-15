using System.ComponentModel.DataAnnotations;

namespace IMS.Models.DTOs.Request
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal UnitPrice { get; set; }
        [Range(0,int.MaxValue)]
        public int ReorderLevel { get; set; }
        [Required]
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
