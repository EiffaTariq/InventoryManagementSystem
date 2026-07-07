namespace IMS.Models.DTOs.Response
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public string Description { get; set; }
        public string SupplierName { get; set; }
        public int SupplierId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
