using arts_core.Interfaces;
using arts_core.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("new")]
        public IActionResult CreateProduct([FromForm] CreateProduct product)
        {
            var customResult = _unitOfWork.ProductRepository.CreateProduct(product);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        [Route("admin-products")]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] IEnumerable<int> categoryId, [FromQuery] string filterOption, [FromQuery] string searchValue ="")
        {
            var customPaging = await _unitOfWork.ProductRepository.GetPagingProducts(pageNumber, pageSize, categoryId,searchValue, filterOption);

            return Ok(customPaging);
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct([FromQuery]int id)
        {
            var customResult = await _unitOfWork.ProductRepository.GetProduct(id);

            return Ok(customResult);
        }

        [HttpGet]
        [Route("admin")]
        public async Task<IActionResult> GetProductAdmin([FromQuery] int id)
        {
            var customResult = await _unitOfWork.ProductRepository.GetProductAdmin(id);

            return Ok(customResult);
        }

        [HttpGet]
        [Route("product-variant")]
        public async Task<IActionResult> GetProductVariantDetail([FromQuery] int id)
        {
            var customResult = await _unitOfWork.ProductRepository.GetProductVariantInfo(id);

            return Ok(customResult);
        }

        [HttpPost]
        [Route("add-images")]
        public async Task<IActionResult> CreateImages([FromForm]RequestImages images)
        {
            var customResult = await _unitOfWork.ProductRepository.CreateImages(images);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpDelete]
        [Route("remove-image")]
        public async Task<IActionResult> RemoveImage([FromQuery] int imageId)
        {
            var customResult = await _unitOfWork.ProductRepository.DeleteImage(imageId);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpGet]
        [Route("listing-page")]
        public async Task<IActionResult> GetPagingProductForListingPage([FromQuery] int categoryId, [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int sort, [FromQuery] string searchValue = "", [FromQuery] float priceRangeMin = 0, [FromQuery] float priceRangeMax = float.MaxValue, [FromQuery] int ratingStar = 0)
        {
            var customPaging = await _unitOfWork.ProductRepository.GetPagingProductForListingPage(categoryId, pageNumber, pageSize, sort, searchValue, priceRangeMin, priceRangeMax, ratingStar);

            return Ok(customPaging);
        }

        [HttpPut]
        [Route("update-product")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProduct product)
        {
            var customResult = await _unitOfWork.ProductRepository.UpdateProduct(product);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }

        [HttpGet]
        [Route("search-product")]
        public async Task<IActionResult> SearchProduct([FromQuery] string searchValue)
        {
            var customResult = await _unitOfWork.ProductRepository.SearchProduct(searchValue);


            return Ok(customResult);
        }

        [HttpGet]
        [Route("get-newest")]
        public async Task<IActionResult> GetNewst()
        {
            var customResult = await _unitOfWork.ProductRepository.GetNewestProduct();
            return Ok(customResult);
        }

        [HttpGet]
        [Route("get-best-seller")]
        public async Task<IActionResult> GetBestSeller()
        {
            var customResult = await _unitOfWork.ProductRepository.GetBestSeller();
            return Ok(customResult);
        }

        [HttpGet]
        [Route("get-suggestion")]
        public async Task<IActionResult> GetSuggestion([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var customPaging = await _unitOfWork.ProductRepository.GetProductSuggestion(pageNumber,pageSize);

            return Ok(customPaging);
        }

        [HttpGet]
        [Route("category-suggestion")]
        public async Task<IActionResult> GetSuggestion([FromQuery] int categoryId, [FromQuery] int excludedId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var customPaging = await _unitOfWork.ProductRepository.GetRelatedProduct( categoryId,  excludedId,  pageNumber,  pageSize);

            return Ok(customPaging);
        }

        [HttpPut]
        [Route("update-variants")]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateVariants([FromBody] IEnumerable<VariantUpdate> variantUpdates)
        {
            var customResult = await _unitOfWork.ProductRepository.UpdateVariants(variantUpdates);

            return Ok(customResult);
        }


    }
}
