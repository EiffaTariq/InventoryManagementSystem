namespace IMS.Models.DTOs.Request
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
    }
}
