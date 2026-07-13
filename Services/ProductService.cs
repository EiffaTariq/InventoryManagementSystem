using IMS.Models;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;
namespace IMS.Services
{
    public class ProductService: IProductService
    {
        private readonly IRepository<Product> _productRepo;

        public ProductService(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }
        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitPrice = p.UnitPrice,
                Quantity = p.Quantity,
                ReorderLevel = p.ReorderLevel,
                Description = p.Description,
                SupplierName = p.Supplier.Name,
                CategoryName = p.Category.Name
            });
            // 1. get all products from repo
            // 2. map each product to ProductResponseDto
            // 3. return the list
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("ProductNotFound");
               
            }
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                UnitPrice = product.UnitPrice,
                Quantity = product.Quantity,
                ReorderLevel = product.ReorderLevel,
                Description = product.Description,
                SupplierName = product.Supplier.Name,
                CategoryName = product.Category.Name
            };
           
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                UnitPrice = dto.UnitPrice,
                ReorderLevel = dto.ReorderLevel,
                SupplierId = dto.SupplierId,
                CategoryId = dto.CategoryId,
                Quantity = 0 // business rule: new products start at 0 stock
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();
            return await GetByIdAsync(product.Id);

          
        }

        public async Task UpdateAsync(int id, CreateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("ProductNotFound");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.UnitPrice = dto.UnitPrice;
            product.ReorderLevel = dto.ReorderLevel;
            product.SupplierId = dto.SupplierId;
            product.CategoryId = dto.CategoryId;

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException("ProductNotFound");
            }

            _productRepo.Delete(product);
            await _productRepo.SaveChangesAsync();
        }
    }
}
