using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Replay.Models;
using Replay.Repositories;
using Replay.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

/// <summary>
/// Controller responsible for handling task list operations.
/// </summary>
/// <author>Noah Engelschall</author>
public class TaskListController : Controller
{
        private readonly OperationRepository _operationRepository;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskListController"/> class.
        /// </summary>
        /// <param name="operationRepository">The operation repository instance.</param>
        /// <param name="userManager">The user manager instance.</param>
        public TaskListController(OperationRepository operationRepository, UserManager<User> userManager)
        {
                _operationRepository = operationRepository;
                _userManager = userManager;
        }

        /// <summary>
        /// Displays the task list view.
        /// </summary>
        /// <param name="viewType">The type of view to display (e.g., "Alle", "Meine").</param>
        /// <param name="filter">The filter string to apply to the task list.</param>
        /// <param name="filterType">The type of filter to apply (e.g., "Aufgabe", "Vorgang", "Alle").</param>
        /// <param name="first"></param>
        /// <returns>The task list view.</returns>
        public async Task<IActionResult> Index(string viewType = "Alle", string filter = "", string filterType = "Alle", bool first = true)
        {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                        return Challenge();
                }

                if (filter != null)
                {
                        string pattern = @"[\n\r\t\\]";
                        string clearedFilter = Regex.Replace(filter, pattern, string.Empty);
                        HttpContext.Response.Cookies.Append("tasklist-filter", clearedFilter);
                }



                if (first && HttpContext.Request.Cookies.TryGetValue("tasklist-filter", out string? filterCookie))
                {
                        filter = filterCookie;
                }



                var activeTasks = (_operationRepository.GetTasks(user)).Where(t => t.StatusString != "bearbeitet").ToList();

                if (!string.IsNullOrEmpty(filter))
                {
                        switch (filterType)
                        {
                                case "Aufgabe":
                                        activeTasks = activeTasks.Where(t => t.Title.Contains(filter)).ToList();
                                        break;
                                case "Vorgang":
                                        activeTasks = activeTasks.Where(t => t.Operation.Title.Contains(filter)).ToList();
                                        break;
                                default:
                                        activeTasks = activeTasks.Where(t => t.Title.Contains(filter) || t.Operation.Title.Contains(filter)).ToList();
                                        break;
                        }
                }

                List<ActiveTask> tasks;
                if (await _userManager.IsInRoleAsync(user, "Administrator"))
                {
                        if (viewType == "Meine")
                        {
                                tasks = activeTasks.Where(t => t.StatusString == "in Bearbeitung").ToList();
                        }
                        else
                        {
                                tasks = activeTasks;
                        }
                }
                else
                {
                        tasks = activeTasks.Where(t => t.ResponsibleTaskOwnerName == user.Email).ToList();
                }

                var viewModel = new TaskListViewModel
                {
                        ViewType = viewType,
                        Tasks = viewType == "Meine" ? tasks : activeTasks,
                        Filter = filter,
                        FilterType = filterType
                };

                return View(viewModel);
        }

        /// <summary>
        /// Claims a task for the current user.
        /// </summary>
        /// <param name="id">The ID of the task to claim.</param>
        /// <returns>Redirects to the Index view.</returns>
        [HttpPost]
        public async Task<IActionResult> ClaimTask(int id)
        {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                        return Challenge();
                }

                var task = (_operationRepository.GetTasks(user)).FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                        _operationRepository.ClaimTask(task.OperationId, id, user);
                }
                return RedirectToAction(nameof(Index), new { viewType = "Alle" });
        }

        /// <summary>
        /// Marks a task as completed.
        /// </summary>
        /// <param name="id">The ID of the task to complete.</param>
        /// <returns>Redirects to the Index view.</returns>
        [HttpPost]
        public async Task<IActionResult> CompleteTask(int id)
        {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                        return Challenge();
                }

                var task = (_operationRepository.GetTasks(user)).FirstOrDefault(t => t.Id == id);
                _operationRepository.CompleteTask(task.OperationId, id);

                user.TasksFinished++;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(nameof(Index), new { viewType = "Meine" });
        }
}
