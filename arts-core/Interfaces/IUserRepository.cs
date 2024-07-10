using arts_core.Data;
using arts_core.Models;
using arts_core.RequestModels;
using arts_core.Service;
using Faker;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace arts_core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        void CreateOwner(User owner);
        void UpdateOwner(User owner);
        void DeleteOwner(User owner);

        Task<User> ManagerAuthenticate(RequestLogin account);

        Task<CustomResult> AdminLogin(RequestLogin account);

        Task<CustomResult> GetAdmin(string email);

        Task<CustomResult> GetEmployees();

        Task<bool> CheckEmailExist(string email);

        Task<bool> CheckPhoneExist(string phone);

        Task<CustomResult> CreateEmployee(CreateEmployee account);

        Task<CustomPaging> GetAllEmployees(int pageNumber, int pageSize);

        Task<CustomResult> CreateCustomer(CreateCustomer account);

        Task<CustomResult> CustomerLogin(RequestLogin account);

        Task<User> CustomerAuthenticate(RequestLogin account);

        Task<CustomResult> GetUser(string email);

        Task<CustomResult> ChangeUserImage(string email, IFormFile image);

        Task<CustomResult> EditUserInfo(string email, UpdateUserRequest info);

        Task<CustomResult> ActivateEmployee(int userId);
        Task<CustomResult> DeactivateEmployee(int userId);

        Task<CustomResult> GetEmployee(int userId);
        Task<CustomResult> UpdateEmployee(RequestEmployeeUpdate info);
        Task<CustomResult> SendMail(RequestModels.MailRequest request);
        Task<CustomResult> VerifyAccount(string email);
        Task<CustomResult> GetAllUserId();

        Task<CustomResult> ChangePassword(int userId, ChangePasswordRequest changePasswordRequest);

    }

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        public UserRepository(DataContext dataContext, ILogger<UserRepository> logger, IConfiguration configuration, IWebHostEnvironment env, IMailService mailService) : base(dataContext)
        {
            _logger = logger;
            _config = configuration;
            _env = env;
            _mailService = mailService;
        }

        public void CreateOwner(User owner)
        {
            Add(owner);
        }

        public void DeleteOwner(User owner)
        {
            Delete(owner);
        }
        public void UpdateOwner(User owner)
        {
            Update(owner);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int ownerId)
        {
            return await _context.Users.FindAsync(ownerId);
        }

        public async Task<User> ManagerAuthenticate(RequestLogin account)
        {
            try
            {
                var verified = await _context.Users.Include(u => u.RoleType).Where(u => u.Email == account.Email && (u.RoleType.Name == "Admin" || u.RoleType.Name == "Employee")).SingleOrDefaultAsync();

                if (verified != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(account.Password, verified.Password))
                    {
                        return verified;
                    }
                }

                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleType.Name),
                new Claim("Id", user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                    issuer: _config["JwtSettings:Issuer"],
                    audience: _config["JwtSettings:Audience"],
                    signingCredentials: credentials,
                    claims: claims,
                    expires: DateTime.Now.AddDays(7)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<CustomResult> AdminLogin(RequestLogin account)
        {
            var user = await ManagerAuthenticate(account);

        
            if (user == null)
            {
                return new CustomResult(404, "Not Found", null);
            }

            if (user.RoleTypeId == 5 && user.Active == false)
            {
                return new CustomResult(401, "Account is not active", null);
            }

            var token = CreateToken(user);

            return new CustomResult(200, "token", token);
        }

        public async Task<CustomResult> GetAdmin(string email)
        {
            var user = await _context.Users.Include(u => u.RoleType).SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new CustomResult(400, "Bad Request", null);
            }

            if(user != null && user.RoleTypeId == 5 && user.Active == false)
            {
                return new CustomResult(400, "Bad Request", null);
            }

            return new CustomResult(200, "Success", user);
        }

        public async Task<CustomResult> GetEmployees()
        {
            try
            {
                var list = await _context.Users.Include(u => u.RoleType).Where(u => u.RoleType.Name == "Employee").ToListAsync();

                return new CustomResult(200, "success", list);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<bool> CheckEmailExist(string email)
        {
            try
            {
                var verified = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

                if (verified == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckPhoneExist(string phone)
        {
            try
            {
                var verified = await _context.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phone);

                if (verified == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CustomResult> CreateEmployee(CreateEmployee account)
        {
            try
            {
                var verifiedEmail = await CheckEmailExist(account.Email);

                if (verifiedEmail == true)
                {
                    return new CustomResult(400, "Email already exist", null);
                }
                if (account.Phone != null)
                {
                    var verifiedPhone = await CheckPhoneExist(account.Phone);

                    if (verifiedPhone == true)
                    {
                        return new CustomResult(400, "Phone number already exist", null);
                    }
                }

                var employeeRole = await _context.Types.SingleOrDefaultAsync(r => r.NameType == "UserRole" && r.Name == "Employee");

                var employee = new User()
                {
                    Email = account.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(account.Password),
                    Active = true,
                    PhoneNumber = account.Phone,
                    Address = account.Address,
                    RoleType = employeeRole,
                    Fullname = account.FullName,
                };

                if (account.Avatar != null)
                {
                    var fileName = DateTime.Now.Ticks + account.Avatar.FileName;
                    var uploadPath = Path.Combine(_env.WebRootPath, "images");
                    var filePath = Path.Combine(uploadPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    account.Avatar.CopyTo(stream);
                    employee.Avatar = fileName;
                }

                _context.Users.Add(employee);

                _ = _mailService.SendMail(new MailRequestNhan(account.Email, "New employee",
                $"<h1>Your employee account has been created</h1><p>Your password is {account.Password}</p>"))
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError(t.Exception, "Some Exception in Test");
                }
            });

                return new CustomResult(200, "success", employee);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "failed", ex.Message);
            }

        }

        public async Task<CustomPaging> GetAllEmployees(int pageNumber, int pageSize)
        {
            var list = await _context.Users.Include(u => u.RoleType).Where(u => u.RoleType.Name == "Employee").Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var total = _context.Users.Include(u => u.RoleType).Where(u => u.RoleType.Name == "Employee").Count();

            var customPaging = new CustomPaging()
            {
                Status = 200,
                Message = "OK",
                CurrentPage = pageNumber,
                TotalPages = (total / pageSize),
                PageSize = pageSize,
                TotalCount = total,
                Data = list
            };

            return customPaging;
        }

        public async Task<CustomResult> CreateCustomer(CreateCustomer account)
        {
            try
            {
                var verifiedEmail = await CheckEmailExist(account.Email);

                if (verifiedEmail == true)
                {
                    return new CustomResult(400, "Email already exist", null);
                }

                var verifiedPhone = await CheckPhoneExist(account.PhoneNumber);

                if (verifiedPhone == true)
                {
                    return new CustomResult(400, "Phone number already exist", null);
                }

                var customerRole = await _context.Types.SingleOrDefaultAsync(r => r.NameType == "UserRole" && r.Name == "Customer");

                var customer = new User()
                {
                    Email = account.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(account.Password),
                    Active = true,
                    PhoneNumber = account.PhoneNumber,
                    Fullname = account.Name,
                    RoleType = customerRole,
                };

                _context.Users.Add(customer);
                //return new CustomResult(200, "success", customer);

                var token = CreateToken(customer);

                _ = _mailService.SendMail(new MailRequestNhan(customer.Email, "Verify Email",
                    $"<h1>Thank you for registering</h1>" +
                           $"<p>Please verify your email by clicking the following link: " +
                           $"<a href='{_config["AppSettings:ClientURL"]}?token={token}'>Verify Email</a></p>"))
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.LogError(t.Exception, "Some Exception in Test");
                    }
                });

                return new CustomResult(200, "Account created successfully. Please verify your email.", customer);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "failed", ex.Message);
            }
        }

        public async Task<CustomResult> CustomerLogin(RequestLogin account)
        {
            var user = await CustomerAuthenticate(account);

            if (user == null)
            {
                return new CustomResult(404, "Not Found", null);
            }

            var token = CreateToken(user);

            return new CustomResult(200, "token", token);
        }

        public async Task<User> CustomerAuthenticate(RequestLogin account)
        {
            try
            {
                var verified = await _context.Users.Include(u => u.RoleType).Where(u => u.Email == account.Email && (u.RoleType.Name == "Customer")).SingleOrDefaultAsync();

                if (verified != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(account.Password, verified.Password))
                    {
                        return verified;
                    }
                }

                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<CustomResult> GetUser(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return new CustomResult(400, "Bad Request", null);
            }

            return new CustomResult(200, "Success", user);
        }

        public async Task<CustomResult> ChangeUserImage(string email, IFormFile image)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                var fileName = DateTime.Now.Ticks + image.FileName;
                var uploadPath = Path.Combine(_env.WebRootPath, "images");
                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                image.CopyTo(stream);
                user.Avatar = fileName;

                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", user);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }


        }

        public async Task<CustomResult> EditUserInfo(string email, UpdateUserRequest info)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

                user.Fullname = info.FullName;
                user.Dob = DateTime.Parse(info.Dob);
                user.Gender = info.Gender;

                if (user.PhoneNumber != info.PhoneNumber)
                {
                    var verified = await CheckPhoneExist(info.PhoneNumber);
                    if (verified)
                    {
                        return new CustomResult(400, "Phone number exist", null);
                    }

                    user.PhoneNumber = info.PhoneNumber;
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return new CustomResult(200, "Success", user);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }



        }

        public async Task<CustomResult> ActivateEmployee(int userId)
        {
            try
            {
                var employee = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                employee.Active = true;

                

                return new CustomResult(200, "Succes", employee);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> DeactivateEmployee(int userId)
        {
            try
            {
                var employee = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                employee.Active = false;

                return new CustomResult(200, "Succes", employee);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> GetEmployee(int userId)
        {
            try
            {
                var employee = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                return new CustomResult(200, "Succes", employee);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> UpdateEmployee(RequestEmployeeUpdate info)
        {
            try
            {
                var employee = await _context.Users.SingleOrDefaultAsync(u => u.Email == info.Email);

                if (info.Phone != null && info.Phone != employee.PhoneNumber)
                {
                    var verified = await CheckPhoneExist(info.Phone);

                    if (verified == true)
                    {
                        return new CustomResult(404, "Phone number exist", info.Phone);
                    }
                }

                if (employee != null)
                {
                    employee.Password =( info.Password != null && info.Password != "") ? BCrypt.Net.BCrypt.HashPassword(info.Password) : employee.Password;
                    employee.Address = info.Address;
                    employee.PhoneNumber = info.Phone;
                    employee.Fullname = info.FullName;

                    if (info.Avatar != null)
                    {
                        var fileName = DateTime.Now.Ticks + info.Avatar.FileName;
                        var uploadPath = Path.Combine(_env.WebRootPath, "images");
                        var filePath = Path.Combine(uploadPath, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        info.Avatar.CopyTo(stream);
                        employee.Avatar = fileName;
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                    return new CustomResult(200, "Success", employee);
                }

                return new CustomResult(400, "Failed", null);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public async Task<CustomResult> VerifyAccount(string email)
        {


            try
            {
                var account = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
                account.Verifired = true;

                _context.Users.Update(account);

                return new CustomResult(200, "success", account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verifying the account");
                return new CustomResult(500, "An error occurred", ex.Message);
            }
        }

        public async Task<CustomResult> SendMail(MailRequest request)
        {
            try
            {
                await _mailService.SendEmailAsync(request);
                return new CustomResult(200, "Succesed", request);
            }
            catch (Exception ex)
            {
                return new CustomResult(401, "Fail", null);
            }
        }

        public async Task<CustomResult> GetAllUserId()
        {
            try
            {
                var ids = await _context.Users.Where(u => u.RoleTypeId == 6).Select(u => u.Id).ToListAsync();

                return new CustomResult(200, "Success", ids);
            }
            catch (Exception ex)
            {

                return new CustomResult(400, "Failed", ex.Message);
            }
        }

        public async Task<CustomResult> ChangePassword(int userId, ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

                if(!BCrypt.Net.BCrypt.Verify(changePasswordRequest.PreviousPassword, user.Password))
                {
                    return new CustomResult(400, "Wrong password", null);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordRequest.NewPassword);

                _context.Users.Update(user);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", null);

            }catch(Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }
    }

}
