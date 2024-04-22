using HomeWorkOTUS.Handlers;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Dialogs;
using HomeWorkOTUS.Models.Token;
using Microsoft.AspNetCore.Mvc;

namespace HomeWorkOTUS.Controllers
{
    [Route("friend")]
    [ApiController]
    [Authorize]
    public class DialogController : ControllerBase
    {
        private readonly IDialogsService _dialogsService;

        public DialogController(IDialogsService dialogsService)
        {
            _dialogsService = dialogsService;
        }

        [HttpPost("dialog/{toClient:guid}/send")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SendAsync(Guid toClient, [FromBody] DialogBase dialog)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _dialogsService.SendAsync(toClient, claims.ClientId, dialog);
            return Ok("Успешно отправлено сообщение");
        }

        [HttpGet("dialog/{fromClient:guid}/list")]
        [ProducesResponseType(typeof(IEnumerable<DialogBase>), 200)]
        public async Task<IActionResult> ListAsync(Guid fromClient)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            var result = await _dialogsService.ListAsync(claims.ClientId, fromClient);
            return Ok(result);
        }
    }
}
