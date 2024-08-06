using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TasksManagement_API.DataBaseContext;
using TasksManagement_API.Models;

namespace TasksManagement_API.DataBaseContext
{

	public class DesignTimeDbContextFactory //: IDesignTimeDbContextFactory<DailyTasksMigrationsContext>
{
	// public DailyTasksMigrationsContext CreateDbContext(string[] args)
	// {
	// 	var configuration = new ConfigurationBuilder()
	// 		.SetBasePath(Directory.GetCurrentDirectory())
	// 		.AddJsonFile("appsettings.Production.json")
	// 		.Build();

	// 	var optionsBuilder = new DbContextOptionsBuilder<DailyTasksMigrationsContext>();
	// 	optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

	// 	return new DailyTasksMigrationsContext(optionsBuilder.Options);
	// }
}

}