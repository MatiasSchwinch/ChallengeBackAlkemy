using ChallengeBackendCSharp.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChallengeBackendCSharp.Services
{
    public class DatabaseConnector : IdentityDbContext
    {
        public DbSet<Character>? Characters { get; set; }
        public DbSet<AudiovisualWork>? AudiovisualWorks { get; set; }
        public DbSet<CharacterAudiovisualWork>? CharacterAudiovisualWorks { get; set; }
        public DbSet<Genre>? Genres { get; set; }
        public DbSet<GenreAudiovisualWork>? GenresAudiovisualWorks { get; set; }

        public DatabaseConnector(DbContextOptions options) : base(options) { }

        // Modelado de la Base de datos.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Importante pasar el base para que también se modelen las tablas necesarias para el uso de Identity.
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Character>(entity =>
            {
                entity.ToTable("Character");

                entity.HasKey(pk => pk.CharacterID);

                entity.Property(prop => prop.Image).HasColumnType("nvarchar(80)");

                entity.Property(prop => prop.Name).HasColumnType("nvarchar(90)").IsRequired();

                entity.Property(prop => prop.Age).HasColumnType("smallint");

                entity.Property(prop => prop.Weight).HasColumnType("decimal(6,2)");

                entity.Property(prop => prop.History);
            });

            modelBuilder.Entity<AudiovisualWork>(entity =>
            {
                entity.ToTable("AudiovisualWork");

                entity.HasKey(pk => pk.AudiovisualWorkID);

                entity.Property(prop => prop.Image).HasColumnType("nvarchar(80)");

                entity.Property(prop => prop.Title).HasColumnType("nvarchar(120)").IsRequired();

                entity.Property(prop => prop.ReleaseDate).HasColumnType("date");

                entity.Property(prop => prop.Rating).HasColumnType("decimal(2,1)");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genre");

                entity.HasKey(pk => pk.GenreID);

                entity.Property(prop => prop.Image).HasColumnType("nvarchar(80)");

                entity.Property(prop => prop.Name).HasColumnType("nvarchar(40)").IsRequired();
            });

            modelBuilder.Entity<CharacterAudiovisualWork>(entity =>
            {
                entity.ToTable("CharacterAudiovisualWork");

                entity.HasKey(pk => new { pk.CharacterID, pk.AudiovisualWorkID });

                entity.HasOne(rel => rel.Character)
                      .WithMany(rel => rel.CharacterAudiovisualWorks)
                      .HasForeignKey(fk => fk.CharacterID);

                entity.HasOne(rel => rel.AudiovisualWork)
                      .WithMany(rel => rel.CharacterAudiovisualWorks)
                      .HasForeignKey(fk => fk.AudiovisualWorkID);
            });

            modelBuilder.Entity<GenreAudiovisualWork>(entity =>
            {
                entity.ToTable("GenreAudiovisualWork");

                entity.HasKey(pk => new { pk.GenreID, pk.AudiovisualWorkID });

                entity.HasOne(rel => rel.Genre)
                      .WithMany(rel => rel.GenreAudiovisualWorks)
                      .HasForeignKey(fk => fk.GenreID);

                entity.HasOne(rel => rel.AudiovisualWork)
                      .WithMany(rel => rel.GenreAudiovisualWorks)
                      .HasForeignKey(fk => fk.AudiovisualWorkID);
            });
        }
    }
}
