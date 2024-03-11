using HomeWorkOTUS.Extensions;
using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Token;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HomeWorkOTUS.Services
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(TokenClaims data)
        {
            var jwt = new JwtSecurityToken(
                issuer: "Issuer",
                claims: CommonFunc.GenerateClaimsList(data),
                notBefore: DateTime.Now,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(Convert.ToInt32(_configuration["RefreshToken:LifeTimeMinutes"]))),
                signingCredentials: new SigningCredentials(CommonFunc.GetSymmetricSecurityKey(_configuration["RefreshToken:SecretKey"]),
                    SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public TokenClaims ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = CommonFunc.GetSymmetricSecurityKey(_configuration["RefreshToken:SecretKey"]),
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidIssuer = "Issuer",
                ClockSkew = TimeSpan.FromSeconds(Convert.ToInt32(_configuration["RefreshToken:ClockSkewSeconds"]))
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return CommonFunc.GetRefreshTokenClaims(jwtToken.Claims);
        }
    }
}
