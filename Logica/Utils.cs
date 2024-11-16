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

        private DataQuery _dataQuery = new DataQuery();

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

        public string GenerateJwtToken(Login login, int IdRol)
        {
            var jwt = _configuration.GetSection("Jwt").Get<JWT>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("Username", login.username),
                new Claim("Password", login.password),
                new Claim("IdRol", IdRol.ToString())
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

        public async Task<bool> HasRole(int idRol, string rol)
        {
            var roles = await _dataQuery.GetRoles();
            var rolName = roles.FirstOrDefault(r => r.id == idRol)?.name;

            return rolName == rol;
        }
    }
}