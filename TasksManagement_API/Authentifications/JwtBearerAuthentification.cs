using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace TasksManagement_API.Authentifications
{
	public class JwtBearerAuthentification : AuthenticationHandler<JwtBearerOptions>
	{
		public JwtBearerAuthentification(IOptionsMonitor<JwtBearerOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock)
		: base(options, logger, encoder, clock)
		{
			
		}
		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
				return Task.FromResult(AuthenticateResult.Fail("Authorization header missing"));
			try
			{
				var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
				if (!authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
				{
					return Task.FromResult(AuthenticateResult.Fail("Schéma d'authentification invalide"));
				}
				// Récupérer le jeton JWT à partir de l'en-tête d'autorisation
				var jwtToken = authHeader.Parameter;
				var tokenValidationParameters = Options.TokenValidationParameters;
				
				if (tokenValidationParameters == null)
					return Task.FromResult(AuthenticateResult.Fail("Les paramètres du token de validation ne sont pas configurés"));

				var tokenHandler = new JwtSecurityTokenHandler();
				SecurityToken securityToken;
				var principal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out securityToken);

				// Créer un ticket d'authentification réussi avec le principal
				var ticket = new AuthenticationTicket(principal, Scheme.Name);
				return Task.FromResult(AuthenticateResult.Success(ticket));
			}
			catch (Exception ex)
			{
				return Task.FromResult(AuthenticateResult.Fail($"Echec d'authentication : {ex.Message}"));
			}
		}
	}
}
