using arts_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {

        private IUnitOfWork _unitOfWork;
        public TypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlls()
        {
            var customResult = await _unitOfWork.TypeRepository.GetAllType();

            return Ok(customResult);
        }

        [HttpPost]
        public IActionResult CreateType([FromForm]Models.Type type)
        {
            var customResult = _unitOfWork.TypeRepository.CreateNewType(type);

            _unitOfWork.SaveChanges();

            return Ok(customResult);
        }
    }
}
