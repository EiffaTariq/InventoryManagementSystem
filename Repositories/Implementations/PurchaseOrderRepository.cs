using IMS.Data;
using IMS.Models;
using IMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace IMS.Repositories.Implementations
{
    public class PurchaseOrderRepository: Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;
        public PurchaseOrderRepository(AppDbContext context) : base(context)
        {
           _context = context;

        }
        public async Task<PurchaseOrder> GetWithLineItemsAsync(int id)
        {
            return await _context.PurchaseOrders.Include(x => x.LineItems)
                .ThenInclude(li => li.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
