using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Replay.ViewModels.RoleViewModels
{
    public class EditRoleViewModel
    {

        [Required]
        public string Id{get; set;}
        
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}