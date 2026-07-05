namespace IMS.Models.DTOs
{
    public class ProductDto
    {
        public string Name {get; set;}
        public string Description {get; set;}
        public decimal UnitPrice {get; set;}
        public int ReorderLevel {get; set;}
    }
}
