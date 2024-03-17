using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
				var utilisateur = dataBaseMemoryContext.Utilisateurs
				.Where(u => u.Email.ToUpper().Equals(email.ToUpper()) && u.Role.Equals(Utilisateur.Privilege.Admin))
				.SingleOrDefault();
				if (utilisateur is null)
				{
					return Conflict("Droits insuffisants ou adresse mail inexistante !");
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