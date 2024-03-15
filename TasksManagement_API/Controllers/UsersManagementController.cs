using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
namespace TasksManagement_API.Controllers;

[ApiController]
[Route("api/v1.0/[controller]/")]
[Produces("application/json")]
public class UsersManagementController : ControllerBase
{
	private readonly IReadUsersMethods readMethods;
	private readonly IWriteUsersMethods writeMethods;
	public UsersManagementController(IReadUsersMethods readMethods, IWriteUsersMethods writeMethods) { this.readMethods = readMethods; this.writeMethods = writeMethods; }

	/// <summary>
	/// Affiche la liste de tous les utilisateurs.
	/// </summary>
	[Authorize(Policy = "AdminPolicy")]
	[HttpGet("GetAllUsers")]
	public async Task<ActionResult> GetUsers()
	{
		return Ok(await readMethods.GetUsers());
	}

	/// <summary>
	/// Affiche les informations sur un utilisateur en fonction de son ID.
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	[Authorize(Policy = "UserPolicy")]
	[HttpGet("GetUserByID/{ID:int}")]
	public async Task<ActionResult> GetUserById(int ID)
	{
		try
		{
			var utilisateur = await readMethods.GetUserById(ID);
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

	/// <summary>
	/// Créée un utilisateur.
	/// </summary>
	/// <param name="identifiant"></param>
	/// <param name="nom"></param>
	/// <param name="mdp"></param>
	/// <param name="role"></param>
	/// <param name="email"></param>
	/// <returns></returns>
	/// <remarks>
	/// Sample request:
	///
	///     POST /CreateUser
	///     {
	///        "id": This value is autoincremented,
	///        "Nom": "username",
	///        "mdp": "password",
	///        "role": "UserX",
	///        "email": "adress_name@mailing_server.domain"  
	///     }
	/// </remarks>

	[HttpPost("CreateUser/")]
	public async Task<IActionResult> CreateUser(int identifiant, string nom, [DataType(DataType.Password)] string mdp, string role, string email)
	{
		try
		{
			Utilisateur.Privilege privilege;
			if (!Enum.TryParse(role, true, out privilege))
			{
				return BadRequest("Le rôle spécifié n'est pas valide.");
			}
			Utilisateur newUtilisateur = new()
			{
				ID = identifiant,
				Nom = nom,
				Pass = mdp,
				Role = privilege,
				Email = email
			};
			var listUtilisateurs = await readMethods.GetUsers();
			foreach (var item in listUtilisateurs)
			{
				if (item.Nom == nom && item.Role == privilege)
				{
					return Conflict("Cet utilisateur est déjà présent");
				}
			}
			await writeMethods.CreateUser(newUtilisateur);
			return Ok("La ressource a bien été créée");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}
	/// <summary>
	/// Supprime un utilisateur en fonction de son ID.
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	[Authorize(Policy = "AdminPolicy")]
	[HttpDelete("DeleteUser/{ID:int}")]
	public async Task<IActionResult> DeleteUserById(int ID)
	{
		var utilisateur = await readMethods.GetUserById(ID);
		try
		{
			if (utilisateur == null)
			{
				return NotFound($"L'utilisateur id=[{ID}] n'a pas été trouvé dans le contexte de base de données");
			}

			await writeMethods.DeleteUserById(ID);
			return Ok("La donnée a bien été supprimée");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}

	/// <summary>
	/// Met à jour les informations d'un utilisateur.
	/// </summary>
	/// <param name="utilisateur"></param>
	/// <returns></returns>
	//[Authorize(Policy = "AdminPolicy")]
	[HttpPut("UpdateUser")]
	public async Task<IActionResult> UpdateUser([FromBody] Utilisateur utilisateur)
	{

		if (utilisateur.ID <= 0)
		{
			throw new InvalidOperationException("L'identifiant ne peut pas etre inferieur ou égale à zero.");
		}
		try
		{
			var item = await readMethods.GetUserById(utilisateur.ID);
			if (item is null)
			{
				return NotFound($"Cet utilisateur n'existe plus dans le contexte de base de données");
			}
			await (item.ID == utilisateur.ID ? writeMethods.UpdateUser(utilisateur) : Task.CompletedTask);
			return Ok($"Les infos de l'utilisateur [{item.ID}] ont bien été modifiées.");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  ex.Message.Trim());
		}
	}
	
	/// <summary>
	/// Met à jour une partie des informations d'un utilisateur.
	/// </summary>
	/// <param name="id"></param>
	/// <param name="nom"></param>
	/// <param name="mdp"></param>
	/// <param name="role"></param>
	/// <param name="email"></param>
	/// <returns></returns>
	[HttpPatch("PatchUser")]
	public async Task<IActionResult> PartialUpdateUser(int id, string nom, [DataType(DataType.Password)] string mdp, string role, string email)
	{
		try
		{
			var item = await readMethods.GetUserById(id);
			if (item is null)
			{
				return NotFound($"Cet utilisateur n'existe plus dans le contexte de base de données");
			}
			
			Utilisateur.Privilege privilege;
			if (!Enum.TryParse(role, true, out privilege))
			{
				return BadRequest("Le rôle spécifié n'est pas valide.");
			}
			await (item.ID == id ? writeMethods.PartialUpdateUser(id,nom,mdp,role,email) : Task.CompletedTask);
			return Ok($"Les infos de l'utilisateur [{item.ID}] ont bien été modifiées.");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  ex.Message.Trim());
		}
	}
}