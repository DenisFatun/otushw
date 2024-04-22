using CommonLib.Handlers;
using CommonLib.Models.Token;
using HomeWorkOTUS.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeWorkOTUS.Controllers.v1
{
    [Route("v1/friend")]
    [ApiController]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendsService _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        [HttpPut("set")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> AddFriendAsync([FromQuery] Guid friendId)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _friendsService.AddFriendAsync(claims.ClientId, friendId);
            return Ok("Пользователь успешно указал своего друга");
        }

        [HttpPut("delete")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> RemoveFriendAsync([FromQuery] Guid friendId)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _friendsService.RemoveFriendAsync(claims.ClientId, friendId);
            return Ok("Пользователь успешно указал своего друга");
        }
    }
}
