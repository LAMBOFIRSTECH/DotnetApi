using Microsoft.EntityFrameworkCore;
namespace TasksManagement_API.Models;

public class DailyTasksMigrationsContext : DbContext
{
	public DailyTasksMigrationsContext(DbContextOptions<DailyTasksMigrationsContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
	public DbSet<Tache> Taches { get; set; } = null!;



	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Utilisateur>()
		   .HasMany(u => u.LesTaches)
		   .WithOne(t => t.utilisateur)
		   .HasForeignKey(t => t.UserId) 
		   .IsRequired()
		   .OnDelete(DeleteBehavior.Cascade);
		modelBuilder.Entity<Tache>();
		base.OnModelCreating(modelBuilder);
	}
	
}

