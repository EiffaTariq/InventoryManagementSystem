namespace IMS.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } //navigation properties
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<POLineItem> LineItems { get; set; }
    }
}
