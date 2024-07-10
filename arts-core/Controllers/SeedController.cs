using arts_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ISeed _seeder;
        private readonly ILogger<SeedController> _logger;
        public SeedController(ISeed seed, ILogger<SeedController> logger)
        {
            _seeder = seed;
            _logger = logger;
        }

        [HttpGet("seedProducts")]
        public IActionResult SeedProducts()
        {
            try
            {
                _seeder.SeedProductAndVariantData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in seedController");
            }
            return Ok("");
        }
        [HttpGet("seedUsers")]
        public IActionResult SeedUsers()
        {
            try
            {
                _seeder.SeedUser();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in seedController");
            }
            return Ok("");
        }

        [HttpGet("seedVariantAttributes")]
        public IActionResult SeedVariantAttributes()
        {
            try
            {
                _seeder.SeedVariantAttribute();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in seedController");
            }
            return Ok("");
        }
        [HttpGet("seedAddress")]
        public IActionResult SeedAddress()
        {
            _seeder.SeedAddress();
            return Ok("");
        }
        [HttpGet("seedPayments")]
        public IActionResult SeedPayments()
        {
            _seeder.SeedPayments();
            return Ok("");
        }
        [HttpGet("seedOrders")]
        public IActionResult SeedOrders()
        {
            _seeder.SeedOrders();
            return Ok("");
        }
        [HttpGet("seedReview")]
        public IActionResult SeedReview()
        {
            _seeder.SeedReview();
            return Ok("");
        }
        [HttpGet("test")]
        public IActionResult Test(string jsonUrl, int categoryId, string imageUrl)
        {
            //_seeder.SeedProductOfCategory("dollsProducts.json", 4,"Dolls");
            _seeder.SeedProductOfCategory(jsonUrl, categoryId, imageUrl);
            return Ok("");
        }

        [HttpGet("seeduser")]
        public async Task<IActionResult> SeedDataForUser()
        {
            try
            {
                await _seeder.SeedUsersGiu();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in seedController");
            }
            return Ok("Ok");
        }
        [HttpGet("seedordersgiu")]
        public async Task<IActionResult> SeesOrders()
        {
            try
            {
                await _seeder.SeedOrderGiu();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something wrong in seedController");
            }
            return Ok("Ok");
        }
    }
}