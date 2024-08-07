using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TasksManagement_API.Models;

namespace TasksManagement_API.DataBaseContext
{

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DailyTasksMigrationsContext>
	{
		public DailyTasksMigrationsContext CreateDbContext(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.Production.json")
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<DailyTasksMigrationsContext>();
			optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
			sqlOptions => sqlOptions.EnableRetryOnFailure(
			maxRetryCount: 10,
			maxRetryDelay: TimeSpan.FromSeconds(40),
			errorNumbersToAdd: null));

			return new DailyTasksMigrationsContext(optionsBuilder.Options);
		}
	}

}
