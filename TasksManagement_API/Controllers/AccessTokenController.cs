using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Controllers
{
	[ApiController]
	[Route("api/v1.0/[controller]/")]
	public class AccessTokenController : ControllerBase
	{
		private readonly IWriteUsersMethods writeMethods;
		public AccessTokenController(IWriteUsersMethods writeMethods)
		{

			this.writeMethods = writeMethods;
		}
		/// <summary>
		/// Permet de générer un token JWt pour l'utilisateur Admin en fonction de son adresse mail
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		[HttpPost("Login")]
		public async Task<ActionResult> Login(string email)
		{
			try
			{
				if (email is null)
				{
					return Conflict("Veuillez saisir une adresse mail valide");
				}
				var token = await writeMethods.GetToken(email);
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