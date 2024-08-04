using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Models;

namespace TasksManagement_API.DataBaseContext
{
	public class DailyTasksForMemory : DbContext
	{
		public DailyTasksForMemory(DbContextOptions<DailyTasksForMemory> options)
		: base(options)
		{
		}
		public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
		public DbSet<Tache> Taches { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tache>()
			.Property(t => t.TasksDate)
			.HasColumnName("DateH");
			base.OnModelCreating(modelBuilder);
		}
	}
}