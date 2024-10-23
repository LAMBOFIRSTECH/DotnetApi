using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		private readonly Mock<DailyTasksMigrationsContext> mockDbContext;
		private readonly Mock<IJwtTokenService> mockJwtTokenService;
		private readonly Mock<IConfiguration> mockConfig;
		private readonly Mock<IDataProtectionProvider> mockDataProtectionProvider;
		private readonly UtilisateurService utilisateurService;

		public UtilisateurServiceTest()
		{
			// Initialisation des mocks
			mockDbContext = new Mock<DailyTasksMigrationsContext>();
			mockJwtTokenService = new Mock<IJwtTokenService>();
			mockConfig = new Mock<IConfiguration>();
			mockDataProtectionProvider = new Mock<IDataProtectionProvider>();

			// Initialisation du service avec les mocks
			utilisateurService = new UtilisateurService(
				mockDbContext.Object,
				mockJwtTokenService.Object,
				mockConfig.Object,
				mockDataProtectionProvider.Object
			);
		}

		// [Fact]
		// public async Task<ICollection<Utilisateur>> GetUsersByFilter_ReturnUsersOrEmptyList_1()
		// {
		// 	IQueryable<Utilisateur> query = mockDbContext.Utilisateurs
		// 						   .Include(u => u.LesTaches);
		// 	// Act


		// 	// Arrange

		// 	// Assert


		// 	/* 
		// 	Aucun filtre n’a été passé on affiche
		// 	- la liste vide 
		// 	- la liste des utilisateurs
		// 	*/
		// 	await Task.Delay(100);
		// }
		// [Fact]
		// public async Task GetUsersByFilter_ReturnUserOrNot_2()
		// {
		//     /* 
		// 	Le filtre a été passé on retourne
		// 	- un utilisateur 
		// 	- un utilisateur non trouvé
		// 	*/
		//     await Task.Delay(100);

		// }
	}

}