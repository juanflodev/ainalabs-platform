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

    [Required] [Column("content")] 
    public string Content { get; set; } = string.Empty;
    
    [Column("model_used")]
    public string? ModelUsed { get; set; }

    [Column("embedding_768", TypeName = "vector(768)")]
    public Vector? Embedding768 { get; set; }

    [Column("embedding_1536", TypeName = "vector(1536)")]
    public Vector? Embedding1536 { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    

    [ForeignKey("TenantId")]
    public Tenant? Tenant { get; set; }
    
    [ForeignKey("DocumentId")]
    public Document? Document { get; set; }

}