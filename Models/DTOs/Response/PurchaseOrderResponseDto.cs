using IMS.Enums;

namespace IMS.Models.DTOs.Response
{
    public class PurchaseOrderResponseDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public POStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<POLineItemResponseDto> LineItems { get; set; }
        public List<SupplierResponseDto> Suppliers { get; set; }
    }
}
