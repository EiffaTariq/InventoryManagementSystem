using Microsoft.EntityFrameworkCore.Storage;

namespace IMS.Models
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public Product Product { get; set; }
        public Type StockMovementType { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public int? PurchaseOrderId { get; set; }
        // Date, Notes (nullable string)
        // PurchaseOrderId (nullable int) — not every movement comes from a PO

    }
}
