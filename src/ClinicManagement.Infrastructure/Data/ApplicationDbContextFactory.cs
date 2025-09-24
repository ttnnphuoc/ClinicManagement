using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ClinicManagement.Infrastructure.Data;
using ClinicManagement.Core.Interfaces;

namespace ClinicManagement.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=ClinicManagementDB;User Id=postgres;Password=sa;Pooling=true;Maximum Pool Size=200;Command Timeout=300;");

        // Create a mock clinic context for design-time
        var clinicContext = new DesignTimeClinicContext();
        
        return new ApplicationDbContext(optionsBuilder.Options, clinicContext);
    }
}

// Mock implementation for design-time
public class DesignTimeClinicContext : IClinicContext
{
    public Guid? CurrentClinicId => null;
    
    public void SetClinicId(Guid clinicId)
    {
        // No-op for design-time
    }
}