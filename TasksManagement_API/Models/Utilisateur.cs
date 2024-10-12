using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente un utilisateur dans le système.
/// </summary>

public class Utilisateur
{
	/// <summary>
	/// Représente l'identifiant unique d'un utilisateur.
	/// </summary>
	[Key]
	public int ID { get; set; }
	//public Guid UserId { get; set; } A revoir
	[Required]
	public string? Nom { get; set; }

	[Required]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public string Email { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public enum Privilege { Administrateur, Utilisateur }
	[EnumDataType(typeof(Privilege))]
	[Required]
	public Privilege Role { get; set; }

	[Required]
	[Category("Security")]
	//[System.Text.Json.Serialization.JsonIgnore] // set à disable le mot de passe dans la serialisation json
	public string? Pass { get; set; }
	public bool CheckHashPassword(string? password)
	{
		return BCrypt.Net.BCrypt.Verify(password, Pass);
	}
	public string SetHashPassword(string? password)
	{
		if (!string.IsNullOrEmpty(password))
		{
			Pass = BCrypt.Net.BCrypt.HashPassword($"{password}");
		}
		return Pass!;
	}
	public bool CheckEmailAdress(string? email)
	{
		string regexMatch = "(?<alpha>\\w+)@(?<mailing>[aA-zZ]+)\\.(?<domaine>[aA-zZ]+$)";
		if (string.IsNullOrEmpty(email))
		{
			return false;
		}
		Match check = Regex.Match(email, regexMatch);
		return check.Success;
	}
}