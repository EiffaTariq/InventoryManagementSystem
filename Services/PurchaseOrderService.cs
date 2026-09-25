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
        private readonly AppDbContext _context;

        public PurchaseOrderService(IPurchaseOrderRepository poRepo, IRepository<Product> repo,
            AppDbContext context)
        {
            _poRepo = poRepo;
            _repo = repo;
            _context = context;
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
            { POStatus.Closed,            Array.Empty<POStatus>() }, 
            { POStatus.Cancelled,         Array.Empty<POStatus>() }  
        };

        public async Task<PurchaseOrderResponseDto> TransitionStatusAsync(int orderId, POStatus newStatus, string userRole)
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

        private async Task CreateStockMovementsForReceivedOrderAsync(PurchaseOrder order)
        {
            foreach (var lineItem in order.LineItems)
            {
                var movement = new StockMovement
                {
                    ProductId = lineItem.ProductId,
                    Type = StockMovementType.In,
                    Quantity = lineItem.Quantity,
                    PurchaseOrderId = order.Id,
                    Date = DateTime.UtcNow,
                    Notes = $"Stock received from PO #{order.Id}"
                };
                await _context.StockMovements.AddAsync(movement);
            }
            // don't SaveChanges here — TransitionStatusAsync handles it
        }
        public async Task<PagedResponseDto<PurchaseOrderResponseDto>> GetAllAsync(PurchaseOrderQueryParams queryParams)
        {
            // start with IQueryable — nothing hits DB yet
            var query = _poRepo.GetQueryable()
                .Include(po => po.Supplier)
                .Include(po => po.LineItems)
                    .ThenInclude(li => li.Product)
                .AsQueryable();

            // apply filters only if provided
            if (queryParams.SupplierId.HasValue)
                query = query.Where(po => po.SupplierId == queryParams.SupplierId);

            if (queryParams.CategoryId.HasValue)
                query = query.Where(po => po.LineItems.Any(li => li.Product.CategoryId == queryParams.CategoryId));

            // get total count BEFORE pagination
            var totalCount = await query.CountAsync();

            // apply pagination — Skip and Take
            var orders = await query
                .OrderByDescending(po => po.CreatedDate)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // map and return PagedResponseDto
            return new PagedResponseDto<PurchaseOrderResponseDto>
            {
                Items = orders.Select(MapToResponseDto).ToList(),
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
    }

}
