namespace IMS.Models.DTOs.Request
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
