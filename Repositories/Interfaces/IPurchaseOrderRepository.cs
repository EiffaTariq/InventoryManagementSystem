using IMS.Migrations;
using IMS.Models;
using IMS.Models.DTOs;
namespace IMS.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        Task<PurchaseOrder> GetWithLineItemsAsync(int id);
    }
}
