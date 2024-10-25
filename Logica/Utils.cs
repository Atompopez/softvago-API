using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using softvago_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace softvago_API.Logica
{
    public class Utils
    {
        private IConfiguration _configuration = new ConfigurationBuilder()
                                                .AddJsonFile("appsettings.json")
                                                .Build();

        public string HashGenerator(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GenerateJwtToken(Login login)
        {
            var jwt = _configuration.GetSection("Jwt").Get<JWT>();

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("Username", login.username),
                    new Claim("Username", login.password)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: singIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}