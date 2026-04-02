using Microsoft.AspNetCore.Mvc;
using Replay.Models;
using Replay.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Replay.Services;
using Replay.Data;
using Replay.Repositories;
using Replay.ViewModels.UserViewModels;

namespace Replay.Controllers
{

    /// <summary>
    /// The UserController manages Users.
    /// </summary>
    /// <author>Daniel Feustel, Robert Figl, Noah Engelschall </author>
    public class UsersController : Controller
    {   
        // User Manager is part of ASP.NET Core Identity
        // It provides functionality for managing user accounts.
        // The UserManager class offers a variety of methods for:
        //      - Creating new users: CreateAsync
        //      - Updating user information: UpdateAsync
        //      - Deleting users: DeleteAsync
        //      - Finding users: FindByIdAsync, FindByNameAsync
        //      - Locking/unlocking accounts: SetLockoutEndDateAsync, SetLockoutEnabledAsync      
        private readonly UserManager<User> _userManager;

        // used for log in 
        private readonly SignInManager<User> _signInManager;

        // used for configuration
        private readonly IConfiguration _conf;

        // for sending Emails
        private readonly EmailSenderService _emailSender;

        // Securtiy
        private readonly SecurityRepository _securityRepository;
        
        /// <summary>
        ///  Constructor enables the userController to have access to nedded Services
        /// </summary>
        /// <param name="conf">for config</param>
        /// <param name="userManager">management of users</param>
        /// <param name="signInManager">for sign in</param>
        /// <param name="emailSenderService">for email</param>
        /// <param name="repo">for security</param>
        public UsersController(IConfiguration conf, UserManager<User> userManager, SignInManager<User> signInManager, EmailSenderService emailSenderService, SecurityRepository repo)
        {
            // SignInManager is used for sign in operations
            _signInManager = signInManager;
            
            // Management of users
            _userManager = userManager;
            
            // gets configuration values from different sources (f.e. appsettings.json)
            // for example email configurations
            _conf = conf;
            
            // sends Emails like Password reset or InitialPassword Email
            _emailSender = emailSenderService;
            
            // Manages security related information like authorizations
            _securityRepository = repo;
        }

        /// <summary>
        /// Only Administrator can see the index view of user management.
        /// On this view the registrered users, and possibilities 
        /// of managing them are shwon.
        /// </summary>
        /// <returns>the View</returns>
        /// <author> Robert Figl </author>
        [Authorize(Roles = "Administrator")]
        public IActionResult Index()
        {       

            return View();
        }

        /// <summary>
        /// This method Deletes a user.
        /// </summary>
        /// <param name="id">Id of the user, for identitfying the user</param>
        /// <returns>Redirect to Index View</returns>
        /// <author> Robert Figl </author>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(string id) {
            
            // gets the user
            User usr = await _userManager.FindByIdAsync(id);
            
            // Deletes the user
            var res = await _userManager.DeleteAsync(usr);
            
            // if successfull redirect to index
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the create user page.
        /// Only visible to users with the Administrator role.
        /// </summary>
        /// <returns>The create user view</returns>
        /// <author>Robert Figl</author>
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult CreateUser() {
            return View();
        }

        /// <summary>
        /// Handles the creation of user.
        /// </summary>
        /// <param name="model">The create user view mode, containing user details.</param>
        /// <returns>When a user is succesfully created, a redirect to the user index view is returned
        /// Else it stays in thee create User View to enable another try for creating a user</returns>
        /// <author> Robert Figl <author>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model) {

