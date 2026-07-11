using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
namespace IMS.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
        Task UpdateAsync(int id, CreateCategoryDto dto);
        Task DeleteAsync(int id);
    }
}
