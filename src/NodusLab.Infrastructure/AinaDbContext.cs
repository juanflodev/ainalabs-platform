using Microsoft.EntityFrameworkCore;
using NodusLab.Core.Interfaces;
using NodusLab.Core.Models;

namespace NodusLab.Infrastructure;

public class AinaDbContext : DbContext
{
    private readonly Guid _currentTenantId;
    public AinaDbContext(DbContextOptions<AinaDbContext> options, ITenantService tenantService) : base(options)
    {
        _currentTenantId = tenantService.GetTenantId();
    }
    
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentChunk> DocumentChunks { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Siempre llamar al base primero
        base.OnModelCreating(modelBuilder);

        // 2. Extensión de IA
        modelBuilder.HasPostgresExtension("vector");

        // 3. FORZAR LOS NOMBRES EXACTOS DE LAS TABLAS EN MINÚSCULAS
        modelBuilder.Entity<Tenant>().ToTable("tenants");
        modelBuilder.Entity<Document>().ToTable("documents");
        modelBuilder.Entity<DocumentChunk>().ToTable("document_chunks");

        // 4. Forzar los nombres exactos de las columnas clave (por si acaso)
        modelBuilder.Entity<Document>()
            .Property(d => d.TenantId)
            .HasColumnName("tenant_id");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await SetTenantSessionVariable();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task SetTenantSessionVariable()
    {
        var sql = $"SET app.current_tenant = '{_currentTenantId}';";
        await Database.ExecuteSqlRawAsync(sql);
    }
}