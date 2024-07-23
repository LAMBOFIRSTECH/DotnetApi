using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

			if (!readMethods.CheckUserSecret(secretUser) == true)
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