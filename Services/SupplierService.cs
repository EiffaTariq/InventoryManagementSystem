using IMS.Models;
using IMS.Models.DTOs.Request;
using IMS.Models.DTOs.Response;
using IMS.Repositories.Interfaces;
using IMS.Services.Interfaces;

namespace IMS.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _suppRepo;

        public SupplierService(IRepository<Supplier> suppRepo)
        {
            _suppRepo = suppRepo;
        }
        public async Task<IEnumerable<SupplierResponseDto>> GetAllAsync()
        {
            var supp = await _suppRepo.GetAllAsync();
            return supp.Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                TotalPurchaseOrders = s.PurchaseOrders?.Count() ?? 0
            });                
        }
        public async Task<SupplierResponseDto> GetByIdAsync(int id)
        {
            var supp = await _suppRepo.GetByIdAsync(id);
            if(supp == null)
            {
                throw new KeyNotFoundException("No Suppliers found");
            }
            return new SupplierResponseDto
            {
                Id = supp.Id,
                Name = supp.Name,
                Email = supp.Email,
                PhoneNumber = supp.PhoneNumber,
            };
        }

        public async Task<SupplierResponseDto> CreateAsync(CreateSupplierDto dto)
        {
            if(dto.PhoneNumber.Length > 11)
            {
                throw new ArgumentOutOfRangeException("Phone number too long");
            }
            if (dto.PhoneNumber.Length < 11)
            {
                throw new ArgumentOutOfRangeException("Phone number is less than range");
            }
            var supplier = new Supplier
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
            };
            await _suppRepo.AddAsync(supplier);
            await _suppRepo.SaveChangesAsync();
            return await GetByIdAsync(supplier.Id);
        }
        public async Task UpdateAsync(int id, CreateSupplierDto dto)
        {
            var supp = await _suppRepo.GetByIdAsync(id);
            if(supp == null)
            {
                throw new KeyNotFoundException("Supplier not found");
            }
            if (dto.PhoneNumber.Length > 11)
            {
                throw new ArgumentOutOfRangeException("Phone number too long");
            }
            if (dto.PhoneNumber.Length < 11)
            {
                throw new ArgumentOutOfRangeException("Phone number is less than range");
            }
            supp.Name = dto.Name;
            supp.Email = dto.Email;
            supp.PhoneNumber = dto.PhoneNumber;
            _suppRepo.Update(supp);
            await _suppRepo.SaveChangesAsync();

        }
        public async Task DeleteAsync(int id)
        {
            var supp = await _suppRepo.GetByIdAsync(id);
            if(supp == null)
            {
                throw new KeyNotFoundException("Supplier does not exist");
            }
            if (supp.PurchaseOrders.Any())
            {
                throw new Exception("Cannot delete this Supplier");
            }
            _suppRepo.Delete(supp);
            await _suppRepo.SaveChangesAsync();
        }
    }
}
