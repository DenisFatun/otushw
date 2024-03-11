using HomeWorkOTUS.Models.Clients;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IClientsService : IService
    {
        Task<Guid> RegisterAsync(RegistrationRequest request);
        Task<string> LoginAsync(LoginRequest request);
        Task<Client> GetAsync(Guid id);
        Task<ClientSearchResponse> SearchAsync(ClientSearchFilter filter);
    }
}
