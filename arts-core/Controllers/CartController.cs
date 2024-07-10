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
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CartController> _logger;
        public CartController(IUnitOfWork unitOfWork, ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }



        [HttpGet]
        [Route("quantity")]
        [Authorize]
        public async Task<IActionResult> GetUserCartQuantity()
        {
            var email = User.FindFirst(ClaimTypes.Email).Value;

            var customResult= await _unitOfWork.CartRepository.GetUserCartQuantity(email);

            return Ok(customResult);
        } 


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCart([FromForm] CartRequest cartRequest)
        {

            var email = User.FindFirst(ClaimTypes.Email).Value;
            var result = await _unitOfWork.CartRepository.CreateCartAsync(email, cartRequest.VariantId, cartRequest.Quantity);

            if (result.isOkay)
            {
                return Ok(new CustomResult(201, $"{result.Message}", result));
            }

            return Ok(new CustomResult(405, $"Create cartId fail", result));
        }

        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            int userId;
            string idClaim;
            List<Cart> carts;
            try
            {
                if (User.Claims.Any() && User.Claims != null)
                {
                    idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                    int.TryParse(idClaim, out userId);
                    carts = await _unitOfWork.CartRepository.GetCartsByUserIdAsync(userId);
                    if (carts == null || carts.Count == 0)
                    {
                        return Ok(new CustomResult(404, $"Nothing cart found by UserId {userId}", carts));
                    }
                    return Ok(new CustomResult(200, $"Cart found by UserId {userId}", carts));
                }
                else
                {
                    return Ok(new CustomResult(404, "UserClaim Null ", null));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong get cart by UserId  in cart controller");
            }
            return Ok("");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart(int cartId, int quanity)
        {
            try
            {
                var result = await _unitOfWork.CartRepository.UpdateCartById(cartId, quanity);
                if (result.isOkay)
                    return Ok(new CustomResult(201, $"Update cartId {cartId} success", result));
                else
                    return Ok(new CustomResult(405, $"Update cartId {cartId} fail", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update cartId {cartId}", cartId);
            }
            return Ok("");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCart([FromQuery] int cartId)
        {
            try
            {
                var result = await _unitOfWork.CartRepository.DeleteCartById(cartId);
                return Ok(new CustomResult(200, $"{result.Message}", result));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("TotalAmount")]
        public async Task<IActionResult> GetTotalAmount()
        {

            int userId;
            string idClaim;
            try
            {
                if (User.Claims.Any() && User.Claims != null)
                {
                    idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                    int.TryParse(idClaim, out userId);
                    var result = await _unitOfWork.CartRepository.GetTotalAmountByUserId(userId);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return Ok("");
        }

        [HttpPut("UpdateAllCartChecked")]
        public async Task<IActionResult> UpdateAllCartChecked([FromForm] bool isCheckedState)
        {
            int userId;
            string idClaim;
            try
            {
                if (User.Claims.Any() && User.Claims != null)
                {
                    idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
                    int.TryParse(idClaim, out userId);
                    var result = await _unitOfWork.CartRepository.UpdateAllCartCheckedAsync(userId, isCheckedState);
                    return Ok(result);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Ok("");
        }

        [HttpPut("UpdateCartCheckedById")]
        public async Task<IActionResult> UpdateCartCheckedById([FromForm] int cartId, [FromForm] bool isCheckedState)
        {
            var result = await _unitOfWork.CartRepository.UpdateCartCheckedByIdAsync(cartId, isCheckedState);

            return Ok(result);
        }

        [HttpPost("CreatePayment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromForm] PaymentRequest paymentRequest)
        {
            int userId;
            string idClaim;
            idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            int.TryParse(idClaim, out userId);

            var customResult = await _unitOfWork.CartRepository.CreatePayment(userId, paymentRequest);

           _unitOfWork.SaveChanges();
            return Ok(customResult);

        }

    }

    public static class Extension
    {
        public static List<T> Paginate<T>(this List<T> records, int pageNumber = 1, int pageSize = 10)
        {
            return records.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
    }


}
