using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Options;
using TasksManagement_API.Models;

namespace TasksManagement_API.Authentifications
{
    public class AuthenticationCertification : AuthenticationHandler<CertificateAuthenticationOptions>
	{

		public AuthenticationCertification(IOptionsMonitor<CertificateAuthenticationOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock)
		: base(options, logger, encoder, clock)
		{

		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var cert = Context.Connection.ClientCertificate;
			Console.WriteLine(cert + "le certificat est ici");
			if (cert == null)
			{
				return Task.FromResult(AuthenticateResult.Fail("Le certificat client n'a pas été fourni"));
			}
			var events = Options.Events;
			try
			{
				// Vous pouvez utiliser le certificat pour effectuer l'authentification
				// Par exemple, vérifiez si le certificat est valide, s'il est autorisé, etc.
				if (!cert.Subject.Contains("CN=YourClientName"))  
				{
					return Task.FromResult(AuthenticateResult.Fail("Invalid client certificate."));
				}


				// Si l'authentification réussit, vous pouvez créer les claims pour l'utilisateur
				var claims = new List<Claim>{
					new Claim(ClaimTypes.NameIdentifier, cert.Subject),
					new Claim(ClaimTypes.Role, nameof(Utilisateur.Privilege.Admin))
			// Ajoutez d'autres claims selon vos besoins
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
	}
}