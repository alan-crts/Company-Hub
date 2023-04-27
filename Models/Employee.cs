using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Models;

public class Employee : BaseEntity
{
    [Required(ErrorMessage = "Le prénom est requis.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom de famille est requis.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom de famille doit contenir entre 2 et 50 caractères.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "L'adresse email est requise.")]
    [EmailAddress(ErrorMessage = "L'adresse email est invalide.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le numéro de téléphone fixe est requis.")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Le numéro de téléphone fixe est invalide.")]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Le numéro de téléphone fixe est invalide.")]
    public string LandlinePhone { get; set; }

    [Required(ErrorMessage = "Le numéro de téléphone mobile est requis.")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Le numéro de téléphone mobile est invalide.")]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Le numéro de téléphone mobile est invalide.")]
    public string MobilePhone { get; set; }

    public string? Password { get; set; }

    public bool IsAdmin { get; set; }
    
    public Service? Service { get; set; }
    public int ServiceId { get; set; }
    
    public Site? Site { get; set; }
    public int SiteId { get; set; }
}