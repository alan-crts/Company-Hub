using System.ComponentModel.DataAnnotations;

namespace CompanyHub.Models;

public class Site : BaseEntity
{
    [Required(ErrorMessage = "Le nom du site est requis.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom du site doit contenir entre 2 et 50 caractères.")]
    public string City { get; set; }

    [Required(ErrorMessage = "La description du site est requise.")]
    [StringLength(500, MinimumLength = 2,
        ErrorMessage = "La description du site doit contenir entre 2 et 500 caractères.")]
    public string Description { get; set; }

    public List<Employee>? Employees { get; set; }
}