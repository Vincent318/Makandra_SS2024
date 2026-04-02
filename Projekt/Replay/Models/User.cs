using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Replay.Enumerations;

namespace Replay.Models
{
    // User class inherits from IdentityUser 
    //     - IdentityUser is from Microsoft.AspNetCore.Identity (look at wiki (to be written))
    //     ---> inheriting from IdentityUser keeps the functionality of the IdentityUser class, 
    //          and makes needed changes possible (like adding roles)

    /// <summary>
    /// Represents a user in the application, inherits from IdentityUser to utilize Identity functionality
    /// </summary>
    /// <author> Robert Figl </author>
    public class User : IdentityUser
    {
        public string? FirstName {get; set;}
        public string? LastName {get; set;}
        // --------- Database saves other entities as strings -------------------

        // this saves the different other classes in the user class
        // saving the different classes as a long string for each class, with the different instances, seperated with commata
        // is efficient and evades different complicated database problems
        // ----> easier to save a string in a database not an array
        public string? RolesString {get; set;}
        public string? DepartmentsString {get; set;}
        
        public int TasksFinished {get; set;}

        // -------- Converting database string into an Array ---------
    
        // NotMapped ---> used in EntityFrameworkCore to signalize that this attribute should not be safed in the database (strings above should be saved)
        // getter -> converts the String from the database into an array
        // setter -> when saving, the array should be converted into a string and saved into the database

        /// <summary>
        /// Converting database string
        /// </summary>
        /// <author> Daniel Feustel </author>
        [NotMapped]
        public string[] Departments {
        //  getter:    converts the DeparmentsString from the databse into an Array
        get => DepartmentsString?.Split(',') ?? Array.Empty<string>();
        //  setter:    converts the Array into a string and saves it in the database
        set => DepartmentsString = string.Join(',', value);
        }

        /// <summary>
        /// Converting database string
        /// </summary>
        /// <author> Daniel Feustel </author>
        [NotMapped]
        public string[] Roles {
            get => RolesString?.Split(',') ?? Array.Empty<string>();
            set => RolesString = string.Join(',', value);
        }

        public string GetName() {
            string first = FirstName is null ? "-" : FirstName;
            string last = LastName is null ? "-" : LastName;
            return first + " " + last;
        }

        public string GetType() {
            return "User";
        }

        /// <summary>
        /// Method for checking if the user is lockedOut
        /// </summary>
        /// <author> Robert Figl </author>
        public bool IsLockedOut
        {
        get
            {
                return LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
            }
        }
    }
}