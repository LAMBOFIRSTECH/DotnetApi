using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks_WEB_API.Interfaces;
namespace Tasks_WEB_API.Controllers;

[ApiController]
//[Area("TasksDocumentation")]
[Route("api/v1.0/")]

public class TasksManagementController : ControllerBase
{
	private readonly IReadTasksMethods readMethods;
	private readonly IWriteTasksMethods writeMethods;
	public TasksManagementController(IReadTasksMethods readMethods,IWriteTasksMethods writeMethods)
	{
		this.readMethods = readMethods;
		this.writeMethods = writeMethods;
	}

	/// <summary>
	/// Affiche la liste de toutes les taches.
	/// </summary>
	/// <returns></returns>
	[Authorize(Policy = "UserPolicy")]
	[HttpGet("~/GetAllTasks")]
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
	[HttpGet("~/GetTaskByID/{Matricule:int}")]
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
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur s'est produite dans la recherche de la ressource {Matricule}.");
		}
	}

	/// <summary>
	/// Crée une tache. 
	/// </summary>
	/// <param name="tache"></param>
	/// <returns></returns>
	[HttpPost("~/CreateTask")]
	public async Task<IActionResult> CreateTask([FromBody] Tache tache)
	{
		try
		{
			Tache newTache = new()
			{
				Matricule = tache.Matricule,
				Titre = tache.Titre,
				Summary = tache.Summary,
				TasksDate = new()
				{
					EndDateH = tache.TasksDate.EndDateH,
					StartDateH = tache.TasksDate.StartDateH
				}
			};
			var listTaches = await readMethods.GetTaches();
			foreach (var item in listTaches)
			{
				if (item.Matricule == tache.Matricule)
				{

					return Conflict("Cette tache est déjà présente");
				}
			}
			await writeMethods.CreateTask(newTache);
			return Ok("La ressource a bien été créée");
		}
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, "Erreur lors de la création de la ressource tache.");
		}
	}

	/// <summary>
	/// Supprime une tache en fonction de son matricule.
	/// </summary>
	/// <param name="Matricule"></param>
	/// <returns></returns>
	[Authorize(Policy = "AdminPolicy")]
	[HttpDelete("~/DeleteTask/{Matricule:int}")]
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
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  "Error deleting data");
		}
	}

	/// <summary>
	/// Met à jour les informations d'une tache.
	/// </summary>
	/// <param name="tache"></param>
	/// <returns></returns>
	[Authorize(Policy = "AdminPolicy")]
	[HttpPut("~/UpdateTask")]
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
		catch (Exception)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  "Error lors de la suppression de la ressource");
		}
	}
}
