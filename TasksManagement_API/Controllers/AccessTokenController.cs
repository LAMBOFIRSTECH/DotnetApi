using System.ComponentModel.DataAnnotations;
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
				var result = await removeParametersInUrl.AccessToken(uriParams);
				if (!result)
				{
					return Conflict(result);// A revoir
				}
				if (!readMethods.CheckUserSecret(secretUser))
				{
					return Unauthorized("Mot de passe de clé secrète incorrect");
				}
				var token = await readMethods.GetToken(email);
				return Ok(token);

			}
			catch
			(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
	}
}