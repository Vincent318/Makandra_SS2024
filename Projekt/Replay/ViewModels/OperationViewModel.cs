// @author: Raphael Huber


using Replay.Models;

namespace Replay.ViewModels
{
    /// <summary>This class is used for getting information to the Operation-List<summary>
    /// <author>Raphael Huber</author>
    public class OperationViewModel
    {
        /// <summary>All operations<summary>
        /// <author>Raphael Huber</author>
        public List<Operation>? Operations{get; set;}
        /// <summary>The currently logged-in user. Used for determining the posibble actions<summary>
        /// <author>Raphael Huber</author>
        public User? CurrentUser {get; set;}
        /// <summary>A list of all users the program has<summary>
        /// <author>Raphael Huber</author>
        public List<User>? Users {get; set;}
        /// <summary>A list of all roles<summary>
        /// <author>Raphael Huber</author>
        public List<Role>? Roles {get; set;}
        /// <summary>Determines if you want to see all, only archived or only active operations<summary>
        /// <author>Raphael Huber</author>
        public int SelectedViewOption {get; set;}

    }


}