using Xunit;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.ServicesRepositories;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace TasksManagement_Tests
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
			new Utilisateur { ID = 2, Nom = "Bob", Email = "bob@example.com", Role = Utilisateur.Privilege.Utilisateur },
			new Utilisateur { ID = 3, Nom = "Charlie", Email = "charlie@example.com", Role = Utilisateur.Privilege.Administrateur },
		});

			dbContext.SaveChanges();

		}

		[Fact]
		public async Task GetUsers_ReturnsAllUsers_WhenNoFilterIsProvided_1()
		{
			// Act
			var result = await utilisateurService.GetUsers();
			// Assert
			Assert.Equal(3, result.Count);
		}

		[Fact]
		public async Task GetUsers_ReturnsAllUsers_WhenFilterIsProvided_2()
		{
			Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur);
			// Act
			var result = await utilisateurService.GetUsers(filter);
			// Assert
			Assert.Equal(2, result.Count);

		}
	}

}