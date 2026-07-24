using IMS.Data;
using IMS.Enums;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Models;
using IMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Services
{
    // Services/PurchaseOrderService.cs
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _context; // direct DbContext here, since we need Include()

        public PurchaseOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseOrderResponseDto>> GetAllAsync()
        {
            var orders = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                    .ThenInclude(li => li.Product)
                .ToListAsync();

            return orders.Select(MapToResponseDto).ToList();
        }

        public async Task<PurchaseOrderResponseDto> GetByIdAsync(int id)
        {
            var order = await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                    .ThenInclude(li => li.Product)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (order == null)
                throw new KeyNotFoundException("Purchase order not found.");

            return MapToResponseDto(order);
        }

        public async Task<PurchaseOrderResponseDto> CreateAsync(CreatePurchaseOrderDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(dto.SupplierId);
            if (supplier == null)
                throw new KeyNotFoundException("Supplier not found.");

            var purchaseOrder = new PurchaseOrder
            {
                SupplierId = dto.SupplierId,
                Status = POStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                LineItems = new List<POLineItem>()
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.LineItems)
            {
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product with Id {itemDto.ProductId} not found.");

                var lineTotal = itemDto.Quantity * itemDto.UnitPrice;
                totalAmount += lineTotal;

                purchaseOrder.LineItems.Add(new POLineItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    LineTotal = lineTotal
                });
            }

            purchaseOrder.TotalAmount = totalAmount;

            await _context.PurchaseOrders.AddAsync(purchaseOrder);
            await _context.SaveChangesAsync();

            // reload with related data for the response
            return await GetByIdAsync(purchaseOrder.Id);
        }

        private PurchaseOrderResponseDto MapToResponseDto(PurchaseOrder po)
        {
            return new PurchaseOrderResponseDto
            {
                Id = po.Id,
                SupplierId = po.SupplierId,
                SupplierName = po.Supplier.Name,
                Status = po.Status,
                CreatedDate = po.CreatedDate,
                TotalAmount = po.TotalAmount,
                LineItems = po.LineItems.Select(li => new POLineItemResponseDto
                {
                    ProductId = li.ProductId,
                    ProductName = li.Product.Name,
                    Quantity = li.Quantity,
                    UnitPrice = li.UnitPrice,
                    LineTotal = li.LineTotal
                }).ToList()
            };
        }
    }
}
