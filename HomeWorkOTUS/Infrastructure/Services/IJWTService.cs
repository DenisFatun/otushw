using HomeWorkOTUS.Models.Token;

namespace HomeWorkOTUS.Infrastructure.Services
{
    public interface IJWTService : IService
    {
        string GetToken(TokenClaims data);
        TokenClaims ValidateToken(string token);
    }
}
