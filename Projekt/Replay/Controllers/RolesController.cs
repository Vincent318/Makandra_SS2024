using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Replay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Replay.ViewModels;
using Replay.ViewModels.RoleViewModels;


namespace Replay.Controllers{

    /// <summary>
    /// The RolesController manages role-related Operations in the program.
    /// It enables the creation, the editing and deleting of roles in the programm.
    /// These Methods in this Controller and its Views are only accessable by users with the role "Administrator"
    /// </summary>
    /// <author> Robert Figl </author>
    public class RolesController : Controller
    {

        // Role Manager is part from ASP.NET Core Identity
        // useful for managment of UserRoles
        // - offers different methods for Roles:
        //      creating:   CreateAsync
        //      editing:    UpdateAsync
        //      deleting:   DeleteAsync
        //      getting:    FindByIdAsync, FindByNameAsync

        private readonly RoleManager<Role> _roleManager;

        private readonly ILogger<RolesController> _logger;

        public RolesController(ILogger<RolesController> logger, RoleManager<Role> roleManager)
        {   
            _logger = logger;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Display the roles index page.
        /// Only accessible to a user with a Adminstrator role.
        /// </summary>
        /// <returns>The index view.</returns>
        /// <author> Robert Figl </author>
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {   
            return View();
        }

        /// <summary>
        /// Displays the create role page.
        /// Only visible to users with the Adminstrator role.
        /// </summary>
        /// <returns>The create role view.</returns>
        /// <auhtor> Robert Figl </author>
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        /// <summary>
        /// Handles the creation of a new role.
        /// </summary>
        /// <param name="model">The create role viwe model, containing role details</param>
        /// <returns>When a role is succesfully created, a redirect to the role index view is returned
        /// Else it stays in the create Role View to enable another try for creating a role.</returns>
        /// <author> Robert Figl </author>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model){
            // Check if a role with the same title already exists
            var existingRole = await _roleManager.FindByNameAsync(model.Title);
            if (existingRole != null)
            {
                // throw error
                ModelState.AddModelError("Title", "Eine Rolle mit diesem Titel existiert bereits");
                // throw new InvalidOperationException($"A role with this title already exists:  {existingRole.Title}!! ");
            }

            if(ModelState.IsValid)
            {
                try
                {
                    Role role = new Role {Name = model.Title, Description = model.Description, Title = model.Title};
                    var res = await _roleManager.CreateAsync(role);
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Fehler beim Erstelen der Rolle");
                    ModelState.AddModelError("", "Fehler beim Erstellen der Rolle. Bitte versuchen Sie es erneut.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Handles the Http Get Request for editing a role.
        /// The values of the role a saved in EditRoleView model, 
        /// to make its values available, in the Http Post editRole method
        /// </summary>
        /// <param name="title">Title is the name of the role. There could be confusion because
        /// in IdentityRole from which our Roles inherit, is a Attribute which is called "name", and this
        /// attribute has functionality like finding methods. Title is most often used as an equivalent to name, 
        /// althoudh we could have just used the predefined name attribute instead of title</param>
        /// <returns>Returns the edit role view model with the roles`s details</returns>
        /// <author> Robert Figl </author>
        [HttpGet]   
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditRole(string title)
        {
            // getting the role via roleManager 
            var role = await _roleManager.FindByNameAsync(title);
            if (role == null)
            {
                return NotFound();
            }
            // saving the attributes of the role in a viewModel
            var model = new EditRoleViewModel{
              
                Title = role.Title, 
                Description = role.Description
            };
            
            // returning to Index View
            return View(model);
        }
        
        /// <summary>
        /// Handles the Http Post Request for editing a role.
        /// Acutally not in the customer requirements, so not important
        /// </summary>
        /// <param name="model">The ViewModel for Editing Roles has Title and Description as Attribute</param>
        /// <returns>Returns a Task which handles the asyncronous operation and directs the user to the index view</returns>
        /// <author> Robert Figl </author>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if(ModelState.IsValid)
            {
                // getting the role
                var role = await _roleManager.FindByIdAsync(model.Id);

                if (role == null)
                {
                    return NotFound();
                }

                // the role gets its new attributes from the viewModel
                role.Name = model.Title;
                role.Title = model.Title;
                role.Description = model.Description;

                // updating the role via roleManager method
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return View(Index);
                }
                // Exception handling in case something went wrong
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    Console.WriteLine("Fehler beim Aktualisieren der Rolle:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    // return View(model)
                    return View(Index);
                }
            } 
            else
            {
                // if modelState is not valid, error messages are printed in the console 
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Property: {state.Key} Error: {error.ErrorMessage}");
                        Console.WriteLine(model.Description);
                        Console.WriteLine(model.Title);
                    }
                }
            // returning to index view
            return View(Index);
            }
        }

        /// <summary>
        /// Handles the deletion of a role
        /// Not in the requirements, so not important
        /// </summary>
        /// <param name="title">the title attribute serves as an idetifier for getting the role</param>
        /// <returns>Redirects to the index view after deletion</returns>
        /// <exception cref="InvalidOperationException">Maybe thrwoing an exception is too strong, 
        /// but the admin role should never be deleted</exception>
        /// <author>Robert Figl</author>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteRole(string title){

            Role role = await _roleManager.FindByNameAsync(title);
            
            if (role.Title == "Administrator")
            {
                throw new InvalidOperationException("The Administrator role cannot be deleted.");
            }
            
            var rle = await _roleManager.DeleteAsync(role);

            return RedirectToAction("Index");
        }
    }
}