using CommonLib.Handlers;
using CommonLib.Models.Token;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Posts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace HomeWorkOTUS.Controllers.v1
{
    [Route("v1/post")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postService;
        private readonly IBusControl _rabbitBusControl;

        public PostsController(IPostsService postService, IBusControl rabbitBusControl)
        {
            _postService = postService;
            _rabbitBusControl = rabbitBusControl;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> AddPostAsync([FromBody] string text)
        {
            //using var source = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _postService.AddPostSimpleAsync(claims.ClientId, text);
            //await _rabbitBusControl.Publish(new AddPost { ClientId = claims.ClientId, Text = text }, source.Token);
            return Ok("Успешно создан пост");
        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> UpdatePostAsync([FromQuery] int id, [FromBody] string text)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _postService.UpdatePostAsync(claims.ClientId, id, text);
            return Ok("Успешно обновлен пост");
        }

        [HttpPut("delete")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> RemovePostAsync([FromQuery] int id)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _postService.RemovePostAsync(claims.ClientId, id);
            return Ok("Успешно удален пост");
        }

        [HttpGet("get")]
        [ProducesResponseType(typeof(ClientPost), 200)]
        public async Task<IActionResult> GetPostAsync([FromQuery] int id)
        {
            var post = await _postService.GetPostAsync(id);
            return Ok(post);
        }

        [HttpGet("feed")]
        [ProducesResponseType(typeof(List<ClientPost>), 200)]
        public async Task<IActionResult> GetPostAsync([FromQuery] PostListFilter request)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            var post = await _postService.ListPostFromCacheAsync(claims.ClientId, request);
            return Ok(post);
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<ClientPost>), 200)]
        public async Task<IActionResult> ListPostAsync([FromQuery] PostListFilter request)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            var posts = await _postService.ListPostAsync(claims.ClientId, request);
            return Ok(posts);
        }

        [HttpGet("self-list")]
        [ProducesResponseType(typeof(List<ClientPostSimple>), 200)]
        public async Task<IActionResult> AddPostAsync()
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            var posts = await _postService.PostsByAuthorAsync(claims.ClientId);
            return Ok(posts);
        }
    }
}
