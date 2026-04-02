//  @author: Daniel Feustel
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Replay.Models;
using Replay.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Replay.Controllers;
/// <summary>
///  The Initial Controller handles requests for the initial start of the application, if no data exist already
/// </summary>
/// <author>Daniel Feustel</author>
public class InitialController : Controller
{   
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    

    public InitialController(UserManager<User> userManager, SignInManager<User> signInManager,RoleManager<Role> roleManager)
    {   
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }
    /// <summary>Returns the initial view</summary>
    [AllowAnonymous]
    public IActionResult Index()
    {   
        return View();
    }

    /// <summary>handles the initial registration</summary>
    /// <param name="model">contains data, required to create the initial user</param>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateInitialUser(InitialRegisterViewModel model) {
       
        if (ModelState.IsValid) {
            if (model.Roles != null) {
                List<string> tmp = model.Roles.ToList();
                tmp.Add("Administrator");
                model.Roles = tmp.ToArray();
            } else {
                string[] tmp = {"Administrator"};
                model.Roles = tmp;
            }
            
            var user = new User {Email = model.Email, UserName = model.Email, FirstName = model.FirstName, LastName = model.LastName, Departments = model.Departments ?? Array.Empty<string>(), Roles = model.Roles ?? Array.Empty<string>()};
            var res = await _userManager.CreateAsync(user, model.Password);
            
            if (res.Succeeded) {
                if (model.Roles != null)
                await _userManager.AddToRolesAsync(user, model.Roles);
                
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                
                await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                return LocalRedirect("/");
            }
            
        }

        return RedirectToAction("Index");
    }

}