using System;
using System.ComponentModel.DataAnnotations;

namespace Replay.Models
{
        /// <summary>
        /// Represents a task in the task list.
        /// </summary>
        /// <author>Noah Engelschall</author>
        public class TaskList
        {
                /// <summary>
                /// Gets or sets the primary key of the task.
                /// </summary>
                /// <author>Noah Engelschall</author>
                public int Id { get; set; }

                /// <summary>
                /// Gets or sets the description of the task.
                /// </summary>
                /// <author>Noah Engelschall</author>
                public string Aufgabe { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets the process to which the task is assigned.
                /// </summary>
                /// <author>Noah Engelschall</author>
                public string Vorgang { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets the due date of the task.
                /// </summary>
                /// <author>Noah Engelschall</author>
                public DateTime Fällig { get; set; }

                /// <summary>
                /// Gets or sets the status of the task.
                /// </summary>
                /// <author>Noah Engelschall</author>
                public string Status { get; set; } = string.Empty;

                /// <summary>
                /// Gets or sets a value indicating whether the task is claimed.
                /// </summary>
                /// <author>Noah Engelschall</author>
                public bool IsClaimed { get; set; }
        }
}
