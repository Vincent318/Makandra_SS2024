// @author: Daniel Feustel

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Replay.Models;
using Replay.Repositories;
using Replay.Services;
using Replay.ViewModels;



namespace Replay.Controllers;
/// <summary>
///  The dashboard controller accepts inquiries regarding dashboard-specific matters
/// </summary>
/// <author>Daniel Feustel</author>
public class DashboardController : Controller
{
    
    private readonly UserManager<User> _usermanager;
    private readonly IConfiguration _conf;

    private readonly OperationRepository _operationRepository;
    

    public DashboardController(UserManager<User> userManager, IConfiguration conf, OperationRepository repo)
    {
        _usermanager = userManager;
        _conf = conf;
        _operationRepository = repo;
    }

    /// <summary>Returns the Dashboard View</summary>
    [Authorize]
    public async Task<IActionResult> Index()
    {           

        User user = await _usermanager.GetUserAsync(User);
        List<ActiveTask> tasks = _operationRepository.GetTasks(user);

        int openTasksCounter = 0;
        int claimedTaskCounter = 0;
        int finishedTaskCounter = user.TasksFinished;
        var activeOperations = _operationRepository.ReadActive();
        int activeOperationsCounter = activeOperations.Count; 

        foreach (var task in tasks) {
            if (task.ResponsibleTaskOwnerName == user.Email && task.StatusString != EnumMapperService.GetTaskStatusName(Enumerations.TaskStatus.BEARBEITET)) {
                claimedTaskCounter++;
            }
            if (task.StatusString == EnumMapperService.GetTaskStatusName(Enumerations.TaskStatus.UNBEARBEITET)) {
                openTasksCounter++;
            }
        }


        if (User.Identity != null) {
            var usr = await _usermanager.FindByNameAsync(User.Identity.Name);
            DashboardViewModel model;
            if (await _usermanager.IsInRoleAsync(usr, "Administrator")) {
                model = new() {TasksOpen = openTasksCounter, TasksInProgress = claimedTaskCounter, TasksDone = finishedTaskCounter, OperationsActive = activeOperationsCounter, IsAdmin = true};
            } else {
                model = new() {TasksOpen = openTasksCounter, TasksInProgress = claimedTaskCounter, TasksDone = finishedTaskCounter, IsAdmin = false};
            }
            return View(model);
        }  
        return LocalRedirect("/Error?statusCode=500");
    }
}