using IMS.Data;
using IMS.Enums;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Models;
using IMS.Services.Interfaces;
using IMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

using IMS.Exceptions;
using System.Data;

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

        private static readonly Dictionary<POStatus, List<POStatus>> validTransitions = new()
        {
            { POStatus.Draft,             new List<POStatus> { POStatus.Submitted, POStatus.Cancelled } },
            { POStatus.Submitted,         new List<POStatus> { POStatus.Approved, POStatus.Cancelled } },
            { POStatus.Approved,          new List<POStatus> { POStatus.PartiallyReceived, POStatus.FullyReceived,  POStatus.Cancelled } },
            { POStatus.PartiallyReceived, new List<POStatus> { POStatus.FullyReceived, POStatus.Cancelled } },
            { POStatus.FullyReceived,     new List<POStatus> { POStatus.Closed } },
            { POStatus.Closed,            new List<POStatus>() },
            { POStatus.Cancelled,         new List<POStatus>() }
        };

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

        public async Task TransitionStatusAsync(int orderId, POStatus newStatus, string userRole)
        {
            var order = await _poRepo.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");
            if (!validTransitions.TryGetValue(order.Status, out var allowedNext)
            || !allowedNext.Contains(newStatus))
            {
                throw new InvalidStatusTransitionException(order.Status.ToString(), newStatus.ToString());
            }
            if (newStatus == POStatus.Approved && userRole != "Admin")
                throw new UnauthorizedAccessException("Only Admin can approve a purchase order");
            order.Status = newStatus;
            _poRepo.Update(order);
            await _poRepo.SaveChangesAsync();
            return MapToResponseDto(order);
        }
    }
}
