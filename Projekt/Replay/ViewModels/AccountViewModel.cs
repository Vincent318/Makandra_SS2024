using System.ComponentModel.DataAnnotations;

namespace Replay.ViewModels
{
    public class AccountViewModel
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        
        public string[]? Roles { get; set; }
        public string[]? Departments{ get; set; }

    }
}