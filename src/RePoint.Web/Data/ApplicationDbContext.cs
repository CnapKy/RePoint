using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RePoint.Web.Models;

namespace RePoint.Web.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<CollectionPoint> CollectionPoints => Set<CollectionPoint>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<PointMaterial> PointMaterials => Set<PointMaterial>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<HandoverRequest> HandoverRequests => Set<HandoverRequest>();
    public DbSet<ConfirmedDelivery> ConfirmedDeliveries => Set<ConfirmedDelivery>();
    public DbSet<IssueReport> IssueReports => Set<IssueReport>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "repoint-role-user", Name = "User", NormalizedName = "USER", ConcurrencyStamp = "repoint-role-user" },
            new IdentityRole { Id = "repoint-role-operator", Name = "Operator", NormalizedName = "OPERATOR", ConcurrencyStamp = "repoint-role-operator" },
            new IdentityRole { Id = "repoint-role-organizer", Name = "Organizer", NormalizedName = "ORGANIZER", ConcurrencyStamp = "repoint-role-organizer" },
            new IdentityRole { Id = "repoint-role-administrator", Name = "Administrator", NormalizedName = "ADMINISTRATOR", ConcurrencyStamp = "repoint-role-administrator" }
        );

        b.Entity<Campaign>().ToTable(t => t.HasCheckConstraint("CK_Campaigns_Dates", "[EndsAtUtc] >= [StartsAtUtc]"));
        b.Entity<HandoverRequest>().ToTable(t => t.HasCheckConstraint(
            "CK_HandoverRequests_EstimatedKg", "[EstimatedKg] BETWEEN 0.001 AND 500"));
        b.Entity<ConfirmedDelivery>().ToTable(t => t.HasCheckConstraint(
            "CK_ConfirmedDeliveries_ActualKg", "[ActualKg] > 0"));
        b.Entity<Material>().HasIndex(x => x.Code).IsUnique();
        b.Entity<CollectionPoint>().HasIndex(x => new { x.City, x.IsActive });
        b.Entity<CollectionPoint>().Property(x => x.Latitude).HasPrecision(9, 6);
        b.Entity<CollectionPoint>().Property(x => x.Longitude).HasPrecision(9, 6);
        b.Entity<PointMaterial>().HasKey(x => new { x.CollectionPointId, x.MaterialId });
        b.Entity<PointMaterial>().HasOne(x => x.CollectionPoint).WithMany(x => x.AcceptedMaterials)
            .HasForeignKey(x => x.CollectionPointId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<PointMaterial>().HasOne(x => x.Material).WithMany(x => x.CollectionPoints)
            .HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Campaign>().HasOne(x => x.CollectionPoint).WithMany()
            .HasForeignKey(x => x.CollectionPointId).OnDelete(DeleteBehavior.SetNull);
        b.Entity<HandoverRequest>().Property(x => x.EstimatedKg).HasPrecision(12, 3);
        b.Entity<HandoverRequest>().HasIndex(x => new { x.UserId, x.SubmittedAtUtc });
        b.Entity<HandoverRequest>().Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.Entity<HandoverRequest>().HasOne(x => x.User).WithMany(x => x.HandoverRequests)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<HandoverRequest>().HasOne(x => x.CollectionPoint).WithMany()
            .HasForeignKey(x => x.CollectionPointId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<HandoverRequest>().HasOne(x => x.Material).WithMany()
            .HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<HandoverRequest>().HasOne<PointMaterial>().WithMany()
            .HasForeignKey(x => new { x.CollectionPointId, x.MaterialId }).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ConfirmedDelivery>().Property(x => x.ActualKg).HasPrecision(12, 3);
        b.Entity<ConfirmedDelivery>().HasIndex(x => x.HandoverRequestId).IsUnique();
        b.Entity<ConfirmedDelivery>().HasOne(x => x.HandoverRequest).WithOne(x => x.ConfirmedDelivery)
            .HasForeignKey<ConfirmedDelivery>(x => x.HandoverRequestId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<ConfirmedDelivery>().HasOne(x => x.OperatorUser).WithMany()
            .HasForeignKey(x => x.OperatorUserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<IssueReport>().HasOne(x => x.CollectionPoint).WithMany()
            .HasForeignKey(x => x.CollectionPointId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<IssueReport>().HasOne<ApplicationUser>().WithMany()
            .HasForeignKey(x => x.SubmittedByUserId).OnDelete(DeleteBehavior.SetNull);
        b.Entity<Notification>().HasOne(x => x.User).WithMany()
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

        b.Entity<Material>().HasData(
            new Material { Id = 1, Name = "Хартия и картон", Code = "PAPER", Description = "Хартия и картонени опаковки" },
            new Material { Id = 2, Name = "Пластмаса", Code = "PLASTIC", Description = "Пластмасови опаковки" },
            new Material { Id = 3, Name = "Стъкло", Code = "GLASS", Description = "Стъклени опаковки" },
            new Material { Id = 4, Name = "Метал", Code = "METAL", Description = "Метални опаковки" },
            new Material { Id = 5, Name = "Батерии", Code = "BATTERY", Description = "Батерии от домакинството (само специализирани пунктове)" }
        );
        // Тези записи са учебни, НЕ представляват реални пунктове за предаване.
        b.Entity<CollectionPoint>().HasData(
            new CollectionPoint { Id = 1, Name = "Демо пункт – Център", City = "София", Address = "Демонстрационен адрес – не е реален пункт", OpeningHours = "Пон–Пет 09:00–18:00" },
            new CollectionPoint { Id = 2, Name = "Демо пункт – Изток", City = "София", Address = "Демонстрационен адрес – не е реален пункт", OpeningHours = "Пон–Съб 10:00–17:00" }
        );
        b.Entity<PointMaterial>().HasData(
            new PointMaterial { CollectionPointId = 1, MaterialId = 1 },
            new PointMaterial { CollectionPointId = 1, MaterialId = 2 },
            new PointMaterial { CollectionPointId = 2, MaterialId = 3 },
            new PointMaterial { CollectionPointId = 2, MaterialId = 4 }
        );
    }
}
