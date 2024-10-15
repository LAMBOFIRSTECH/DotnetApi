using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Controllers;
[ApiController]
[Route("api/v1.1/[controller]/")]

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
	//[Authorize(Policy = "UserPolicy")]
	[HttpGet("GetAllTasks/")]
	public async Task<IActionResult> GetAllTasks()
	{
		var taches = await readMethods.GetTaches();
		return Ok(taches);
	}

	/// <summary>
	/// Affiche les informations sur une tache précise.
	/// </summary>
	/// <param name="Titre"></param>
	/// <returns></returns>
	//[Authorize(Policy = "UserPolicy")]
	[HttpGet("GetTaskByTitle/{titre}")]
	public async Task<IActionResult> GetTaskByTitle(string Titre)
	{
		try
		{
			var tache = await readMethods.GetTaskByTitle(Titre);
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
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH= tache.EndDateH
			};
			var listTaches = await readMethods.GetTaches();
			var tacheExistante = listTaches.FirstOrDefault(item => item.Matricule == tache.Matricule);//GetByTitle au lieu de ceci 

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
	/// <param name="titre"></param>
	/// <returns></returns>
	//[Authorize(Policy = "AdminPolicy")]
	[HttpDelete("DeleteTask/{titre}")]
	public async Task<IActionResult> DeleteTaskById(string titre)
	{
		var tache = await readMethods.GetTaskByTitle(titre);
		try
		{
			if (tache == null)
			{
				return NotFound();
			}
			await writeMethods.DeleteTaskByTitle(titre);

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
	//[Authorize(Policy = "AdminPolicy")]
	[HttpPut("UpdateTask/")]
	public async Task<IActionResult> UpdateTask([FromBody] Tache tache)
	{
		try
		{
			var item = await readMethods.GetTaskByTitle(tache.Titre);
			if (item is null)
			{
				return NotFound();
			}
			if (item.Titre == tache.Titre)
			{
				await writeMethods.UpdateTask(tache);
			}
			return Ok($"Les infos de la tache [{item.Titre}] ont bien été modifiées avec succès.");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError,
					  ex.Message.Trim());
		}
	}

}
