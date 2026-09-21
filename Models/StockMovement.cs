using Microsoft.EntityFrameworkCore.Storage;
using IMS.Enums;
namespace IMS.Models
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public StockMovementType Type { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

    }
}
