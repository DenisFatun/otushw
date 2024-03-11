using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Technical;
using Microsoft.AspNetCore.Mvc;

namespace HomeWorkOTUS.Controllers
{
    [Route("technical")]
    [ApiController]
    public class TechnicalController : ControllerBase
    {
        private readonly ITechnicalService _technicalService;

        public TechnicalController(ITechnicalService technicalService)
        {
            _technicalService = technicalService;
        }

        [HttpPost("generate-users")]
        [ProducesResponseType(typeof(GenerateUsersResponse), 200)]
        public async Task<IActionResult> GenerateTestUsers(GenerateUsersRequest request)
        {
            var response = await _technicalService.GenerateUsersAsync(request);
            return Ok(response);
        }
    }
}
