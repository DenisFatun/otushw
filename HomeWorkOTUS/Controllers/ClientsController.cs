using HomeWorkOTUS.Handlers;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Clients;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HomeWorkOTUS.Controllers
{
    [Route("user")]
    [ApiController]    
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(Guid), 200)]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            var clientId = await _clientsService.RegisterAsync(request);
            return Ok(clientId);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _clientsService.LoginAsync(request);
            if (string.IsNullOrEmpty(token))
                return NotFound("Пользователь не найден");
            return Ok(token);
        }

        [Authorize]
        [HttpGet("get")]
        [ProducesResponseType(typeof(Client), 200)]
        public async Task<IActionResult> Get([FromQuery] Guid id)
        {
            var client = await _clientsService.GetAsync(id);
            if ( client == null)
                return NotFound("Пользователь не найден");
            return Ok(client);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ClientSearchResponse), 200)]
        public async Task<IActionResult> Search([FromQuery] ClientSearchFilter filter)
        {
            var response = await _clientsService.SearchAsync(filter);            
            return Ok(response);
        }
    }
}
