using Xunit;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.ServicesRepositories;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace TasksManagement_Tests.a_UnitTests
{
	public class UtilisateurServiceTest
	{

		private readonly DailyTasksMigrationsContext dbContext;
		private readonly UtilisateurService utilisateurService;


		public UtilisateurServiceTest()
		{
			// Configuration de la base de données en mémoire
			var options = new DbContextOptionsBuilder<DailyTasksMigrationsContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Très important quand on souhaite lancer plusieurs tests en meme temps Permet d'utiliser un nom de base de données unique à chaque exécution
				.Options;
			dbContext = new DailyTasksMigrationsContext(options);
			// Initialisation du service avec le contexte || dans le cas des mocks on doit initialiser le service avec les mocks
			utilisateurService = new UtilisateurService(
				dbContext,
				Mock.Of<IJwtTokenService>(),
				Mock.Of<IConfiguration>(),
				Mock.Of<IDataProtectionProvider>()
			);
			// Ajout de données de test à la base de données
			dbContext.Utilisateurs.AddRange(new List<Utilisateur>
		{
			new Utilisateur { ID = 1, Nom = "Alice", Email = "alice@example.com", Role = Utilisateur.Privilege.Administrateur },
			new Utilisateur { ID = 2, Nom = "Alice_1", Email = "alice@example.com", Role = Utilisateur.Privilege.Administrateur },
			new Utilisateur { ID = 3, Nom = "Bob", Email = "bob@example.com", Role = Utilisateur.Privilege.Utilisateur },
			new Utilisateur { ID = 4, Nom = "Charlie", Email = "charlie@example.com", Role = Utilisateur.Privilege.Administrateur },
		});
			dbContext.SaveChanges();
		}

		[Fact]
		public async Task GetUsers_ReturnsAllUsers_WhenNoFilterIsProvided_1()
		{
			// Act
			var result = await utilisateurService.GetUsers();
			// Assert
			Assert.Equal(4, result.Count);
		}
		[Theory]
		[InlineData("Alice")]
		[InlineData("Bob")]

		public async Task GetUsers_ReturnsSpecificUser_WhenFilterIsProvided_2(string Nom)
		{
			// Arrange
			Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur && u.Nom.Equals(Nom) || u.Role == Utilisateur.Privilege.Utilisateur && u.Nom.Equals(Nom));
			// Act
			var result = await utilisateurService.GetUsers(filter);
			// Assert
			Assert.Equal(1, result.Count);
		}
		[Theory]
		[InlineData("Alice", 2)]
		public async Task CheckExistedUser_ReturnsUserNewName_3(string Nom, int i)
		{
			// Arrange
			var expectedUsername = $"{Nom}_{i}";
			Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur && u.Nom.Equals(Nom) || u.Role == Utilisateur.Privilege.Utilisateur && u.Nom.Equals(Nom));
			// Act
			var utilisateurs = (await utilisateurService.GetUsers(filter)).ToList();
			var currentUsername = utilisateurService.CheckExistedUser(utilisateurs.First()!).Result;
			// Assert
			Assert.Equal(expectedUsername, currentUsername);
		}
		[Theory]
		[InlineData("Alice")]
		public async Task GetUsers_ReturnsSpecificUser_WhenFilterIsProvided_WithTasks_4(string Nom) // Peut redondant mettre en évidence le service Tache
		{
			// Arrange
			Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur && u.Nom.Equals(Nom) || u.Role == Utilisateur.Privilege.Utilisateur && u.Nom.Equals(Nom));
			// Act
			var result = await utilisateurService.GetUsers(filter);
			// Assert
			Assert.Equal(1, result.Count);
		}
		
	}

}