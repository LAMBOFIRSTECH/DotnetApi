using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
namespace TasksManagement_API.Controllers;

[ApiController]
[Route("api/v1.1/")]
[Produces("application/json")]
public class UsersManagementController : ControllerBase
{
	private readonly IReadUsersMethods readMethods;
	private readonly IWriteUsersMethods writeMethods;

	public UsersManagementController(IReadUsersMethods readMethods, IWriteUsersMethods writeMethods)
	{
		this.readMethods = readMethods; this.writeMethods = writeMethods;
	}

	/// <summary>
	/// Affiche la liste de tous les utilisateurs.
	/// </summary>
	//[Authorize(Policy = "AdminPolicy")]
	[HttpGet("users")]
	public async Task<ActionResult> GetUsers()
	{
		var users = await readMethods.GetUsers();
		if (users.Any()) { return Ok(users); }
		return NoContent();
	}

	/// <summary>
	/// Affiche les informations sur un utilisateur en fonction de son ID.
	/// </summary>
	/// <param name="Nom"></param>
	/// <param name="Role"></param>
	/// <returns></returns>
	//[Authorize(Policy = "UserPolicy")]
	[HttpGet("SingleUser/{Nom}/{Role}")]
	public async Task<ActionResult> GetSingleUser(string Nom, string Role)
	{
		if (Enum.TryParse(char.ToUpper(Role[0]) + Role.Substring(1).ToLower(), out Utilisateur.Privilege result))
		{
			try
			{
				var utilisateur = await readMethods.GetSingleUserByNameRole(Nom, result);
				if (utilisateur != null)
				{
					return Ok(utilisateur);
				}
				return NotFound("Utilisateur non trouvé.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
		else
		{
			return BadRequest("Le rôle spécifié n'est pas valide.");
		}
	}
	/// <summary>
	/// Créée un utilisateur.
	/// </summary>
	/// <remarks>
	/// Sample request:
	///
	///     POST /CreateUser
	///     {
	///        "id": This value is autoincremented,
	///        "Nom": "username",
	///        "mdp": "password",
	///        "role": "Utilisateur",
	///        "email": "adress_name@mailing_server.domain"  
	///     }
	/// </remarks>

	[HttpPost("user")]
	public async Task<ActionResult> CreateUser([FromBody] Utilisateur utilisateur)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		try
		{
			if (!Enum.IsDefined(typeof(Utilisateur.Privilege), utilisateur.Role))
			{
				return BadRequest("Le rôle spécifié n'est pas valide.");
			}
			var nouveauNomUtilisateur = await readMethods.CheckExistedUser(utilisateur);
			Utilisateur newUtilisateur = new()
			{
				Nom = nouveauNomUtilisateur!,
				Pass = utilisateur.Pass,
				Role = utilisateur.Role,
				Email = utilisateur.Email
			};
			await writeMethods.CreateUser(newUtilisateur);
			return CreatedAtAction(nameof(GetUsers), new { Nom = newUtilisateur.Nom,Email=newUtilisateur.Email }, newUtilisateur);
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}
	/// <summary>
	/// Supprime un utilisateur en fonction de son ID.
	/// </summary>
	/// <param name="Nom"></param>
	/// <param name="Role"></param>
	/// <returns></returns>
	//[Authorize(Policy = "AdminPolicy")]
	[HttpDelete("user/{Nom}/{Role}")]
	public async Task<ActionResult> DeleteUserByDetails(string Nom, string Role)
	{
		if (Enum.TryParse(char.ToUpper(Role[0]) + Role.Substring(1).ToLower(), out Utilisateur.Privilege result))
		{
			try
			{
				var utilisateur = await readMethods.GetSingleUserByNameRole(Nom, result);
				if (utilisateur == null)
				{
					return NotFound($"L'utilisateur [{Nom}] n'a pas été trouvé dans le contexte de base de données");
				}
				await writeMethods.DeleteUserByDetails(Nom, result);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
			}
		}
		else
		{
			return BadRequest("Le rôle spécifié n'est pas valide.");
		}
	}
	/// <summary>
	/// Met à jour le mot de passe d'un utilisateur en fonction de son nom.
	/// </summary>
	/// <param name="nom"></param>
	/// <param name="currentpassword"></param>
	/// <param name="newpassword"></param>
	/// <returns></returns>
	[HttpPatch("user/{nom}/{currentpassword}/{newpassword}")]
	public async Task<ActionResult> UpdateUserPassword(string nom, [DataType(DataType.Password)] string currentpassword, [DataType(DataType.Password)] string newpassword)
	{
		try
		{
			if (currentpassword == newpassword)
			{
				return Conflict("Le mot de passe saisi existe déjà.");
			}
			await writeMethods.SetUserPassword(nom, newpassword);
			return NoContent();
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}
}