using arts_core.Interfaces;
using arts_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customResult = await _unitOfWork.CategoryRepository.GetAllCategories();

            return Ok(customResult);
        }

        [HttpPost]
        public IActionResult Create([FromForm] Category category)
        {
            var customResult = _unitOfWork.CategoryRepository.CreateNewCategory(category);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpGet]
        [Route("get-sale")]
        public async Task<IActionResult> GetSaleByCategory([FromQuery] string option)
        {
            var customResult= await _unitOfWork.CategoryRepository.GetSaleByCategory(option);

            return Ok(customResult);
        }
    }
}
