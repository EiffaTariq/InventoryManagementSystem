using System.Net.NetworkInformation;
using IMS.Enums;
namespace IMS.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public POStatus Status { get; set; }
        public int CreatedByUserId { get; set; } // FK
        public User CreatedByUser { get; set; } // Navigation Property
        public int SupplierId { get; set; } // FK
        public Supplier Supplier { get; set; }// Navigation property
        public ICollection<POLineItem> LineItems { get; set; }
    }
}
