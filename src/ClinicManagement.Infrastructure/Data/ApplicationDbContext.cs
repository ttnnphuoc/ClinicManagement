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
    
    // Billing and Payment
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillItem> BillItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    // Medicine and Prescription
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    
    // Receipt and Invoice
    public DbSet<Receipt> Receipts { get; set; }
    
    // Room Management
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomBooking> RoomBookings { get; set; }
    
    // Queue Management
    public DbSet<PatientQueue> PatientQueues { get; set; }
    
    // Notifications
    public DbSet<Notification> Notifications { get; set; }
    
    // Subscription Management
    public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
    public DbSet<PackageLimit> PackageLimits { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UsageTracking> UsageTrackings { get; set; }

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

        // Configure Bill relationships
        modelBuilder.Entity<Bill>()
            .HasOne(b => b.Patient)
            .WithMany(p => p.Bills)
            .HasForeignKey(b => b.PatientId);

        modelBuilder.Entity<BillItem>()
            .HasOne(bi => bi.Bill)
            .WithMany(b => b.BillItems)
            .HasForeignKey(bi => bi.BillId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Bill)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BillId);

        // Configure Prescription relationships
        modelBuilder.Entity<Prescription>()
            .HasOne(p => p.TreatmentHistory)
            .WithOne(t => t.Prescription)
            .HasForeignKey<Prescription>(p => p.TreatmentHistoryId);

        modelBuilder.Entity<PrescriptionMedicine>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicines)
            .HasForeignKey(pm => pm.PrescriptionId);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Medicine)
            .WithMany(m => m.InventoryItems)
            .HasForeignKey(ii => ii.MedicineId);

        // Configure Receipt relationships
        modelBuilder.Entity<Receipt>()
            .HasOne(r => r.Bill)
            .WithMany(b => b.Receipts)
            .HasForeignKey(r => r.BillId);

        // Configure Room relationships
        modelBuilder.Entity<RoomBooking>()
            .HasOne(rb => rb.Room)
            .WithMany(r => r.RoomBookings)
            .HasForeignKey(rb => rb.RoomId);

        modelBuilder.Entity<RoomBooking>()
            .HasOne(rb => rb.Appointment)
            .WithOne(a => a.RoomBooking)
            .HasForeignKey<RoomBooking>(rb => rb.AppointmentId);

        // Configure Queue relationships
        modelBuilder.Entity<PatientQueue>()
            .HasOne(pq => pq.Patient)
            .WithMany(p => p.PatientQueues)
            .HasForeignKey(pq => pq.PatientId);

        modelBuilder.Entity<PatientQueue>()
            .HasOne(pq => pq.Appointment)
            .WithOne(a => a.PatientQueue)
            .HasForeignKey<PatientQueue>(pq => pq.AppointmentId);

        // Configure Notification relationships
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Patient)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.PatientId);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Appointment)
            .WithMany(a => a.Notifications)
            .HasForeignKey(n => n.AppointmentId);

        // Configure Subscription relationships
        modelBuilder.Entity<Clinic>()
            .HasOne(c => c.Owner)
            .WithMany()
            .HasForeignKey(c => c.OwnerId);

        modelBuilder.Entity<PackageLimit>()
            .HasOne(pl => pl.SubscriptionPackage)
            .WithMany(sp => sp.PackageLimits)
            .HasForeignKey(pl => pl.SubscriptionPackageId);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.SubscriptionPackage)
            .WithMany(sp => sp.Subscriptions)
            .HasForeignKey(s => s.SubscriptionPackageId);

        modelBuilder.Entity<UsageTracking>()
            .HasOne(ut => ut.Subscription)
            .WithMany(s => s.UsageTrackings)
            .HasForeignKey(ut => ut.SubscriptionId);

        // Configure indexes for clinic entities
        modelBuilder.Entity<Patient>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Appointment>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Service>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Transaction>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Inventory>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<TreatmentHistory>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Bill>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Payment>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Medicine>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Prescription>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<InventoryItem>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Receipt>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Room>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<RoomBooking>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<PatientQueue>().HasIndex(e => e.ClinicId);
        modelBuilder.Entity<Notification>().HasIndex(e => e.ClinicId);
        
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