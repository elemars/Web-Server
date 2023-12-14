using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Services
{
    public class TokenService : ITokenService
    {
		public string GenerateAccessToken(IEnumerable<Claim> claims)
		{
			var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecretKey));
			var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
			var tokeOptions = new JwtSecurityToken(
				issuer: Program.Domain1,
				audience: Program.Domain1,
				claims: claims,
				expires: DateTime.Now.AddDays(5),
				signingCredentials: signinCredentials
			);
			var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
			return tokenString;
		}
		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}
		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecretKey)),
				ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");
			return principal;
		}


		public Claim[] GenerateClaims(string username, string role)
        {
            return new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim("role", role),
        // Fügen Sie hier weitere Ansprüche hinzu, wie z.B. die Beschreibung der Datenbank
            };
        }

        public string GenerateLoginToken(string user, string role)
        {
            var claims = GenerateClaims(user, role);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Program.Domain1,
                audience: Program.Domain1,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateUserToken(string user)
        {
            var claims = GenerateClaims(user, "User");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Program.Domain1,
                audience: Program.Domain2,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
