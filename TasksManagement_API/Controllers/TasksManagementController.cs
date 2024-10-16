using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using TasksManagement_API.ServicesRepositories;
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
	[HttpGet("SingleOrAllTasks/")]
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
		string Titre=tache.Titre;
		try
		{
			Tache newTache = new()
			{
				Titre = tache.Titre,
				Summary = tache.Summary,
				StartDateH = tache.StartDateH,
				EndDateH = tache.EndDateH
			};
			var Taches = await readMethods.GetTaches(query => query.Where(t => t.Titre.Equals(tache.Titre)));
			var tacheExistante = Taches.FirstOrDefault();
			if (tache.StartDateH.Date >= tache.EndDateH.Date)
			{
				var message = "Exemple : Date de debut ->  01/01/2024  (doit etre '>' Supérieur) Date de fin -> 02/02/2024";
				throw new ArgumentException("Date Error",StatusCodes.Status406NotAcceptable.ToString(message));
			}
			if (tacheExistante != null)
			{
				return Conflict("Cette tache est déjà présente");
			}
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
	/// <param name="matricule"></param>
	/// <param name="tache"></param>
	/// <returns></returns>
	//[Authorize(Policy = "AdminPolicy")]
	[HttpPut("tache/{matricule}")]
	public async Task<IActionResult> UpdateTask(int matricule, [FromBody] Tache tache)
	{
		try
		{
			if (matricule <= 0)
			{
				return BadRequest("Le matricule doit etre strictement positif");
			}
			if (matricule != tache.Matricule)
			{
				return NotFound();
			}
			if (tache.StartDateH.Date >= tache.EndDateH.Date)
			{
				var message = "Exemple : Date de debut ->  01/01/2024  (doit etre '>' Supérieur) Date de fin -> 02/02/2024";
				throw new ArgumentException("Date Error",StatusCodes.Status406NotAcceptable.ToString(message));
			}
			await writeMethods.UpdateTask(matricule, tache);
			return  NoContent();
		}
		catch (DbUpdateConcurrencyException ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
		}
	}

	// NoContent(204) Vs OK(200) Vs NotFound(404)
	/*
		- 204: operation réussie mais pas de contenu à renvoyer
		- 200 : opération réussie avec contenu à renvoyer
		- 404 : opération échouée on arrive pas à retrouver la ressource demandée
	
	*/
}
