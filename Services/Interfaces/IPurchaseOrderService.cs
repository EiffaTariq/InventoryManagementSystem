using IMS.Enums;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;

namespace IMS.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderResponseDto>> GetAllAsync();
        Task<PurchaseOrderResponseDto> GetByIdAsync(int id);
        Task<PurchaseOrderResponseDto> CreateAsync(CreatePurchaseOrderDto dto, int createdByUserId);
        Task DeleteAsync(int id);
        Task TransitionStatusAsync(int orderId, POStatus newStatus, string userRole);
    }
}
