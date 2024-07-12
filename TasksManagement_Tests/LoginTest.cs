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
	private readonly IWriteUsersMethods mockWriteMethods;
	// public LoginTest()
	// {
	// 	var options = new DbContextOptionsBuilder<DailyTasksMigrationsContext>()
	// 		.UseInMemoryDatabase(databaseName: "TestDatabase")
	// 		.Options;
	// 	dailyTasks = new DailyTasksMigrationsContext(options);
	// }
	[Fact]
	public async Task GenerateTokenReturns_Conflict_or_Unauthorize_orExactToken()
	{
		//Arrange
		
		var email = "toto@gmail.com";
		mockWriteMethods.GetToken(email);
		var controller = new AccessTokenController(mockWriteMethods);
		//Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		var result1 = await controller.Login(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		var result2 = await controller.Login("toto@gmail.com");

		//Assert
		var conflictResult1 = Assert.IsType<ConflictObjectResult>(result1);
		Assert.Equal(StatusCodes.Status409Conflict, conflictResult1.StatusCode);

		var okResult = Assert.IsType<OkObjectResult>(result2);
		Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
	}

}