using System;
using System.Configuration;
using Horsesoft.Music.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Horsesoft.Music.Data.Sqlite.Context
{
    public partial class HorsifyContext : DbContext
    {
        public virtual DbSet<AllJoinedTable> AllJoinedTables { get; set; }
        public virtual DbSet<Album> Album { get; set; }
        public virtual DbSet<Artist> Artist { get; set; }
        public virtual DbSet<Discog> Discog { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<Filter> Filter { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Label> Label { get; set; }
        public virtual DbSet<MusicalKey> MusicalKey { get; set; }
        public virtual DbSet<Playlist> Playlist { get; set; }
        public virtual DbSet<Song> Song { get; set; }
        public virtual DbSet<Status> Status { get; set; }

        public HorsifyContext()
        {
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                try
                {
                    optionsBuilder.UseSqlite(ConfigurationManager.ConnectionStrings["HorsifyDatabase"].ConnectionString);                                       
                }
                catch (Exception)
                {

                    optionsBuilder.UseSqlite(@"Datasource=C:\ProgramData\Horsify\Horsify.db;");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllJoinedTable>(entity =>
            {
                entity.HasIndex(e => e.Id);
            });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasIndex(e => e.Title)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Discog>(entity =>
            {
                entity.HasIndex(e => e.ReleaseId)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasIndex(e => e.Hash)
                    .IsUnique();

                entity.HasIndex(e => new { e.DriveVolume, e.FileName, e.Folder })
                    .HasName("UQ_FileMatch")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.DriveVolume)
                    .IsRequired()
                    .HasColumnType("TEXT (1, 5)");

                entity.Property(e => e.FileName).IsRequired();

                entity.Property(e => e.Folder)
                    .IsRequired()
                    .HasColumnType("TEXT (0)");
            });

            modelBuilder.Entity<Filter>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<FiltersSearch>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.HasIndex(e => e.Items);

                entity.HasIndex(e => e.Count);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<MusicalKey>(entity =>
            {
                entity.HasIndex(e => e.MusicKey)
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.MusicKey)
                    .IsRequired()
                    .HasColumnType("TEXT (1, 12)");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasIndex(e => e.AlbumId);

                entity.HasIndex(e => e.ArtistId);

                entity.HasIndex(e => e.DiscogId);

                entity.HasIndex(e => e.FileId);

                entity.HasIndex(e => e.GenreId);

                entity.HasIndex(e => e.LabelId);

                entity.HasIndex(e => e.MusicalKeyId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AddedDate).HasColumnType("DATETIME");

                entity.Property(e => e.IsDamaged).HasColumnType("BOOLEAN");

                entity.Property(e => e.LastPlayed).HasColumnType("DATETIME");

                entity.Property(e => e.Nsfw)
                    .HasColumnName("NSFW")
                    .HasColumnType("BOOLEAN");

                entity.Property(e => e.Time).HasColumnType("TIME");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.AlbumId);

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.ArtistId);

                entity.HasOne(d => d.Discog)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.DiscogId);

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.FileId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.GenreId);

                entity.HasOne(d => d.Label)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.LabelId);

                entity.HasOne(d => d.MusicalKey)
                    .WithMany(p => p.Song)
                    .HasForeignKey(d => d.MusicalKeyId);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Status1)
                    .IsRequired()
                    .HasColumnName("Status")
                    .HasColumnType("TEXT (1, 15)");
            });
        }
    }
}
