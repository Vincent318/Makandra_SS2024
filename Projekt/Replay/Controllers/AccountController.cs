// @author: Daniel Feustel

using Microsoft.AspNetCore.Mvc;
using Replay.Models;
using Replay.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Replay.Repositories;
using Replay.Services;
using Microsoft.AspNetCore.Mvc.ViewComponents;


namespace Replay.Controllers;

/// <summary>
///  The account controller accepts inquiries regarding account-specific matters
/// </summary>
/// <author>Daniel Feustel</author>
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SecurityRepository _securityRepository;
    private readonly EmailSenderService _emailSender;

    /// <param name="userManager">Manages all users of the Application</param>
    /// <param name="signInManager">Manages all signIn specific logic</param>
    /// <param name="roleManager">Manages all Roles of the Application</param>
    /// <param name="repo">Stores key-value pairs for Login-specific matters</param>
    /// <param name="emailSender">The service for sending emails</param>
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,RoleManager<Role> roleManager, SecurityRepository repo, EmailSenderService emailSender) {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _securityRepository = repo;
        _emailSender = emailSender;
    }
    /// <summary>Returns Account-information of the current logged-in user</summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index() 
    {       
        var user = await _userManager.GetUserAsync(User);
        if (user != null) {
            var model = new AccountViewModel() {FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Roles = user.Roles, Departments = user.Departments};
            return View(model);
        }
        return View();
    }  

    /// <summary>Returns Login Page</summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    
    /// <summary>Handles User Login</summary>
    /// <param name="model">Contains required information for the login-process</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model) {
        if (ModelState.IsValid) {
            var res = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);
            if (res.Succeeded) {
                
                return LocalRedirect("/");
            }
             
            if (res.IsLockedOut) {
                return View("/Account/Login");
            }
            else {
                
                Console.WriteLine("WTF");
                return View(model);
            }
        }
        return View(model);
    }

    /// <summary>Handles User-Logout</summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }


    [HttpGet]
    public IActionResult AccessDenied() {
        return View();
    }


    /// <summary>returns the View for the initial password</summary>
    /// <param name="id">Guid for double check</param>
    [HttpGet]
    public async Task<IActionResult> InitialPassword(string id) {
        
        
        try {
            Guid guid = Guid.Parse(id);
            SecurityModel model = await _securityRepository.Read(guid); 
            InitialPasswordViewModel initial = new() {Email = model.Email};
            return View(initial);
        } catch (Exception e) {
            Console.WriteLine(e);
            return LocalRedirect("/Account/AccessDenied");
        }
    }

    /// <summary>Handles Initialpassword Action</summary>
    /// <param name="model">contains required information for intialpassword</param>
    [HttpPost]
    public async Task<IActionResult> InitialPassword(InitialPasswordViewModel model) {
        if (ModelState.IsValid) {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (await _userManager.IsEmailConfirmedAsync(user))
                return LocalRedirect("Login");
            if (user != null) {
                await _userManager.AddPasswordAsync(user, model.Password);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                _securityRepository.Delete(model.Email);
            }   
            return RedirectToAction("Login");
        }
        return LocalRedirect("/Error");
    }

    /// <summary>Returns ForgotPasswrod View</summary>
    [HttpGet]
    public IActionResult ForgotPassword() {
        return View();
    }
    
    /// <summary>Returns View for ResetPassword</summary>
    /// <param name="id">Guid for double check</param>
    [HttpGet]
    public async Task<IActionResult> ResetPassword(string id) {

        try {
            Guid parsedId = Guid.Parse(id);
            SecurityModel model = await _securityRepository.Read(parsedId);
            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            ResetPasswordViewModel resetModel = new() {VerificationToken = token, Email = user.Email};
            return View(resetModel);
        }catch (Exception e) {
            Console.WriteLine(e);
            return LocalRedirect("/Account/AccessDenied");
        }
    }

    /// <summary>Handles ResetPassword request</summary>
    /// <param name="model">contains required information for resetpassword</param>
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) {

        if (ModelState.IsValid) {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null) {
                var res = await _userManager.ResetPasswordAsync(user, model.VerificationToken, model.NewPassword);
                if (res.Succeeded)
                    _securityRepository.Delete(model.Email);
            }
            return RedirectToAction("Login");
        }
        Console.WriteLine("WTF");
        return LocalRedirect("/Error");
    }

}
