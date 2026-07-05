using IMS.Models.DTOs;
using IMS.Models.DTOs.Request;
namespace IMS.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAll();
        Task<ProductDTO> GetById(int id);
        Task<ProductDTO> CreateAsync(CreateProductDTO productDTO);
        void UpdateAsync(ProductDTO productDTO);
        void Delete(int id);
    }
}
