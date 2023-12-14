using System.Security.Claims;

namespace Server.Services
{
	public interface ITokenService
	{
		string GenerateAccessToken(IEnumerable<Claim> claims);
		string GenerateRefreshToken();
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
		public string GenerateLoginToken(string user, string role);
        public Claim[] GenerateClaims(string username, string role);
    }
}