            if (ModelState.IsValid) {

                User newUser = new User
                {
                    FirstName = model.FirstName, 
                    LastName = model.LastName, 
                    Email = model.Email, 
                    Roles = model.Roles ?? Array.Empty<string>(), 
                    Departments = model.Departments ?? Array.Empty<string>(), 
                    UserName = model.Email};
                
                var res = await _userManager.CreateAsync(newUser);  

                if (res.Succeeded) {
                    // Check if roles are not empty before adding roles
                    if (model.Roles != null ) {
                        await _userManager.AddToRolesAsync(newUser, model.Roles);
                    }
                    if (model.Departments == null)
                    {
                        model.Departments = Array.Empty<string>();
                    }
                    if (model.Roles == null)
                    {
                        model.Roles = Array.Empty<string>();
                    }
            
                    // Check if roles are not empty before adding roles
                    if (model.Roles != null  /*&& model.Roles.Any() */) {
                        await _userManager.AddToRolesAsync(newUser, model.Roles);
                    }

                    // Setting the user locked when IsLocked attribute is true
                    if (model.IsLocked) {
                        await _userManager.SetLockoutEndDateAsync(newUser, DateTimeOffset.MaxValue);
                        await _userManager.SetLockoutEnabledAsync(newUser, true);
                    }
                   
                    // Create a new security model for the user
                    SecurityModel securityModel = new() {Id = Guid.NewGuid(), Email = model.Email};
                    // send an InitialPassword email
                    var sent =  _emailSender.SendInitialPasswordEmail(_conf, securityModel, model.FirstName);
                    // checking if the email was sent sorrectly
                    if (!sent) {
                        Console.WriteLine("Email not sent something went wrong");
                    } else {
                        _securityRepository.Create(securityModel);
                    }
                }
            }   
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Hadnles the passowrd reset request
        /// </summary>
        /// <param name="model">the View which contains the email of the user</param>
        /// <returns>A redirect to the index View</returns>
        /// <author> Daniel Feustel <author>
        [HttpPost] 
        public async Task<IActionResult> RequestPasswordReset(RequestPasswordResetViewModel model) {
            Console.WriteLine("REQUST");
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null) 
            {
                // Create a new security model
                SecurityModel securityModel = new(){Email = model.Email, Id = Guid.NewGuid()};
                // sending a PasswordResetEmail
                var sent = _emailSender.SendResetPasswordEmail(_conf, securityModel, user.FirstName);
                // checking if the email was sent correctly
                if (!sent) 
                {
                    Console.WriteLine("Email not sent something went wrong");
                }
                else
                {
                    // Save the securityModel to the repository
                    _securityRepository.Create(securityModel);
                }
            } 
            else 
            {
            Console.WriteLine("USER NOT FOUND");
            }
            // Redirect to the Index action
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles the HTTP GET Request for editing a user.
        /// The values of the user are loaded in the view for editing the user.
        /// </summary>
        /// <param name="id"> "id" is the id of the user to edit. Id is an attribute of IdentityUser (ASP NET CORE Identity Framework) 
        /// so we do not find it in our model 
        /// </param>
        /// <returns>Task of the type IActionResult. This handles the asyncronous operation and directs the user to the edit user view </returns>
        /// <author> Robert Figl </author>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditUser(string id)
        {
            // getting the user to edit with the ASP NET CORE Identity method
            var user = await _userManager.FindByIdAsync(id);
            // handling when user is not found
            if(user == null)
            {
                Console.WriteLine($"User with id {id} not found");
                return NotFound();
            }
            // the view model gets the current attributes of the user
            var viewModel = new EditUserViewModel
            {
                // Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles,
                Departments = user.Departments,
                IsLocked = await _userManager.IsLockedOutAsync(user)
            };    
            // directs to the editUserView
            return View(viewModel);
        }

        /// <summary>
        /// Handles the Http Post Request for editing a User.
        /// Saves the attributes of the EditUserViewModel in the user.
        /// </summary>
        /// <param name="model">The View Model for editing Users has the attributes 
        /// FirstName, Lastame, Email, Departments, Roles, IsLocked</param>
        /// <returns>A Redirect to the index View if everyting went right</returns>
        /// <author> Robert Figl </author>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditUser (EditUserViewModel model)        
        {
            if (model == null)
            {
                return BadRequest("The model is null");
            }
            if(ModelState.IsValid)
            {
                // gets the user from the database through ASP NET CORE Identity userManager methods
                var user = await _userManager.FindByIdAsync(model.Id);

                if(user == null)
                {
                    return NotFound();
                }

                Console.WriteLine("TestBeforeAssigning");

                if (model.Departments == null)
                {
                    model.Departments = Array.Empty<string>();
                }
                if (model.Roles == null)
                {
                    model.Roles = Array.Empty<string>();
                }

                // the User gets its new Attributes from the ViewModel
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Departments = model.Departments;
                user.Roles = model.Roles;

                // Handling the lockout functionality for user accounts using ASP.NET Core Identity
                // This code checks the `IsLocked` property from the view model to determine whether the user should be locked out or not.

                // If the `IsLocked` property is true, it means the user should be locked out.
                if (model.IsLocked)
                {
                    // Set the lockout end date to the maximum value, effectively locking the user out indefinitely.
                    // This means the user will not be able to log in until the lockout end date is changed or removed.
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                   
                    // Enable lockout functionality for the user.
                    // This ensures that the lockout will be enforced and that the user will be locked out as specified.
                    await _userManager.SetLockoutEnabledAsync(user, true);
                } 
                else 
                {   
                    // If the `IsLocked` property is false, it means the user should not be locked out.
    
                    // Set the lockout end date to null, which means there is no lockout in effect.
                    // The user will be able to log in without restrictions since there is no lockout date set.
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    
                    // Disable the lockout functionality for the user.
                    // This ensures that the user is not locked out and can log in normally.
                    // await _userManager.SetLockoutEnabledAsync(user, false);
                    // because of this line u have to lock the user two times, 
                    // one time to enable the possibiliy of locking out and another time for locking the user
                    // there is no need to disable the lockout functionality
                }

                // Updating the user 
                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    Console.Write("Mit dem Update hat alles geklappt! Gute Arbeit!!!");
                    return RedirectToAction("Index");
                }
                foreach(var error in result.Errors)
                {
                    Console.Write("Fehler beim Update des Users!!!");
                    ModelState.AddModelError("", error.Description);
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
                    }
                }
            }
            // redirection to the View 
            return View(model);
        }
    }
}