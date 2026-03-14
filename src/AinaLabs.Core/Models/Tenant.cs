using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace AinaLabs.Core.Models;

[Table("Tenants")]
public class Tenant
{
  [Key]
  [Column("id")]
  public Guid Id { get; set; }


  [Required] 
  [Column("name")] 
  public string Name { get; set; } = string.Empty;

  [Column("is_active")] 
  public bool IsActive { get; set; } = true;
  
  [Column("created_at")] 
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  
  // Relaciones
  public ICollection<Document> Documents { get; set; } = new List<Document>();
}
