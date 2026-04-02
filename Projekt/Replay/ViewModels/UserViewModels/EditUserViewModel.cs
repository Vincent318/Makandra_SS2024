using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Replay.ViewModels;

namespace Replay.ViewModels.UserViewModels
{
    public class EditUserViewModel
    {

        public string Id { get; set; }

        
        [Display(Name = "Vorname")]
        public string? FirstName{get; set;}

       
        [Display(Name = "Nachname")]
        public string? LastName{get; set;}

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email{get; set;}

        public string[] Departments { get; set; } = Array.Empty<string>() ; 

        public string[] Roles {get; set;} = Array.Empty<string>() ;
       
        [Display(Name = "Gesperrt")]
        public bool IsLocked{get; set;}


    }



}