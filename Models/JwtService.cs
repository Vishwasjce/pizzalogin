using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Login.Models
{
    public class JwtService
    {
        public string Secretkey { get; set; }
        public int TokenDuration { get; set; } // Keep this as an int
        private readonly IConfiguration config;

        public JwtService(IConfiguration _config)
        {
            config = _config;
            this.Secretkey = config["Jwt:Key"]; // Correctly access the key
            this.TokenDuration = int.Parse(config["Jwt:Duration"]); // Parse the duration as int
        }

        public string GenerateToken(Guid UserID, string firstName, string username, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Secretkey));
            var signature = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var payload = new[]
            {
                new Claim("id",UserID.ToString()),
                new Claim("firstname", firstName),
                new Claim("email", email),
                new Claim("username", username)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"], // Use configuration directly
                audience: config["Jwt:Audience"], // Use configuration directly
                claims: payload,
                expires: DateTime.UtcNow.AddMinutes(TokenDuration), // Ensure TokenDuration is used correctly
                signingCredentials: signature // Provide signing credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken); // Return the generated token
        }
    }
}
