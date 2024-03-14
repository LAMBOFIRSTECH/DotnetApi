using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement_API.Controllers;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Xunit;

namespace TasksManagement_Tests;
public class LoginTest
{
	private readonly DailyTasksMigrationsContext dailyTasks;
	Mock<IJwtTokenService> jwtTokenService = new Mock<IJwtTokenService>();
	public LoginTest()
	{
		var options = new DbContextOptionsBuilder<DailyTasksMigrationsContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;
		dailyTasks = new DailyTasksMigrationsContext(options);
	}
	[Fact]
	public async Task GenerateTokenReturns_Conflict_or_Unauthorize_orExactToken()
	{
		//Arrange
		var user1 = new Utilisateur()
		{
			ID = 1,
			Nom = "nom",
			Pass = "pass",
			Role = Utilisateur.Privilege.Admin,
			Email = "toto@gmail.com"
		};
		var user2 = new Utilisateur()
		{
			ID = 2,
			Nom = "name",
			Pass = "pass",
			Role = Utilisateur.Privilege.UserX,
			Email = "name@gmail.com"
		};
		var user= new Utilisateur();
		dailyTasks.Utilisateurs.Add(user1);
		dailyTasks.Utilisateurs.Add(user2);
		dailyTasks.SaveChanges();
		jwtTokenService.Setup(m=>m.GenerateJwtToken(user.Email));
		var controller1 = new AccessTokenController(dailyTasks, null!);
		var controller2 = new AccessTokenController(dailyTasks, jwtTokenService.Object);

        //Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var result1 = await controller1.Login(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        var result2 = await controller1.Login("titi@gmail.com");
		var result3 = await controller1.Login("name@gmail.com");
		var result4 = await controller2.Login("toto@gmail.com");

		//Assert
		var conflictResult1 = Assert.IsType<ConflictObjectResult>(result1);
		Assert.Equal(StatusCodes.Status409Conflict, conflictResult1.StatusCode);

		var conflictResult2 = Assert.IsType<ConflictObjectResult>(result2);
		Assert.Equal(StatusCodes.Status409Conflict, conflictResult2.StatusCode);

		var unAauthorizeResult = Assert.IsType<UnauthorizedObjectResult>(result3);
		Assert.Equal(StatusCodes.Status401Unauthorized, unAauthorizeResult.StatusCode);

		var okResult = Assert.IsType<OkObjectResult>(result4);
		Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
	}

}