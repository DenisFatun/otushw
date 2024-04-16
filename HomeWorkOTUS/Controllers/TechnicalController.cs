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
        private readonly IPostsService _postsService;

        public TechnicalController(ITechnicalService technicalService, 
            IPostsService postsService)
        {
            _technicalService = technicalService;
            _postsService = postsService;
        }

        [HttpPost("generate-users")]
        [ProducesResponseType(typeof(GenerateUsersResponse), 200)]
        public async Task<IActionResult> GenerateTestUsers(GenerateUsersRequest request)
        {
            var response = await _technicalService.GenerateUsersAsync(request);
            return Ok(response);
        }

        [HttpPost("posts-cache-update")]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> UpdatePostCacheAsync(PostCacheFilter request)
        {
            var response = await _postsService.UpdatePostCacheAsync(request.ClientIdList);
            return Ok(response);
        }

        public class PostCacheFilter
        {
            public List<Guid> ClientIdList { get; set; }
        }
    }
}
