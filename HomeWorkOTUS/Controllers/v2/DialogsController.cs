using CommonLib.Handlers;
using CommonLib.Models.Token;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Dialogs;
using Microsoft.AspNetCore.Mvc;

namespace HomeWorkOTUS.Controllers.v2
{
    [Route("v1/dialog")]
    [ApiController]
    [Authorize]
    public class DialogsController : ControllerBase
    {
        private readonly IDialogsService _dialogsService;

        public DialogsController(IDialogsService dialogsService)
        {
            _dialogsService = dialogsService;
        }

        [HttpPost("{toClient:guid}/send")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> SendAsync(Guid toClient, [FromBody] DialogBase dialog)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            await _dialogsService.SendAsync(toClient, claims.ClientId, dialog);
            return Ok("Успешно отправлено сообщение");
        }

        [HttpGet("{fromClient:guid}/list")]
        [ProducesResponseType(typeof(IEnumerable<DialogBase>), 200)]
        public async Task<IActionResult> ListAsync(Guid fromClient)
        {
            var claims = (TokenClaims)HttpContext.Items["User"];
            var result = await _dialogsService.ListAsync(claims.ClientId, fromClient);
            return Ok(result);
        }
    }
}
