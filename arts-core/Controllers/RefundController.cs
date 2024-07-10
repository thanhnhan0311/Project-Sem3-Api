using arts_core.Interfaces;
using arts_core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefundController> _logger;
        private readonly IMailService _mailService;
        public RefundController(IUnitOfWork unitOfWork, ILogger<RefundController> logger,IMailService mailService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRefund([FromForm]RefundRequest refundRequest)
        {
            var result = await _unitOfWork.RefundRepository.CreateRefundAsync(refundRequest);
            return Ok(result);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRefundsByUserId(int userId)
        {
            var result = await _unitOfWork.RefundRepository.GetRefundsByUserIdAsync(userId);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRefundsForAdmin()
        {
            var result = await _unitOfWork.RefundRepository.GetAllRefundsAsync();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRefundForAdmin([FromForm]RefundReQuestForAdmin request)
        {
            var result = await _unitOfWork.RefundRepository.UpdateRefundAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("get-refund")]
        public async Task<IActionResult> GetRefundById([FromQuery] int refundId)
        {
            var result = await _unitOfWork.RefundRepository.GetRefundById(refundId);
            return Ok(result);
        }

        [HttpGet]
        [Route("get-user-refund")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserRefundById([FromQuery] int refundId)
        {
            int userId;
            string idClaim;
            idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out userId);

            var result = await _unitOfWork.RefundRepository.GetUserRefundById(userId, refundId);
            return Ok(result);
        }


        [HttpGet("testMail")]
        public async Task<IActionResult> Test()
        {
            _ = _mailService.SendMail(new MailRequestNhan("ngodinhtan1997@gmail.com", "Xin chao", "<h1>Your refund has been requested</h1>"))
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.LogError(t.Exception, "Some Exception in Test");
                    }
                });
            return Ok("");
        }
    }
}