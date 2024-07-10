using arts_core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public PaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("user-payment")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserPayment([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            int userId;
            string idClaim;
            idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out userId);

            var customPaging = await _unitOfWork.PaymentRepository.GetUserPayments(userId, pageNumber, pageSize);

            return Ok(customPaging);
        }

    }
}
