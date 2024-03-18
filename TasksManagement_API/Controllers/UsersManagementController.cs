using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using Tasks_WEB_API.SwaggerFilters;
using SQLitePCL;
namespace TasksManagement_API.Controllers;

[ApiController]
[Route("api/v1.0/[controller]/")]
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
	public async Task<ActionResult> CreateUser(int identifiant, string nom, [DataType(DataType.Password)] string mdp, string role, string email)
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
			var utilisateurExistant = listUtilisateurs.FirstOrDefault(item => item.Nom == nom && item.Role == privilege);
			if (utilisateurExistant != null)
			{
				return Conflict("Cet utilisateur est déjà présent");
			}

			var utilisateurAvecMemeNom = listUtilisateurs.FirstOrDefault(item => item.Nom == nom);
			if (utilisateurAvecMemeNom != null)
			{
				var nouveauNomUtilisateur = $"{nom}_1";
				if (listUtilisateurs.Any(item => item.Nom == nouveauNomUtilisateur))
				{
					return Conflict("Cet utilisateur possède déjà ce rôle");
				}
				newUtilisateur.Nom = nouveauNomUtilisateur;
			}

			await writeMethods.CreateUser(newUtilisateur);

			//string newUrl=await removeParametersInUrl.EraseParametersInUri();
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
	public async Task<ActionResult> DeleteUserById(int ID)
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
	/// Met à jour le mot de passe d'un utilisateur en fonction de son nom
	/// </summary>
	/// <param name="nom"></param>
	/// <param name="mdp"></param>
	/// <returns></returns>
	[HttpPatch("SetUserPassword")]
	public async Task<ActionResult> UpdateUserPassword(string nom, [DataType(DataType.Password)] string mdp)
	{
		try
		{
			await writeMethods.SetUserPassword(nom, mdp);
			return Ok($"Le mot de passe de l'utilisateur [{nom}] a bien été modifié.");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  ex.Message.Trim());
		}
	}
}