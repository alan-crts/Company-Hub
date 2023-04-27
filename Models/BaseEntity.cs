using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyHub.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}