﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TasksManagement_API.Models;

#nullable disable

namespace TasksManagement_API.Migrations
{
    [DbContext(typeof(DailyTasksMigrationsContext))]
    [Migration("20241021105446_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("TasksManagement_API.Models.Tache", b =>
                {
                    b.Property<int>("Matricule")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Matricule"), 1L, 1);

                    b.Property<string>("EmailUtilisateur")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDateH")
                        .HasColumnType("datetime2");

                    b.Property<string>("NomUtilisateur")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDateH")
                        .HasColumnType("datetime2");

                    b.Property<string>("Summary")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Matricule");

                    b.HasIndex("UserId");

                    b.ToTable("Taches");
                });

            modelBuilder.Entity("TasksManagement_API.Models.Utilisateur", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pass")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Utilisateurs");
                });

            modelBuilder.Entity("TasksManagement_API.Models.Tache", b =>
                {
                    b.HasOne("TasksManagement_API.Models.Utilisateur", "utilisateur")
                        .WithMany("LesTaches")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("utilisateur");
                });

            modelBuilder.Entity("TasksManagement_API.Models.Utilisateur", b =>
                {
                    b.Navigation("LesTaches");
                });
#pragma warning restore 612, 618
        }
    }
}
