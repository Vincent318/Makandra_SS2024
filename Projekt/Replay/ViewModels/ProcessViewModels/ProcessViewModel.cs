// @author: Vincent Arnold
using System.Collections.Generic;
using Replay.Models;

namespace Replay.ViewModels.ProcessViewModels
{
    public class ProcessViewModel
    {
        // public string title { get; set; }
        // public string description { get; set; }
        // public List<string> authorizedRoles { get; set; }
        // public List<TaskBluePrint> tasks { get; set; }

        // public CreateProcessViewModel()
        // {
        //     authorizedRoles = new List<string>();
        //     tasks = new List<TaskBluePrint>();
        // }

        public List<Process>? Processes {get; set;}
    }

}