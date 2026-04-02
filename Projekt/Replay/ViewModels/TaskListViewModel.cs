// author: Noah Engelschall

using System.Collections.Generic;
using Replay.Models;

namespace Replay.ViewModels
{
    public class TaskListViewModel
    {
        public string ViewType { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
        public string FilterType { get; set; } = string.Empty;
        public List<ActiveTask> Tasks { get; set; } = new List<ActiveTask>();
    }
}
