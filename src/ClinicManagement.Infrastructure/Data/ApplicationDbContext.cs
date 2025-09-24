using Microsoft.EntityFrameworkCore;
using ClinicManagement.Core.Entities;

namespace ClinicManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AppointmentService> AppointmentServices { get; set; }
    public DbSet<TreatmentHistory> TreatmentHistories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Inventory> Inventories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}