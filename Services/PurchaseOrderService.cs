using IMS.Data;
using IMS.Enums;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Models;
using IMS.Services.Interfaces;
using IMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

using IMS.Exceptions;

namespace IMS.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _poRepo;
        private readonly IRepository<Product> _repo;

        public PurchaseOrderService(IPurchaseOrderRepository poRepo, IRepository<Product> repo)
        {
            _poRepo = poRepo;
            _repo = repo;
        }

        public async Task<List<PurchaseOrderResponseDto>> GetAllAsync()
        {
            var orders = await _poRepo.GetAllAsync();

            return orders.Select(MapToResponseDto).ToList();
        }

        public async Task<PurchaseOrderResponseDto> GetByIdAsync(int id)
        {
            var order = await _poRepo.GetWithLineItemsAsync(id);

            if (order == null)
                throw new KeyNotFoundException("Purchase order not found.");

            return MapToResponseDto(order);
        }

        public async Task<PurchaseOrderResponseDto> CreateAsync(CreatePurchaseOrderDto dto, int createdByUserId)
        {
            var supplier = await _poRepo.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
                throw new KeyNotFoundException("Supplier not found.");

            var purchaseOrder = new PurchaseOrder
            {
                SupplierId = dto.SupplierId,
                CreatedByUserId = createdByUserId,
                Status = POStatus.Draft,
                CreatedDate = DateTime.UtcNow,
                LineItems = new List<POLineItem>()
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.LineItems)
            {
                var product = await _repo.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product with Id {itemDto.ProductId} not found.");

                var lineTotal = itemDto.Quantity * itemDto.UnitPrice;
                totalAmount += lineTotal;

                purchaseOrder.LineItems.Add(new POLineItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    //LineTotal = lineTotal
                });
            }

            purchaseOrder.TotalAmount = totalAmount;

            await _poRepo.AddAsync(purchaseOrder);
            await _poRepo.SaveChangesAsync();

            return await GetByIdAsync(purchaseOrder.Id);
        }
        public async Task DeleteAsync(int id)
        {
            var order
                = await _poRepo.GetWithLineItemsAsync(id);
            if (order == null)
                throw new KeyNotFoundException("Order not found.");
            if (order.Status != POStatus.Draft)
                throw new InvalidOperationException("Only draft orders can be deleted.");

            _poRepo.Delete(order); 
            await _poRepo.SaveChangesAsync();

        }

        //private async Task CreateStockMovementsForReceivedOrderAsync(PurchaseOrder order)
        //{
        //    foreach (var lineItem in order.LineItems)
        //    {
        //        _context.StockMovements.Add(new StockMovement
        //        {
        //            ProductId = lineItem.ProductId,
        //            Type = MovementType.In,
        //            Quantity = lineItem.Quantity,
        //            PurchaseOrderId = order.Id
        //        });
        //    }
        //}
        //public async Task<PurchaseOrderResponseDto> TransitionStatusAsync(int orderId, POStatus newStatus)
        //{
        //    var order = await _context.PurchaseOrders
        //        .Include(po => po.LineItems)
        //        .FirstOrDefaultAsync(po => po.Id == orderId);

        //    if (order == null)
        //        throw new KeyNotFoundException($"Purchase Order {orderId} not found.");

        //    bool isAllowed = AllowedTransitions.TryGetValue(order.Status, out var validNextStates)
        //                      && validNextStates.Contains(newStatus);

        //    if (!isAllowed)
        //        throw new InvalidStatusTransitionException(order.Status.ToString(), newStatus.ToString());

        //    order.Status = newStatus;

        //    if (newStatus == POStatus.FullyReceived)
        //    {
        //        await CreateStockMovementsForReceivedOrderAsync(order);
        //    }

        //    await _context.SaveChangesAsync();

        //    return MapToResponseDto(order);
        //}

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
                    LineTotal = li.Quantity * li.UnitPrice
                }).ToList()
            };
        }
        private static readonly Dictionary<POStatus, POStatus[]> AllowedTransitions = new()
        {
            { POStatus.Draft,             new[] { POStatus.Submitted, POStatus.Cancelled } },
            { POStatus.Submitted,         new[] { POStatus.Approved, POStatus.Cancelled } },
            { POStatus.Approved,          new[] { POStatus.PartiallyReceived, POStatus.FullyReceived, POStatus.Cancelled } 
            },
            { POStatus.PartiallyReceived, new[] { POStatus.FullyReceived, POStatus.Cancelled } },
            { POStatus.FullyReceived,     new[] { POStatus.Closed } },
            { POStatus.Closed,            Array.Empty<POStatus>() }, // terminal
            { POStatus.Cancelled,         Array.Empty<POStatus>() }  // terminal
        };
    }
}
