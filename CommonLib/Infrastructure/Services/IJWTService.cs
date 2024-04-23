using CommonLib.Models.Token;

namespace CommonLib.Infrastructure.Services
{
    public interface IJWTService : IService
    {
        string GetToken(TokenClaims data);
        TokenClaims ValidateToken(string token);
    }
}
