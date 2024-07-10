using arts_core.Interfaces;
using arts_core.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromForm] RequestReview review)
        {
            string idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int userId;
            int.TryParse(idClaim, out userId);

            var reviewResult = await _unitOfWork.ReviewRepository.CreateReview(userId, review);

            _unitOfWork.SaveChanges();

            return Ok(reviewResult);
        }
        [HttpGet]
        [Route("checkReview")]
        [Authorize]
        public async Task<IActionResult> CheckReview(int productId)
        {
            string idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int userId;
            int.TryParse(idClaim, out userId);
            var checkReview = await _unitOfWork.ReviewRepository.CheckReview(userId, productId);

            return Ok(checkReview);
        }
        [HttpGet]
        [Route("allReview")]
        public async Task<IActionResult> GetAllReview([FromQuery] int productId, [FromQuery] int pageNumber, [FromQuery] int pageSize, int star, [FromQuery] string search = "")
        {
            var listReview = await _unitOfWork.ReviewRepository.GetAllReviewProductAsync(productId, pageNumber, pageSize, star, search);
            return Ok(listReview);
        }
        [HttpGet]
        [Route("totalStar")]
        public async Task<IActionResult> TotalStar(int productId)
        {
            var totalStar = await _unitOfWork.ReviewRepository.TotalStar(productId);
            return Ok(totalStar);
        }

        [HttpGet]
        [Route("get-all-review-user")]
        [Authorize]

        public async Task<IActionResult> GetAllRatingByUser()
        {
            string idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int userId;
            int.TryParse(idClaim, out userId);

            var reviews = await _unitOfWork.ReviewRepository.GetAllReviewProductByUserAsync(userId);
            return Ok(reviews);
        }

        [HttpDelete]
        [Route("delete-review")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteReview([FromQuery]int reviewId)
        {

            var reviews = await _unitOfWork.ReviewRepository.DeleteReviewByAdmin(reviewId);
            _unitOfWork.SaveChanges();
            return Ok(reviews);
        }
    }
}
