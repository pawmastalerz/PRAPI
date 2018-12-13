using System.IdentityModel.Tokens.Jwt;

namespace PRAPI.Services
{
    public class TokenService : ITokenService
    {
        public bool CheckIfSameUser(string token, int id)
        {
            var noBearerToken = token.Remove(0, 7);
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(noBearerToken);
            if (decodedToken.Payload["unique_name"].ToString() == id.ToString())
                return true;
            return false;
        }
    }
}