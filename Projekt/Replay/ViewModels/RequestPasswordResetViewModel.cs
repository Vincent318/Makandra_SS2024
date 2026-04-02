using System.ComponentModel.DataAnnotations;
namespace Replay.ViewModels;

public class RequestPasswordResetViewModel {

    [DataType(DataType.EmailAddress)]
    [Required]
    public string? Email {get; set;}
    
}