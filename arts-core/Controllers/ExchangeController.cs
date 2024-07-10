using arts_core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ExchangeController> _logger;

        public ExchangeController(ILogger<ExchangeController> logger, IUnitOfWork unitOfWork )
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        [HttpPost]
        public async Task<IActionResult> CreateExchangeForClient([FromForm]ExchangeRequest request)
        {
            var result = await _unitOfWork.ExchangeRepository.CreateExchangeAsync(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEchangeForAdmin([FromForm] UpdateExchangeRequest request)
        {
            var result = await _unitOfWork.ExchangeRepository.UpdateExchangeAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("exchange-by-id")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> GetExchangeById([FromQuery] int exchangeId)
        {
            var result = await _unitOfWork.ExchangeRepository.GetExchangeById(exchangeId);
            return Ok(result);
        }

        [HttpGet]
        [Route("user-exchange-by-id")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserExchangeById([FromQuery] int exchangeId)
        {
            int userId;
            string idClaim;
            idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out userId);

            var result = await _unitOfWork.ExchangeRepository.GetUserExchangeById(userId,exchangeId);
            return Ok(result);
        }
    }
}
