using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Interfaces;
namespace TasksManagement_API.Controllers
{
    [ApiController]
	[Route("api/v1.0/[controller]/")]
	public class AccessTokenController : ControllerBase
	{
		private readonly IReadUsersMethods readMethods;
		
		public AccessTokenController(IReadUsersMethods readMethods)
		{
			this.readMethods = readMethods;
		}
		/// <summary>
		/// Permet de générer un token JWt pour l'utilisateur Admin en fonction de son adresse mail
		/// </summary>
		/// <param name="email"></param>
		/// <param name="secretUser"></param>
		/// <returns></returns>
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

			if (readMethods.CheckUserSecret(secretUser) == true)
			{
				return Unauthorized("Votre clé secrète incorrect");
			}
			var result = await readMethods.GetToken(email);
			if (!result.Success)
			{
				return Unauthorized(new { result.Message });
			}
			return Ok(new { result.Token });
			
		}
	}
}