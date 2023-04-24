using System.ComponentModel.DataAnnotations;

namespace CompanyHub.Models;

public class Login
{
    [Required]
    [EmailAddress(ErrorMessage = "L'adresse email ou le mot de passe est incorrect")]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}