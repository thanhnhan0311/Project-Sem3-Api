using arts_core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace arts_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Sample : ControllerBase
    {      

        [HttpGet]
        public IActionResult Get()
        {

            return Ok("Ok");
        }
    }
}
