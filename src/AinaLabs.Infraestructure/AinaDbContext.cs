using Microsoft.EntityFrameworkCore;
using System.Data;
using AinaLabs.Core.Interfaces;
using AinaLabs.Core.Models;

namespace AinaLabs.Infraestructure;

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
        modelBuilder.Entity<Document>().ToTable("Documents");
        modelBuilder.Entity<DocumentChunk>().ToTable("DocumentChunks");
        
        // Configuración para el tipo 'vector' de pgvector
        modelBuilder.HasPostgresExtension("vector");
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