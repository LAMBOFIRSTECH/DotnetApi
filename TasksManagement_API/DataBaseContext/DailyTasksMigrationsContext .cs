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


// Cette fonction ci-dessous représente Fluent API 
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Utilisateur>()
		   .HasMany(u => u.LesTaches)
		   .WithOne(t => t.utilisateur)
		   .HasForeignKey(t => t.UserId) // Fais de la clé primaire d'un utilisateur une clé étrangère dans la tache
		   .IsRequired()
		   .OnDelete(DeleteBehavior.Cascade); // Va supprimer une tache si l'utilisateur n'existe plus
		modelBuilder.Entity<Tache>();
		base.OnModelCreating(modelBuilder);
	}
	
}

