using IMS.Models;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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
                Quantity = 0 
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
        public async Task<PagedResponseDto<ProductResponseDto>> GetAllAsync(ProductQueryParams queryParams)
        {
            var query = _productRepo.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (queryParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == queryParams.CategoryId);

            if (queryParams.SupplierId.HasValue)
                query = query.Where(p => p.SupplierId == queryParams.SupplierId);

            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                query = query.Where(p => p.Name.Contains(queryParams.SearchTerm));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Id)
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    UnitPrice = p.UnitPrice,
                    Quantity = p.Quantity,
                    ReorderLevel = p.ReorderLevel,
                    Description = p.Description,
                    SupplierName = p.Supplier.Name,
                    SupplierId = p.SupplierId,
                    CategoryName = p.Category.Name,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            return new PagedResponseDto<ProductResponseDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
    }
}
