using arts_core.Interfaces;
using arts_core.Models;
using arts_core.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var users = _unitOfWork.UserRepository.GetAll();
            return Ok(users);
        }
        [HttpPost]
        public IActionResult Create()
        {
            var user = new User { Email = "nhan@gmail.com",Fullname = "Nguyen thanh nhan", Password="123", Address="434 LE loi",PhoneNumber = "424242",Avatar = "afa", Verifired = false, Active = false };
            _unitOfWork.UserRepository.CreateOwner(user);
            _unitOfWork.SaveChanges();
            return Ok("");
        }

        [HttpPost]
        [Route("create-employee")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateEmployee(CreateEmployee account)
        {
            var customResult = await _unitOfWork.UserRepository.CreateEmployee(account);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpPost]
        [Route("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromForm]CreateCustomer account)
        {
            var customResult = await _unitOfWork.UserRepository.CreateCustomer(account);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpGet]
        [Route("get-employee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int pageNumber, [FromQuery] int pageSize = 20)
        {
            var customPaging = await _unitOfWork.UserRepository.GetAllEmployees(pageNumber, pageSize);

            return Ok(customPaging);
        }

        [HttpPost]
        [Route("change-image")]
        [Authorize]
        public async Task<IActionResult> ChangeUserAvatar([FromForm] IFormFile image)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;

            var customResult = await _unitOfWork.UserRepository.ChangeUserImage(email, image);

            return Ok(customResult);
        }

        [HttpPost]
        [Route("change-info")]
        [Authorize]
        public async Task<IActionResult> ChangeUserInfo([FromForm] UpdateUserRequest info)
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;

            var customResult = await _unitOfWork.UserRepository.EditUserInfo(email, info);

            return Ok(customResult);
        }

        [HttpPost]
        [Route("activate-employee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateEmployee([FromForm] int userId)
        {
            var customResult = await _unitOfWork.UserRepository.ActivateEmployee(userId);
            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpPost]
        [Route("deactivate-employee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateEmployee([FromForm] int userId)
        {
            var customResult = await _unitOfWork.UserRepository.DeactivateEmployee(userId);
            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpGet]
        [Route("get-a-employee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEmployee([FromQuery] int userId)
        {
            var customResult = await _unitOfWork.UserRepository.GetEmployee(userId);

            return Ok(customResult);
        }

        [HttpPut]
        [Route("update-employee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee([FromForm] RequestEmployeeUpdate info)
        {
            var customResult = await _unitOfWork.UserRepository.UpdateEmployee(info);

            return Ok(customResult);
        }

        [HttpGet]
        [Route("customer-ids")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> GetCustomerIds()
        {
            var customResult = await _unitOfWork.UserRepository.GetAllUserId();

            return Ok(customResult);
        }

        [HttpPut]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm]ChangePasswordRequest changePasswordRequest)
        {
            int userId;
            string idClaim;
            idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out userId);

            var customResult = await _unitOfWork.UserRepository.ChangePassword(userId, changePasswordRequest);

            return Ok(customResult);
        }

    }
}
