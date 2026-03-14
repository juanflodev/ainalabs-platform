using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace AinaLabs.Core.Models;


[Table("document_chunks")]
public class DocumentChunk
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }
    
    [Required]
    [Column("document_id")]
    public Guid DocumentId { get; set; }

    [Required] [Column("content")] public string Content { get; set; } = string.Empty;
    
    // Aquí usamos el tipo Vector del paguete pgvector
    [Column("embedding", TypeName = "vector(1536")] 
    public Vector? Embedding { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Propiedades de navegación
    [ForeignKey("TenantId")]
    public Tenant? Tenant { get; set; }
    
    [ForeignKey("DocumentId")]
    public Document? Document { get; set; }

}