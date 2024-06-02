using CountsApp.Infrastructure.Services;
using CountsApp.Models.Counts;
using Microsoft.AspNetCore.Mvc;

namespace CountsApp.Controllers.v1
{
    [Route("v1/counts")]
    [ApiController]
    public class CountsController : ControllerBase
    {
        private readonly IDialogsCountService _dialogsCountService;

        public CountsController(IDialogsCountService dialogsCountService)
        {
            _dialogsCountService = dialogsCountService;
        }

        [HttpGet("{to:guid}/{from:guid}")]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> GetAsync(Guid to, Guid from)
        {            
            return Ok(await _dialogsCountService.GetCountAsync(to, from));
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> CreateAsync([FromBody] CountCreateRequest request)
        {
            await _dialogsCountService.CreateAsync(request.To, request.From, request.DialogId);
            return Ok("Группа успешно создана");
        }

        [HttpPut]
        [ProducesResponseType(typeof(int), 200)]
        public async Task<IActionResult> UpdateAsync([FromBody] CountUpdateRequest request)
        {
            await _dialogsCountService.UpdateAsync(request.To, request.From, request.LastReadId);
            return Ok("Счетчик успешно обновлен");
        }
    }
}
