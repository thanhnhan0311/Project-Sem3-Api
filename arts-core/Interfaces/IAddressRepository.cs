using arts_core.Data;
using arts_core.Models;
using Faker;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace arts_core.Interfaces
{
    public interface IAddressRepository : IRepository<Models.Address>
    {

        Task<CustomResult> CreateNewAddress(string email, Models.Address address);

        Task<CustomResult> GetUserAddress(string email);

        Task<CustomResult> GetUserAddressById(int addressId);

        Task<CustomResult> UpdateUserAddress(int userId, Models.Address address);



    }
    public class AddressRepository : GenericRepository<Models.Address>, IAddressRepository
    {
        private readonly ILogger<AddressRepository> _logger;
        public AddressRepository(ILogger<AddressRepository> logger, DataContext dataContext) : base(dataContext)
        {
            _logger = logger;
        }

        public async Task<CustomResult> CreateNewAddress(string email, Models.Address address)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

                var total = _context.Addresses.Where(a => a.UserId == user.Id).Count();

                var newAddress = new Models.Address()
                {
                    FullName = address.FullName,
                    PhoneNumber = address.PhoneNumber,
                    Ward = address.Ward,
                    Province = address.Province,
                    District = address.District,
                    AddressDetail = address.AddressDetail,
                    IsDefault = address.IsDefault,
                    UserId = user.Id,
                };

                if (total == 0)
                {
                    newAddress.IsDefault = true;
                }

                if (newAddress.IsDefault == true && total != 0)
                {
                    var oldDefaultAddress = await _context.Addresses.SingleOrDefaultAsync(a => a.UserId == user.Id && a.IsDefault == true);

                    if (oldDefaultAddress != null)
                    {
                        oldDefaultAddress.IsDefault = false;
                        _context.Addresses.Update(newAddress);
                    }
                }

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", newAddress);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }

        }


        public async Task<CustomResult> GetUserAddress(string email)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

                var listAddress = await _context.Addresses.Where(a => a.UserId == user.Id).OrderByDescending(a => a.Id).ToListAsync();

                return new CustomResult(200, "Success", listAddress);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }

        }

        public async Task<CustomResult> GetUserAddressById(int addressId)
        {
            try
            {
                var address = await _context.Addresses.SingleOrDefaultAsync(a => a.Id == addressId);

                return new CustomResult(200, "Success", address);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> UpdateUserAddress(int userId, Models.Address address)
        {
            try
            {
                var oldAddress = await _context.Addresses.SingleOrDefaultAsync(a => a.Id == address.Id);

                if (oldAddress != null)
                {
                    oldAddress.FullName = address.FullName;
                    oldAddress.PhoneNumber = address.PhoneNumber;
                    oldAddress.Province = address.Province;
                    oldAddress.Ward = address.Ward;
                    oldAddress.District = address.District;
                    oldAddress.AddressDetail = address.AddressDetail;

                    if (address.IsDefault == true)
                    {
                        var defaultAddress = await _context.Addresses.SingleOrDefaultAsync(a => a.UserId == userId && a.IsDefault == true);

                        if(defaultAddress != null)
                        {
                            defaultAddress.IsDefault = false;
                            _context.Addresses.Update(defaultAddress);
                        }
                        oldAddress.IsDefault = address.IsDefault;
                    }
                   
                    _context.Addresses.Update(oldAddress);
                    await _context.SaveChangesAsync();

                    return new CustomResult(200, "Success", oldAddress);
                }

                return new CustomResult(400, "Failed", null);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }
    }
}
