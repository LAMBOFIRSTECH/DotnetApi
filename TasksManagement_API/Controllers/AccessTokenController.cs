using System.ComponentModel.DataAnnotations;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Tasks_WEB_API.SwaggerFilters;
using TasksManagement_API.Interfaces;
namespace TasksManagement_API.Controllers
{
	[ApiController]
	[Route("api/v1.0/[controller]/")]
	public class AccessTokenController : ControllerBase
	{
		private readonly IReadUsersMethods readMethods;
		private readonly IRemoveParametersIn removeParametersInUrl;
		public AccessTokenController(IReadUsersMethods readMethods, IRemoveParametersIn removeParametersInUrl)
		{

			this.readMethods = readMethods;
			this.removeParametersInUrl = removeParametersInUrl;


		}
		/// <summary>
		/// Permet de générer un token JWt pour l'utilisateur Admin en fonction de son adresse mail
		/// </summary>
		/// <param name="email"></param>
		/// <param name="secretUser"></param>
		/// <returns></returns>
		[HttpPost("Login")]
		public async Task<ActionResult> Login([DataType(DataType.EmailAddress)] string email, [DataType(DataType.Password)] string secretUser)
		{
			try
			{
				if (email is null || secretUser is null)
				{
					return Conflict("Veuillez remplir tous les champs");
				}

				var uriParams = new List<string>() { $"{email},{secretUser}" };
				var newUrl = await removeParametersInUrl.AccessToken();
				if (newUrl.IsNullOrEmpty())
				{
					return Conflict(newUrl);// A revoir
				}
				if (!readMethods.CheckUserSecret(secretUser))
				{
					return Unauthorized("Mot de passe de clé secrète incorrect");
				}
				var token = await readMethods.GetToken(email);
				return Ok(token);

				// using (var httpClient = new HttpClient())
				// {
				// 	// Créez les données à envoyer dans le corps de la requête
				// 	var requestData = new List<KeyValuePair<string?, string?>>()
				// 	{
				// 		new KeyValuePair<string?, string?>("email", email),
				// 		new KeyValuePair<string?, string?>("secretUser", secretUser)
				// 	};

				// 	// Créez la requête POST avec les données
				// 	var response = await httpClient.PostAsync(newUrl, new FormUrlEncodedContent(requestData));

				// 	// Vérifiez si la requête a réussi
				// 	if (response.IsSuccessStatusCode)
				// 	{
				// 		// Traitez la réponse réussie si nécessaire
				// 		var token = await response.Content.ReadAsStringAsync();
				// 		return Ok(token);
				// 	}
				// 	else
				// 	{
				// 		// Gérez les erreurs de la requête
				// 		return Conflict($"Erreur lors de l'obtention du jeton : {response.StatusCode}");
				// 	}
				// }


			}
			catch
			(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
	}
}