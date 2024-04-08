using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TasksManagement_API.Interfaces;
namespace TasksManagement_API.Controllers
{
    [ApiController]
	[Route("api/v1.0/[controller]/")]
	public class AccessTokenController : ControllerBase
	{
		private readonly IReadUsersMethods readMethods;
		private readonly IWriteUsersMethods writeUsersMethods;
		private readonly IRemoveParametersIn removeParametersInUrl;
		public AccessTokenController(IReadUsersMethods readMethods, IRemoveParametersIn removeParametersInUrl, IWriteUsersMethods writeUsersMethods)
		{

			this.readMethods = readMethods;
			this.writeUsersMethods = writeUsersMethods;
			this.removeParametersInUrl = removeParametersInUrl;
			Log.Logger = new LoggerConfiguration()
			.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
			.CreateLogger();


		}
		/// <summary>
		/// Permet de générer un token JWt pour l'utilisateur Admin en fonction de son adresse mail
		/// </summary>
		/// <param name="email"></param>
		/// <param name="secretUser"></param>
		/// <returns></returns>
		[Authorize(Policy = "CertPolicy")]
		[HttpPost("Login")]
		public async Task<ActionResult> Login(string email, [DataType(DataType.Password)] string secretUser)
		{
			string regexMatch = "(?<alpha>\\w+)@(?<mailing>[aA-zZ]+)\\.(?<domaine>[aA-zZ]+$)";

			Match check = Regex.Match(email, regexMatch);
			if (secretUser is null)
			{
				return Conflict("Veuillez remplir tous les champs");
			}
			if (!check.Success)
			{
				return NotFound("Cette adresse mail est invalide");
			};

			if (!readMethods.CheckUserSecret(secretUser))
			{
				return Unauthorized("Mot de passe de clé secrète incorrect");
			}
			var uriParams = $"{email},{writeUsersMethods.EncryptUserSecret(secretUser)}";

			var newUrl = await removeParametersInUrl.AccessToken();
			if (newUrl.IsNullOrEmpty())
			{
				return BadRequest(newUrl);
			}

			try
			{
				using (var httpClient = new HttpClient())
				{
					var request = new HttpRequestMessage(HttpMethod.Post, newUrl);

					// Ajout du corps de la requête avec les paramètres requis
					request.Content = new StringContent(uriParams);

					var response = await httpClient.SendAsync(request);

					if (response.IsSuccessStatusCode)
					{
						var token = await readMethods.GetToken(email);
						return Ok(token);
					}
					else
					{
						Log.Error("Échec de la connexion SSL : {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
						return StatusCode((int)response.StatusCode, "La requête a échoué");
					}
				}
			}

			catch (Exception ex)
			{
				Log.Error(ex, "Une erreur s'est produite lors de la connexion");
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
	}
}