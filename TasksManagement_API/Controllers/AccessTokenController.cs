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
		private readonly IWriteUsersMethods writeMethods;
		private readonly IRemoveParametersIn removeParametersInUrl;
		public AccessTokenController(IWriteUsersMethods writeMethods, IRemoveParametersIn removeParametersInUrl)
		{

			this.writeMethods = writeMethods;
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
				await removeParametersInUrl.AccessToken(uriParams);
				if (writeMethods.CheckUserSecret(secretUser))
				{
					var token = await writeMethods.GetToken(email);
					return Ok(token);
				}
				else
				{
					return Unauthorized("Mot de passe de clé secrète incorrect");
				}
			}
			catch
			(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
	}
}