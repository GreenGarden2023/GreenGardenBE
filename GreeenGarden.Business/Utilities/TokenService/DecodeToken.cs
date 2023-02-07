using System;
using System.IdentityModel.Tokens.Jwt;

namespace GreeenGarden.Business.Utilities.TokenService
{
    public class DecodeToken
    {
        private JwtSecurityTokenHandler _tokenHandler;

        public DecodeToken()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public string Decode(string token, string nameClaim)
        {
            var claim = _tokenHandler.ReadJwtToken(token).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            if (claim != null)
            {
                return claim.Value;
            }
            return "Error!!!";
        }

    }
}

