using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Controllers;
[ApiController]
[Route("api/v1.0/[controller]/")]

public class TasksManagementController : ControllerBase
{
	private readonly IReadTasksMethods readMethods;
	private readonly IWriteTasksMethods writeMethods;
	public TasksManagementController(IReadTasksMethods readMethods, IWriteTasksMethods writeMethods)
	{
		this.readMethods = readMethods;
		this.writeMethods = writeMethods;
	}

	/// <summary>
	/// Affiche la liste de toutes les taches.
	/// </summary>
	/// <returns></returns>
	[Authorize(Policy = "UserPolicy")]
	[HttpGet("GetAllTasks/")]
	public async Task<IActionResult> GetAllTasks()
	{
		var taches = await readMethods.GetTaches();
		return Ok(taches);
	}

	/// <summary>
	/// Affiche les informations sur une tache précise.
	/// </summary>
	/// <param name="Matricule"></param>
	/// <returns></returns>
	[Authorize(Policy = "UserPolicy")]
	[HttpGet("GetTaskByID/{Matricule:int}")]
	public async Task<IActionResult> SelectTask(int Matricule)
	{
		try
		{
			var tache = await readMethods.GetTaskById(Matricule);
			if (tache != null)
			{
				return Ok(tache);
			}
			return NotFound("tache non trouvée.");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}

	/// <summary>
	/// Crée une tache. 
	/// </summary>
	/// <param name="tache"></param>
	/// <returns></returns>
	[HttpPost("CreateTask/")]
	public async Task<IActionResult> CreateTask([FromBody] Tache tache)
	{
		try
		{
			Tache newTache = new()
			{
				Matricule = tache.Matricule,
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH= tache.EndDateH
			};
			var listTaches = await readMethods.GetTaches();
			var tacheExistante = listTaches.FirstOrDefault(item => item.Matricule == tache.Matricule);

			if (tacheExistante != null)
			{
				return Conflict("Cette tache est déjà présente");
			}

			await writeMethods.CreateTask(newTache);
			return Ok("La ressource a bien été créée");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}

	/// <summary>
	/// Supprime une tache en fonction de son matricule.
	/// </summary>
	/// <param name="Matricule"></param>
	/// <returns></returns>
	[Authorize(Policy = "AdminPolicy")]
	[HttpDelete("DeleteTask/{Matricule:int}")]
	public async Task<IActionResult> DeleteTaskById(int Matricule)
	{
		var tache = await readMethods.GetTaskById(Matricule);
		try
		{
			if (tache == null)
			{
				return NotFound($"La tache de matricule : matricule=[{Matricule}] n'existe plus dans le contexte de base de données");
			}
			await writeMethods.DeleteTaskById(Matricule);

			return Ok("La donnée a bien été supprimée");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  ex.Message.Trim());
		}
	}

	/// <summary>
	/// Met à jour les informations d'une tache.
	/// </summary>
	/// <param name="tache"></param>
	/// <returns></returns>
	[Authorize(Policy = "AdminPolicy")]
	[HttpPut("UpdateTask/")]
	public async Task<IActionResult> UpdateTask([FromBody] Tache tache)
	{
		try
		{
			var item = await readMethods.GetTaskById(tache.Matricule);
			if (item is null)
			{
				return NotFound($"Cette tache n'existe plus dans le contexte de base de données");
			}
			if (item.Matricule == tache.Matricule)
			{
				await writeMethods.UpdateTask(tache);
			}
			return Ok($"Les infos de la tache [{item.Matricule}] ont bien été modifiées avec succès.");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  ex.Message.Trim());
		}
	}

}
