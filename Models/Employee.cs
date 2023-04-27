using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.Models;

public class Employee : BaseEntity
{
    [Required(ErrorMessage = "Le prénom est requis.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le prénom doit contenir entre 2 et 50 caractères.")]
    [DisplayName("Prénom")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom de famille est requis.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Le nom de famille doit contenir entre 2 et 50 caractères.")]
    [DisplayName("Nom")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "L'adresse email est requise.")]
    [EmailAddress(ErrorMessage = "L'adresse email est invalide.")]
    [DisplayName("Adresse email")]
    [Remote("CheckEmail", "Employee", AdditionalFields = "Id", ErrorMessage = "L'adresse email est déjà utilisée.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Le numéro de téléphone fixe est requis.")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Le numéro de téléphone fixe est invalide.")]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Le numéro de téléphone fixe est invalide.")]
    [DisplayName("Téléphone fixe")]
    public string LandlinePhone { get; set; }

    [Required(ErrorMessage = "Le numéro de téléphone mobile est requis.")]
    [DataType(DataType.PhoneNumber, ErrorMessage = "Le numéro de téléphone mobile est invalide.")]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Le numéro de téléphone mobile est invalide.")]
    [DisplayName("Téléphone mobile")]
    public string MobilePhone { get; set; }

    [DataType(DataType.Password)] public string? Password { get; set; }

    [DisplayName("Administrateur")] public bool IsAdmin { get; set; }

    [DisplayName("Service")] public Service? Service { get; set; }

    public int ServiceId { get; set; }

    [DisplayName("Site")] public Site? Site { get; set; }

    public int SiteId { get; set; }
}