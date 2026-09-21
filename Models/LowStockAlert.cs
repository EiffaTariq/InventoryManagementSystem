namespace IMS.Models
{
    public class LowStockAlert
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime AlertDate { get; set; }
    }
}