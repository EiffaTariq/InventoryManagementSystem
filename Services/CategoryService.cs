using IMS.Models;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;

namespace IMS.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepo;
        public CategoryService(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var category = await _categoryRepo.GetAllAsync();
            return category.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ProductCount = c.Products.Count()
            });
        }
        public async Task<CategoryResponseDto> GetByIdAsync(int id)
        {

            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("No Cateogory found");
            }
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count(),
            };
        }
        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,

            };
            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();
            return await GetByIdAsync(category.Id);
        }
        public async Task UpdateAsync(int id, CreateCategoryDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            category.Name = dto.Name;
            category.Description = dto.Description;
            _categoryRepo.Update(category);
            await _categoryRepo.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category does not exist");
            }
            if (!category.Products.Any())
            {
                throw new Exception("Cannot delete this Category");
            }
            _categoryRepo.Delete(category);
            await _categoryRepo.SaveChangesAsync();
        }
    }
}
