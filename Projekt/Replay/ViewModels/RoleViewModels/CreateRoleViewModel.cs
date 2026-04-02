using System.ComponentModel.DataAnnotations;


namespace Replay.ViewModels.RoleViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

    }
}