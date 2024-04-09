using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Options;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;

namespace TasksManagement_API.Authentifications
{
	public class AuthenticationCertification : AuthenticationHandler<CertificateAuthenticationOptions>, ICertificateValidationService
	{
		private readonly IConfiguration configuration;

		public AuthenticationCertification(IConfiguration configuration, IOptionsMonitor<CertificateAuthenticationOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock)
		: base(options, logger, encoder, clock)
		{
			this.configuration = configuration;
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var cert = Context.Connection.ClientCertificate;
			Console.WriteLine(cert + "le certificat est ici");//
			var checkValidCertificate = ValidateCertificate(cert);
			var events = Options.Events;
			try
			{

				// Vous pouvez utiliser le certificat pour effectuer l'authentification
				// Par exemple, vérifiez si le certificat est valide, s'il est autorisé, etc.

				// Si l'authentification réussit, vous pouvez créer les claims pour l'utilisateur
				var claims = new List<Claim>{
					new Claim(ClaimTypes.NameIdentifier, cert.Subject,ClaimValueTypes.String,ClaimsIssuer),
					new Claim(ClaimTypes.Name,cert.Subject,ClaimValueTypes.String,ClaimsIssuer),
					// new Claim(ClaimTypes.Role, nameof(Utilisateur.Privilege.Admin))
					};

				var identity = new ClaimsIdentity(claims, Scheme.Name);
				var principal = new ClaimsPrincipal(identity);
				var ticket = new AuthenticationTicket(principal, Scheme.Name);

				return Task.FromResult(AuthenticateResult.Success(ticket));

			}
			catch (Exception ex)
			{
				// Si une exception se produit pendant l'authentification, déclenchez l'événement OnAuthenticationFailed
				var failedContext = new CertificateAuthenticationFailedContext(Context, Scheme, Options)
				{
					Exception = ex
				};

				events!.OnAuthenticationFailed(failedContext);

				return Task.FromResult(AuthenticateResult.Fail($"Echec d'authentication : {ex.Message}"));
			}
		}

		public bool ValidateCertificate(X509Certificate2 clientCertificate)
		{
			if (clientCertificate == null)
			{
				Logger.LogError("Le certificat client n'a pas été fourni");
				return false;
			}

			if (!clientCertificate!.Subject.Contains("CN=YourClientName")) //check
			{
				Logger.LogError("Invalid client certificate.", clientCertificate.Subject);
				return false;
			}
			return true;
		}


	}
}