using CommonLib.Infrastructure.Services;
using HomeWorkOTUS.Models.Technical;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface ITechnicalService : IService
    {
        Task<GenerateUsersResponse> GenerateUsersAsync(GenerateUsersRequest request);
    }
}
