using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlineShoppingPlatformApp.WebApi.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace OnlineShoppingPlatformApp.WebApi.Jwt
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(JwtDto jwtDto)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtDto.SecretKey));

            //credentials oluşturma -> kimlik doğrulama için gerekli olan bilgileri tutar.
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            //claims
            var claims = new List<Claim>
            {
                new Claim(JwtCustomClaimNames.Id, jwtDto.Id),
                new Claim(JwtCustomClaimNames.Email, jwtDto.Email),
                new Claim(JwtCustomClaimNames.FirstName, jwtDto.FirstName),
                new Claim(JwtCustomClaimNames.LastName, jwtDto.LastName),
                new Claim(JwtCustomClaimNames.Role, jwtDto.Role.ToString()),

                // authentication ve authorization için gerekli olan claim
                new Claim(ClaimTypes.Role, jwtDto.Role.ToString())
            };

            var expiration = DateTime.Now.AddMinutes(jwtDto.ExpireMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtDto.Issuer,
                audience: jwtDto.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: signinCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor);

            return token;
        }
    }
}