using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CompanyHub.Models;

public class PasswordModification
{
    [Required(ErrorMessage = "L'ancien mot de passe est requis.")]
    [DataType(DataType.Password)]
    [DisplayName("Mot de passe")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Le nouveau mot de passe est requis.")]
    [DataType(DataType.Password)]
    [DisplayName("Mot de passe")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Le nouveau mot de passe est requis.")]
    [DataType(DataType.Password)]
    [DisplayName("Mot de passe")]
    [Compare("NewPassword", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string ConfirmPassword { get; set; }
}