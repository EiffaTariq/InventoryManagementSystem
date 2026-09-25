using IMS.Enums;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;

namespace IMS.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PagedResponseDto<PurchaseOrderResponseDto>> GetAllAsync(PurchaseOrderQueryParams queryParams);

        Task<PurchaseOrderResponseDto> GetByIdAsync(int id);
        Task<PurchaseOrderResponseDto> CreateAsync(CreatePurchaseOrderDto dto, int createdByUserId);
        Task DeleteAsync(int id);
        Task<PurchaseOrderResponseDto> TransitionStatusAsync(int orderId, POStatus newStatus, string userRole);
    }
}
