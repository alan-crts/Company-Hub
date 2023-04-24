using System.ComponentModel.DataAnnotations;

namespace CompanyHub.Models;

public class Service : BaseEntity
{
    [Required(ErrorMessage = "Le nom du service est requis.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom du service doit contenir entre 2 et 50 caractères.")]
    public string Name { get; set; }

    public List<Employee>? Employees { get; set; }
}