using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Controllers;
[ApiController]
[Route("api/v1.1/")]

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
	[HttpGet("singleOrAllTasks/")]
	public async Task<IActionResult> GetSingleOrAllTasks([FromQuery] string? Titre)
	{
		try
		{
			var taches = await readMethods.GetTaches();
			if (!string.IsNullOrEmpty(Titre))
			{
				var tache = taches.Where(t => t.Titre.Equals(Titre, StringComparison.OrdinalIgnoreCase));
				if (!tache.Any())
				{
					return NotFound();

				}
				return Ok(tache.FirstOrDefault());
			}
			return Ok(taches);
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
	[HttpPost("tache/")]
	public async Task<IActionResult> CreateTask([FromBody] Tache tache)
	{
		if (tache.NomUtilisateur is null || tache.EmailUtilisateur is null)
		{
			return BadRequest("Le nom ou l'email de l'utilisateur est requis pour créer une tâche.");
		}
		try
		{
			var Taches = await readMethods.GetTaches(query => query.Where(t => t.Titre.Equals(tache.Titre) && t.utilisateur!.ID.Equals(tache.UserId)));
			var tacheExistante = Taches.FirstOrDefault();
			if (tache.StartDateH.Date >= tache.EndDateH.Date)
			{
				var message = "Exemple : Date de debut ->  01/01/2024  (doit etre '>' Supérieur) Date de fin -> 02/02/2024";
				return StatusCode(StatusCodes.Status406NotAcceptable, message);
			}
			if (tacheExistante != null)
			{
				return Conflict("Cette tache est déjà présente");
			}
			Tache newTache = new()
			{
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH = tache.EndDateH,
				NomUtilisateur = tache.NomUtilisateur,
				EmailUtilisateur = tache.EmailUtilisateur
			};
			await writeMethods.CreateTask(newTache);
			return CreatedAtAction(nameof(GetSingleOrAllTasks), new { Titre = newTache.Titre }, newTache);
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
	[HttpDelete("tache/{titre}")]
	public async Task<IActionResult> DeleteTaskById(string titre)
	{
		var tache = await GetSingleOrAllTasks(titre);
		try
		{
			if (tache == null)
			{
				return NotFound();
			}
			await writeMethods.DeleteTaskByTitle(titre);

			return NoContent();
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
	/// <param name="username"></param>
	/// <param name="tache"></param>
	/// <returns></returns>
	//[Authorize(Policy = "AdminPolicy")]
	[HttpPut("tache/{titre}")]
	public async Task<IActionResult> UpdateTask(string username, [FromBody] Tache tache)
	{
		try
		{
			if (username != tache.NomUtilisateur)
			{
				return NotFound();
			}
			if (tache.StartDateH.Date >= tache.EndDateH.Date)
			{
				var message = "Exemple : Date de debut ->  01/01/2024  (doit etre '>' Supérieur) Date de fin -> 02/02/2024";
				return StatusCode(StatusCodes.Status406NotAcceptable, message);
			}
			await writeMethods.UpdateTask(username, tache);
			return NoContent();
		}
		catch (DbUpdateConcurrencyException ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}
}
