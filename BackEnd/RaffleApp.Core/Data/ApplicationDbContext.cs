using Microsoft.EntityFrameworkCore;
using RaffleApp.Core.Models;

namespace RaffleApp.Core.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Raffle> Raffles { get; set; }
    public DbSet<RaffleNumber> RaffleNumbers { get; set; }
    public DbSet<PriceConfiguration> PriceConfigurations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Participant> Participants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Raffle
        modelBuilder.Entity<Raffle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()"); // Genera UUIDs automáticamente
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()"); // PostgreSQL
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Configuración de RaffleNumber
        modelBuilder.Entity<RaffleNumber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()"); // Genera UUIDs automáticamente
            entity.Property(e => e.ParticipantName).HasMaxLength(100);
            entity.Property(e => e.ParticipantEmail).HasMaxLength(100);
            entity.Property(e => e.ParticipantPhone).HasMaxLength(20);
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            // Relación con Raffle
            entity.HasOne(e => e.Raffle)
                  .WithMany(r => r.RaffleNumbers)
                  .HasForeignKey(e => e.RaffleId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relación con Payment
            entity.HasOne(e => e.Payment)
                  .WithMany(p => p.RaffleNumbers)
                  .HasForeignKey(e => e.PaymentId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Índice único para Number + RaffleId
            entity.HasIndex(e => new { e.RaffleId, e.Number }).IsUnique();
        });

        // Configuración de PriceConfiguration
        modelBuilder.Entity<PriceConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()"); // Genera UUIDs automáticamente
            entity.Property(e => e.PriceFor1).HasColumnType("numeric(10,2)"); // PostgreSQL usa numeric
            entity.Property(e => e.PriceFor2).HasColumnType("numeric(10,2)"); // PostgreSQL usa numeric
            entity.Property(e => e.PriceFor3).HasColumnType("numeric(10,2)"); // PostgreSQL usa numeric
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()"); // PostgreSQL

            // Relación con Raffle
            entity.HasOne(e => e.Raffle)
                  .WithOne(r => r.PriceConfiguration)
                  .HasForeignKey<PriceConfiguration>(e => e.RaffleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()"); // Genera UUIDs automáticamente
            entity.Property(e => e.ParticipantName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ParticipantEmail).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ParticipantPhone).HasMaxLength(20);
            entity.Property(e => e.Amount).HasColumnType("numeric(10,2)"); // PostgreSQL usa numeric
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.PaymentGatewayId).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()"); // PostgreSQL

            // Relación con Raffle
            entity.HasOne(e => e.Raffle)
                  .WithMany(r => r.Payments)
                  .HasForeignKey(e => e.RaffleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Participant
        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()"); // Genera UUIDs automáticamente
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()"); // PostgreSQL

            entity.HasIndex(e => new { e.RaffleId, e.Email }).IsUnique();

            // Relación con Raffle
            entity.HasOne(e => e.Raffle)
                  .WithMany(r => r.Participants)
                  .HasForeignKey(e => e.RaffleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Datos iniciales (Seed Data)
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var raffleId = Guid.NewGuid();

        // Crear una rifa inicial
        modelBuilder.Entity<Raffle>().HasData(
            new Raffle
            {
                Id = raffleId,
                Name = "Rifa Inicial",
                Description = "Primera rifa del sistema",
                StartDate = DateTime.UtcNow, 
                EndDate = DateTime.UtcNow.AddDays(30), 
                IsActive = true,
                CreatedAt = DateTime.UtcNow 
            }
        );

        // Configuración de precios inicial
        modelBuilder.Entity<PriceConfiguration>().HasData(
            new PriceConfiguration
            {
                Id = Guid.NewGuid(),
                RaffleId = raffleId,
                PriceFor1 = 1000m,
                PriceFor2 = 1800m,
                PriceFor3 = 2500m,
                CreatedAt = DateTime.UtcNow 
            }
        );

        // Crear números del 1 al 100
        var raffleNumbers = new List<RaffleNumber>();
        for (int i = 1; i <= 100; i++)
        {
            raffleNumbers.Add(new RaffleNumber
            {
                Id = Guid.NewGuid(),
                RaffleId = raffleId,
                Number = i,
                IsAvailable = true
            });
        }

        modelBuilder.Entity<RaffleNumber>().HasData(raffleNumbers);
    }
}