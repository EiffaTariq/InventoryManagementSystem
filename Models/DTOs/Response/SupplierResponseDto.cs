namespace IMS.Models.DTOs.Response
{
    public class SupplierResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TotalPurchaseOrders { get; set; }
    }
}
