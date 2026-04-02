using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Replay.ViewModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Replay.Models
{
    /// <summary>
    /// Represents the role in the application, which inherits from IdentityRole
    /// </summary>
    /// <author> Robert Figl </author>
    public class Role : IdentityRole
    {   
        public string? Title {get; set;}
        public string? Description {get; set;}

        public string GetName() {
            return (Title is null) ? "noch keinen Titel hinzugefügt" : Title;
        }

        public string GetType() {
            return "Role";
        }
    }
}