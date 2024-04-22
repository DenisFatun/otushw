using CommonLib.Infrastructure.Repos;
using HomeWorkOTUS.Models.Clients;

namespace HomeWorkOTUS.Infrastructure.Repos
{
    public interface IClientsRepo : IRepo
    {
        Task<Guid> CreateAsync(RegistrationRequest request);
        Task<Client> GetByLoginAndPasswordAsync(Guid id, string password);
        Task<Client> GetAsync(Guid id);
        Task<ClientSearchResponse> SearchAsync(ClientSearchFilter filter);
        Task<IEnumerable<Guid>> GetClientIdAllAsync();
    }
}
