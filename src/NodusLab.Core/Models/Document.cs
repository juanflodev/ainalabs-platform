using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NodusLab.Core.Models;


[Table("documents")]
public class Document
{
  [Key]
  [Column("id")]
  public Guid Id { get; set; }
  
  [Required]
  [Column("tenant_id")]
  public Guid TenantId { get; set; }
  
  [Required]
  [Column("filename")]
  public string FileName { get; set; } = string.Empty;

  [Column("status")] 
  public string Status { get; set; } = "pending";
  
  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  
  // Propiedades de navegación
  public Tenant? Tenant { get; set; }
  
  public ICollection<DocumentChunk> Chunks { get; set; } = new List<DocumentChunk>();
  
}