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
		modelBuilder.Entity<Utilisateur>(entity =>
		  {
			  entity.HasKey(u => u.ID);
			  entity.Property(u => u.Nom).IsRequired();
			  entity.Property(u => u.Email).IsRequired();
			  entity.Property(u => u.Pass).IsRequired();
			  entity.Property(u => u.Role).IsRequired();
		  });

		modelBuilder.Entity<Tache>(entity =>
		{
			entity.HasKey(t => t.Matricule);
			entity.Property(t => t.Titre).IsRequired();
			entity.Property(t => t.Summary);
			entity.Property(t => t.StartDateH)
			.HasColumnName("StartDateH")
			.HasColumnType("Datetime");
			entity.Property(t => t.EndDateH)
			.HasColumnName("EndDateH")
			.HasColumnType("Datetime");
		});


		base.OnModelCreating(modelBuilder);
	}
}

