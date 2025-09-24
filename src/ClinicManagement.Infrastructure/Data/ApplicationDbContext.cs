using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;
using ClinicManagement.Core.Interfaces;
using System.Linq.Expressions;

namespace ClinicManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IClinicContext _clinicContext;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IClinicContext clinicContext) : base(options)
    {
        _clinicContext = clinicContext;
    }

    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<StaffClinic> StaffClinics { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentService> AppointmentServices { get; set; }
    public DbSet<TreatmentHistory> TreatmentHistories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Inventory> Inventories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StaffClinic>()
            .HasOne(sc => sc.Staff)
            .WithMany(s => s.StaffClinics)
            .HasForeignKey(sc => sc.StaffId);

        modelBuilder.Entity<StaffClinic>()
            .HasOne(sc => sc.Clinic)
            .WithMany(c => c.StaffClinics)
            .HasForeignKey(sc => sc.ClinicId);

        modelBuilder.Entity<AppointmentService>()
            .HasKey(a => new { a.AppointmentId, a.ServiceId });

        modelBuilder.Entity<AppointmentService>()
            .HasOne(a => a.Appointment)
            .WithMany(ap => ap.AppointmentServices)
            .HasForeignKey(a => a.AppointmentId);

        modelBuilder.Entity<AppointmentService>()
            .HasOne(a => a.Service)
            .WithMany(s => s.AppointmentServices)
            .HasForeignKey(a => a.ServiceId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Staff)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.StaffId);

        modelBuilder.Entity<TreatmentHistory>()
            .HasOne(t => t.Patient)
            .WithMany(p => p.TreatmentHistories)
            .HasForeignKey(t => t.PatientId);

        // Configure indexes for clinic entities
        modelBuilder.Entity<Patient>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Appointment>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Service>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Transaction>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Inventory>().HasIndex(e => e.ClinicId);
        
        // Configure other indexes
        modelBuilder.Entity<StaffClinic>().HasIndex(e => new { e.StaffId, e.ClinicId }).IsUnique();
        
        // Note: Query filters are commented out for migration creation
        // They can be added back after the initial migration is created
        /*
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Only apply filters to the concrete entity types, not base types
            if (typeof(IClinicEntity).IsAssignableFrom(entityType.ClrType) && !entityType.ClrType.IsAbstract)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var clinicIdProperty = Expression.Property(parameter, nameof(IClinicEntity.ClinicId));
                var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                
                var clinicIdValue = Expression.Property(
                    Expression.Constant(_clinicContext),
                    nameof(IClinicContext.CurrentClinicId)
                );
                
                // Handle nullable comparison - only filter if CurrentClinicId has a value
                var hasValue = Expression.Property(clinicIdValue, "HasValue");
                var value = Expression.Property(clinicIdValue, "Value");
                var clinicIdEquals = Expression.Condition(
                    hasValue,
                    Expression.Equal(clinicIdProperty, value),
                    Expression.Constant(true) // Include all records if no clinic is selected
                );
                var isNotDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                
                var combinedExpression = Expression.AndAlso(clinicIdEquals, isNotDeleted);
                var lambda = Expression.Lambda(combinedExpression, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
            else if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && !entityType.ClrType.IsAbstract && !typeof(IClinicEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var isNotDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(isNotDeleted, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
        */
    }
}