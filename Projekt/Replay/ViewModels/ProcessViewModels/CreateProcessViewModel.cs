// @author: Vincent Arnold
using System.ComponentModel.DataAnnotations;
using Replay.Models;

namespace Replay.ViewModels.ProcessViewModels;

public class CreateProcessViewModel {

        [Required(ErrorMessage = "Dieses Feld muss ausgefüllt sein")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Dieses Feld muss ausgefüllt sein")]
        public string? Description { get; set; }
        
        public string[]? Roles {get; set;}
        public string[]? ChoosedRoles {get; set;}

}