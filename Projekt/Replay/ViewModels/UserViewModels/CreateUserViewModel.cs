using System.ComponentModel.DataAnnotations;

namespace Replay.ViewModels.UserViewModels
{
    public class CreateUserViewModel
    {
        
        [Display(Name = "Vorname")]
        public string? FirstName { get; set; }

       
        [Display(Name = "Nachname")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        
        public string[]? Departments {get; set;}
        public string[]? Roles {get; set;}

        [Required]
        public bool IsLocked {get; set;}
    }
}