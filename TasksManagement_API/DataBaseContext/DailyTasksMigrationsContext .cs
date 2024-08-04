using Microsoft.EntityFrameworkCore;
namespace TasksManagement_API.Models;

public class DailyTasksMigrationsContext : DbContext
{
	public DailyTasksMigrationsContext(DbContextOptions<DailyTasksMigrationsContext> options)
		: base(options)
	{
	}

	public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
	public DbSet<Tache> Taches { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Tache>()
		.Property(t => t.StartDateH)
		.HasColumnName("StartDateH")
		.HasColumnType("Datetime");
		
		modelBuilder.Entity<Tache>()
		.Property(t => t.EndDateH)
		.HasColumnName("EndDateH")
		.HasColumnType("Datetime");
		
		base.OnModelCreating(modelBuilder);
	}
}

