using System.ComponentModel.DataAnnotations;
namespace Replay.ViewModels;

public class ResetPasswordViewModel {

    [DataType(DataType.EmailAddress)]
    [Required]
    public string? Email {get; set;}

    [DataType(DataType.Password)]
    [Required]
    public string? NewPassword {get; set;}

    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    [Required]
    public string? ConfirmPassword {get; set;}

    [Required]
    public string? VerificationToken {get; set;}

}