using Microsoft.EntityFrameworkCore;
using Replay.Services;
using System.ComponentModel.DataAnnotations;
using Replay.Enumerations;
namespace Replay.ViewModels;
public class LoginViewModel
{
    [Required(ErrorMessage = "Dieses Feld muss ausgefüllt sein")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Eine gültige E-Mail-Adresse muss angegeben werden")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Dieses Feld muss ausgefüllt sein")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
