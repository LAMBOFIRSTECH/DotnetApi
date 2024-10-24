using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement_API.Controllers;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Xunit;

namespace TasksManagement_Tests.aIntegrationTests
{
	public class UsersManagementControllerTest
	{
		const int userID = 1;
		Mock<IReadUsersMethods> mockReadMethods = new Mock<IReadUsersMethods>();
		Mock<IWriteUsersMethods> mockWriteMethods1 = new Mock<IWriteUsersMethods>();
		Mock<IWriteUsersMethods> mockWriteMethods2 = new Mock<IWriteUsersMethods>();
		private readonly DailyTasksMigrationsContext dbContext;
		public UsersManagementControllerTest()
		{
			// Configuration de la base de données en mémoire // on aura presque pas besoin
			var options = new DbContextOptionsBuilder<DailyTasksMigrationsContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
				.Options;
			dbContext = new DailyTasksMigrationsContext(options);
			//dans le cas des mocks on doit initialiser le service avec les mocks
			// utilisateurService = new UtilisateurService(
			// 	dbContext,
			// 	Mock.Of<IJwtTokenService>(),
			// 	Mock.Of<IConfiguration>(),
			// 	Mock.Of<IDataProtectionProvider>()
			// );
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
		[Theory]
		[InlineData("password")]
		public async Task CheckEncryptedPassword_Returns_Ok_1(string password)
		{
			Utilisateur utilisateur = new Utilisateur(); // Test sur les endpoints d'api test d'intégration
			await Task.Delay(1000);

		}
		// [Fact]
		// public async Task GetUsersReturns_OkResult_3()
		// {
		// 	// Arrange
		// 	var expectedUserList = new List<Utilisateur>();
		// 	mockReadMethods.Setup(m => m.GetUsers()).ReturnsAsync(expectedUserList);

		// 	// Passez null si le writeMethods n'est pas nécessaire dans ce test
		// 	var controller = new UsersManagementController(mockReadMethods.Object, null);

		// 	// Act
		// 	var result = await controller.GetUsers();

		// 	// Assert
		// 	var okResult = Assert.IsType<OkObjectResult>(result);
		// 	var actualUserList = Assert.IsAssignableFrom<IEnumerable<Utilisateur>>(okResult.Value);
		// 	Assert.Equal(expectedUserList, actualUserList);
		// }

		// [Fact]
		// public async Task GetUsersByIdReturns_NotFound_or_OkResult_2()
		// {
		// 	// Arrange
		// 	mockReadMethods.SetupSequence(m => m.GetUserById(userID))
		// 	.ReturnsAsync(new Utilisateur() { ID = userID, Nom = "nom", Pass = "password", Role = Utilisateur.Privilege.UserX, Email = "toto@gmail.com" })
		// 	.ReturnsAsync((Utilisateur)null!);

		// 	var controller = new UsersManagementController(mockReadMethods.Object, null!);

		// 	// Act
		// 	var result1 = await controller.GetUserById(1);
		// 	var result2 = await controller.GetUserById(2);

		// 	//Assert
		// 	var okResult = Assert.IsType<OkObjectResult>(result1);
		// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

		// 	var notResult = Assert.IsType<NotFoundObjectResult>(result2);
		// 	Assert.Equal(StatusCodes.Status404NotFound, notResult.StatusCode);
		// }

		// [Fact]
		// public async Task CreateUserReturns_BadRequest_or_Conflict_or_CorrectsData_3()
		// {
		// 	// Arrange
		// 	var user = new Utilisateur()
		// 	{
		// 		ID = 1,
		// 		Nom = "nom",
		// 		Pass = "pass",
		// 		Role = Utilisateur.Privilege.Admin,
		// 		Email = "toto@gmail.com"
		// 	};

		// 	var users = new List<Utilisateur> { user };
		// 	mockReadMethods.Setup(m => m.GetUsers()).ReturnsAsync(users);
		// 	mockWriteMethods1.Setup(m => m.CreateUser(It.IsAny<Utilisateur>())).ThrowsAsync(new Exception());
		// 	var controller1 = new UsersManagementController(mockReadMethods.Object, mockWriteMethods1.Object);

		// 	mockWriteMethods2.Setup(m => m.CreateUser(It.IsAny<Utilisateur>())).ReturnsAsync((Utilisateur)null!);
		// 	var controller2 = new UsersManagementController(mockReadMethods.Object, mockWriteMethods2.Object);

		// 	// Act
		// 	var result1 = await controller1.CreateUser(2, "nom", "password", "Excepted Admin/UserX", "toto@gmail.com");
		// 	var result2 = await controller2.CreateUser(3, "nom", "password", "Admin", "toto@gmail.com");
		// 	var result3 = await controller2.CreateUser(4, "nom", "password", "UserX", "toto@gmail.com");

		// 	//Assert
		// 	var badRequestResult = Assert.IsType<BadRequestObjectResult>(result1);
		// 	Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
		// 	var conflictResult = Assert.IsType<ConflictObjectResult>(result2);
		// 	Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
		// 	var okResult = Assert.IsType<OkObjectResult>(result3);
		// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
		// }

		// [Fact]
		// public async Task DeleteUserReturns_NotFound_or_CorrectDeleting_4()
		// {
		// 	//Arrange
		// 	mockReadMethods.SetupSequence(m => m.GetUserById(userID))
		// 	.ReturnsAsync(new Utilisateur() { ID = userID })
		// 	.ReturnsAsync((Utilisateur)null!);

		// 	var controller = new UsersManagementController(mockReadMethods.Object, mockWriteMethods1.Object);

		// 	//Act
		// 	var result1 = await controller.DeleteUserById(userID);
		// 	var result2 = await controller.DeleteUserById(userID);

		// 	//Assert
		// 	var notFound = Assert.IsType<NotFoundObjectResult>(result2);
		// 	Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);

		// 	var okResult = Assert.IsType<OkObjectResult>(result1);
		// 	Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

		// }

		// // [Fact]
		// // public async Task UpdateUserPasswordReturns_NotFound_or_OkUpdating_5()
		// // {
		// //     //Arrange
		// //     mockReadMethods.SetupSequence(m => m.GetUserById(userID))
		// //     .ReturnsAsync(new Utilisateur() { ID = userID, Nom = "nom", Pass = "password", Role = Utilisateur.Privilege.UserX, Email = "toto@gmail.com" })
		// //     .ReturnsAsync((Utilisateur)null!);

		// //     var controller = new UsersManagementController(mockReadMethods.Object, mockWriteMethods1.Object);
		// //     //Act
		// //     var result1 = await controller.UpdateUserPassword("nom", "password");
		// //     //Assert
		// //     var okResult = Assert.IsType<OkObjectResult>(result1);
		// //     Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
		// // }

	}

}

