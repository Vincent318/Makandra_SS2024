using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Replay.Models;
using Replay.ViewModels;
using Replay.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace Replay.Controllers;

/// <summary>
/// The Controller for the Operation-View that handles the Input/Output to the View
/// </summary>
/// <author>Raphael Huber</author>
public class OperationsController : Controller
{
    /// <summary>An operation-repository that is used to read/change operations</summary>
    /// <author>Raphael Huber</author>
    private readonly OperationRepository _operationRep;
    /// <summary>The user-manager that is used to get the current user and all user available</summary>
    /// <author>Raphael Huber</author>
    private readonly UserManager<User> _userManager;
    /// <summary>The role-manager this is used to get all roles<summary>
    /// <author>Raphael Huber</author>
    private readonly RoleManager<Role> _roleManager;
    private readonly UserRepository _userRepository;

    /// <summary>
    /// Sets all class members using dependency-injection
    /// </summary>
    /// <param name="repo">value for _operationRepository</param>
    /// <param name="userManager">value for _userManager</param>
    /// <param name="roleManager">value for _roleManager</param>
    /// <author>Raphael Huber</author>

    public OperationsController(OperationRepository repo, UserManager<User> userManager, RoleManager<Role> roleManager, UserRepository userRepository)
    {
        _operationRep = repo;
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
    }


    /// <summary>
    /// Will be called whenever the Operation-View is called
    /// </summary>
    /// <param name="selectedOption">In the view you can choose between viewing all operations (=1), only active ones (=2) and only archived ones (=3). 
    /// What is viewed can be changed with this par. The default is 2 </param>
    /// <returns>Returns the view with the correct ViewModel</returns>
    /// <author>Raphael Huber</author>
    [Authorize]
    public async Task<IActionResult> Index(int selectedOption = 2)
    {
        List<Operation> operations = new();
        switch (selectedOption)
        {
            case 1:
                operations = _operationRep.ReadAll();
                break;
            case 2:
                operations = _operationRep.ReadActive();
                break;
            case 3:
                operations = _operationRep.ReadArchived();
                break;
        }
        foreach (var op in operations)
        {
            if (!string.IsNullOrEmpty(op.PersonInChargeMail))
            {
                var user = await _userManager.FindByEmailAsync(op.PersonInChargeMail);
                if(user != null && !user.IsLockedOut)
                {
                    op.PersonInCharge = user;
                }
                else
                {
                    Console.WriteLine($"PersonInChargeMail is invalid or user is locked out for operation {op.Id}");
                    op.PersonInCharge = null; // Set to null to display "Unbekannter Benutzer" in the view
                }
                // op.PersonInCharge = await _userManager.FindByEmailAsync(op.PersonInChargeMail);
            }
            else
            {
                Console.WriteLine($"PersonInChargeMail is null or empty for operation {op.Id}");
            }

            if (!string.IsNullOrEmpty(op.ReferencePersonMail))
            {
                var user = await _userManager.FindByEmailAsync(op.ReferencePersonMail);
                if(user != null && !user.IsLockedOut)
                {
                    op.ReferencePerson = user;
                }
                else
                {   
                    Console.WriteLine($"ReferencePersonMail is invalid or user is locked");
                    op.ReferencePerson = null;
                }
                // op.ReferencePerson = await _userManager.FindByEmailAsync(op.ReferencePersonMail);
            }
            else
            {
                Console.WriteLine($"ReferencePersonMail is null or empty for operation {op.Id}");
            }    
        }

         // Verwende die UserRepository Methode, um die aktiven Benutzer zu erhalten
        Console.WriteLine($"11operationView.Users         Totoro" );

        OperationViewModel operationView = new() { 
            Operations = operations,

            // Users = _userManager.Users.ToList(),
            
            Users = await _userRepository.GetActiveUsersAsync(),

            //hier muss ich doch jetzt die user repository methode nutzen
            Roles = _roleManager.Roles.ToList(),
            SelectedViewOption= selectedOption,
            CurrentUser = await _userManager.GetUserAsync(User)
        };
        Console.WriteLine($"operationView.Users         Totoro" );

        // Hier die Benutzer in operationView.Users ausgeben
        foreach (var user in operationView.Users)
        {
            Console.WriteLine($"User: {user.UserName}, LockoutEnabled: {user.LockoutEnabled}, LockoutEnd: {user.LockoutEnd}");
        }

        return View(operationView);
    }

    /// <summary>
    /// Archives the operation with the id and returns to Index
    /// </summary>
    /// <param name="id">the id of the operation that should be archived</param>
    /// <returns>return the redirection to the index</returns>
    /// <author>Raphael Huber</author>

    [HttpPost]
    public IActionResult ArchiveOperation(int id)
    {
        _operationRep.ArchiveOperation(id);
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Saves the new target date of the task
    /// </summary>
    /// <param name="opId">the id of the operation where the task is saved in</param>
    /// <param name="taskId">the id of the task</param>
    /// <param name="date">new date with the format yyyy-MM-dd</param>
    /// <returns>return the redirection to the index</returns>
    /// <author>Raphael Huber</author>
    [HttpPost]
    public IActionResult SaveDate(int opId, int taskId, string date)
    {
        DateOnly Date = DateOnly.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        _operationRep.SaveDate(opId, taskId, Date);
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Saves the new responsible task owener to the task
    /// </summary>
    /// <param name="opId">the id of the operation where the task is saved in</param>
    /// <param name="taskId">the id of the task</param>
    /// <param name="responibleTaskOwner">the new task owner with format name|type (name is Email or Role Name and type is user or role)</returns>
    /// <author>Raphael Huber</author>
    [HttpPost]
    public IActionResult ChangeResponsibleTaskOwner(int opId, int taskId, string responibleTaskOwner)
    {
        //the responsibleTaskOwner is of format name|type so it has to be seperated
        //last index to avoid bug when Name contains |
        int lastIndex = responibleTaskOwner.LastIndexOf('|'); 
        if (lastIndex >= 0) {
            string name = responibleTaskOwner.Substring(0, lastIndex);
            string type = responibleTaskOwner.Substring(lastIndex + 1);
            _operationRep.ChangeResponsibleTaskOwner(opId, taskId, name, type);
        }
        return RedirectToAction("Index");
    }
}