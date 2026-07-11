using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;

namespace IMS.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
        Task<SupplierResponseDto> GetByIdAsync(int id);
        Task<SupplierResponseDto> CreateAsync(CreateSupplierDto dto);
        Task UpdateAsync(int id, CreateSupplierDto dto);
        Task DeleteAsync(int id);
    }
}
