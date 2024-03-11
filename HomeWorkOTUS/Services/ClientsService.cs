using HomeWorkOTUS.Infrastructure.Repos;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Clients;

namespace HomeWorkOTUS.Services
{
    public class ClientsService : IClientsService
    {
        private readonly IClientsRepo _clientsRepo;
        private readonly IJWTService _jwtService;
        public ClientsService(IClientsRepo clientsRepo, IJWTService jwtService)
        {
            _clientsRepo = clientsRepo;
            _jwtService = jwtService;
        }

        public async Task<Guid> RegisterAsync(RegistrationRequest request)
        {
            return await _clientsRepo.CreateAsync(request);
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var client = await _clientsRepo.GetByLoginAndPasswordAsync(request.ClientId, request.Password);
            if (client == null)
                return string.Empty;

            var token = _jwtService.GetToken(new Models.Token.TokenClaims { ClientId = request.ClientId });
            return token.ToString();
        }

        public async Task<Client> GetAsync(Guid id)
        {
            return await _clientsRepo.GetAsync(id);
        }

        public async Task<ClientSearchResponse> SearchAsync(ClientSearchFilter filter)
        {
            return await _clientsRepo.SearchAsync(filter);
        }
    }
}
