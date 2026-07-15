namespace IMS.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
