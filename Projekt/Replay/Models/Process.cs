// @author: Vincent Arnold
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Replay.ViewModels;

namespace Replay.Models
{
    /// <summary>
    /// Represents a process with associated tasks and properties.
    /// </summary>
    public class Process
    {
        // Primary key for the Process entity
        public int Id { get; set; } 
        
        // Title of the process, required field
        [Required]
        public string? Title { get; set; }
        
        // Description of the process, required field
        [Required]
        public string? Description { get; set; }
        
        // Roles authorized to interact with this process, stored as a comma-separated string
        public string? AuthorizedRolesString { get; set; }
        
        // Type of contract associated with the process
        public string? ContractType { get; set; } 
        
        // Department responsible for the process
        public string? Department { get; set; }
        
        // List of tasks associated with this process
        public List<TaskBluePrint> Tasks { get; set; } = new List<TaskBluePrint>();

        // Roles authorized to interact with this process, handled as an array
        [NotMapped]
        [Required]
        public string[] AuthorizedRoles {
            get => AuthorizedRolesString?.Split(',') ?? Array.Empty<string>();
            set => AuthorizedRolesString = string.Join(',', value);
        }

        // Number of tasks associated with this process
        [NotMapped]
        public int TaskCount => Tasks.Count; // New property for the count of tasks
    }
}