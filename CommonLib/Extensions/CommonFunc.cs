using CommonLib.Models.Token;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CommonLib.Extensions
{
    public class CommonFunc
    {
        public static string HashSHA512(string data)
        {
            using var sha = SHA512.Create();
            var hash = string.Empty;
            var enc = new UTF8Encoding();
            byte[] dataBytes = enc.GetBytes(data);
            byte[] hashBytes = sha.ComputeHash(dataBytes);
            foreach (byte b in hashBytes)
            {
                hash += string.Format("{0:x2}", b);
            }
            return hash;
        }

        public static IEnumerable<Claim> GenerateClaimsList(object tokenClaims)
        {
            List<Claim> list = new List<Claim>();
            PropertyInfo[] properties = tokenClaims.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                string type = ("Name".Equals(propertyInfo.Name) ? "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" : propertyInfo.Name);
                list.Add(new Claim(type, (propertyInfo.GetValue(tokenClaims) == null) ? string.Empty : propertyInfo.GetValue(tokenClaims).ToString()));
            }

            return new ClaimsIdentity(list, "Token", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Claims;
        }

        public static TokenClaims GetRefreshTokenClaims(IEnumerable<Claim> tokenClaims)
        {
            TokenClaims claims = new TokenClaims();
            foreach (var claim in tokenClaims)
            {
                object value;
                var prop = claims.GetType().GetProperty(claim.Type);
                if (prop == null) continue;

                if (prop.PropertyType == typeof(DateTime?) && !string.IsNullOrEmpty(claim.Value))
                {
                    value = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                        .AddSeconds(Convert.ToInt64(claim.Value));
                }
                else if (prop.PropertyType == typeof(Guid) && !string.IsNullOrEmpty(claim.Value))
                {
                    value = Guid.Parse(claim.Value);
                }
                else
                {
                    value = Convert.ChangeType(claim.Value, prop.PropertyType);
                }

                prop.SetValue(claims, value);
            }
            return claims;
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        }

        public static string TokenFromHeader(string headerString)
        {
            return (!string.IsNullOrEmpty(headerString))
                ? headerString.Replace("Bearer ", "")
                : string.Empty;
        }
    }
}
