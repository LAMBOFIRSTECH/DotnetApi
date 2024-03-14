using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Components;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using TasksManagement_API.SwaggerFilters;

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
	[Required]
	public string? Nom { get; set; }
	
	[Required]
	[DataType(DataType.EmailAddress)]
	[EmailAddress]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public string Email { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public enum Privilege { Admin, UserX }
	[EnumDataType(typeof(Privilege))]
	[Required]
	public Privilege Role { get; set; }
	
	[Required]
	[System.Text.Json.Serialization.JsonIgnore] // set à disable le mot de passe dans la serialisation json
	[SwaggerSchema(Description = "Mot de passe de l'utilisateur", Format = "password")]
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
		return Pass;
	}
}