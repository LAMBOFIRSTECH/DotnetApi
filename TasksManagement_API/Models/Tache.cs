using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente une tache dans le système
/// </summary>
public class Tache
{
	/// <summary>
	/// Représente l'identifiant unique d'une tache.
	/// </summary>
	[Key]
	public int Matricule { get; set; }
	[Required]
	public string Titre { get; set; } = string.Empty;
	public string Summary { get; set; } = string.Empty;
	[Required(ErrorMessage = "Le format de date doit être comme l'exemple suivant : 01/01/2024")]
	[DataType(DataType.Date)]
	public DateTime StartDateH { get; set; }

	[Required(ErrorMessage = "Le format de date doit être comme l'exemple suivant : 01/01/2024")]
	[DataType(DataType.Date)]
	public DateTime EndDateH { get; set; }
	public int UserId { get; set; }
	public Utilisateur? utilisateur { get; set; }
}
