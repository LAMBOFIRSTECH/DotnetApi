using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Controllers
{
	[Route("[controller]")]
	public class AccessTokenController : ControllerBase
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		private readonly IJwtTokenService jwtTokenService;

		public AccessTokenController(DailyTasksMigrationsContext dataBaseMemoryContext, IJwtTokenService jwtTokenService)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
			this.jwtTokenService = jwtTokenService;
		}
		[HttpPost]
		public async Task<IActionResult> Login(string email)
		{
			try
			{
				if (email is null)
				{
					return Conflict("Veuillez saisir une adresse mail valide");
				}
				var utilisateur = dataBaseMemoryContext.Utilisateurs.Where(u => u.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
				if (utilisateur is null)
				{
					return Conflict("L'adresse mail n'existe pas dans le contexte");
				}
				if (utilisateur.Role != 0)
				{
					return Unauthorized("Vous ne disposez pas des droits suffisant !");
				}
				await Task.Delay(500);
				return Ok(jwtTokenService.GenerateJwtToken(utilisateur.Email));
			}
			catch
			(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
	}
}