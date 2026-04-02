// @author: Vincent Arnold
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Replay.Models;
using Replay.Repositories;
using Replay.ViewModels.ProcessViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authorization;
using Replay.Services;


namespace Replay.Controllers;
/// <summary>The process controller accepts inquiries regarding account-specific matters</summary>
/// Controller for managing processes and tasks, with authorization and role-based access.
    public class ProcessesController : Controller
    {
        
        private readonly RoleManager<Role> _roleManager;
        private readonly ProcessRepository _processRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;
        private readonly ProcessConverterService _processConverter;
        private readonly OperationRepository _operationRepository;

        // Robert added
        private readonly UserRepository _userRepository;

        public ProcessesController(OperationRepository operationRepository, RoleManager<Role> roleManager, ProcessRepository repo, UserManager<User> usermanager, SignInManager<User> signInManager, ProcessConverterService processConverter, UserRepository userRepository)
        {
            _roleManager = roleManager;
            _processRepository = repo;
            _userManager = usermanager;
            _signinManager = signInManager;
            _processConverter = processConverter;
            _operationRepository = operationRepository;
            _userRepository = userRepository;
        }
        [Authorize]
         // GET: /Processes - Displays a list of processes based on user roles
        public async Task<IActionResult> Index()
        {   
            List<Process> processes = new List<Process>();
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "Administrator")) {
                processes = await  _processRepository.LoadAllProcesses();
            } else {

                foreach (var role in user.Roles) {
                    processes.AddRange(await _processRepository.LoadProcesses(role));
                }
                
            }
            

            ProcessViewModel model = new() { Processes = processes };
            return View(model);
        }

        // GET: /Processes/CreateProcess - Displays form to create a new process (Admin only)

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateProcess()
        {   
            var roles = await _roleManager.Roles.ToArrayAsync();
            string[] roleStrings = new string[roles.Length];
            for (var i = 0; i < roleStrings.Length; i++) {
                roleStrings[i] = roles[i].Title?? "";
            }
            CreateProcessViewModel model = new() {Roles = roleStrings};
            return View(model);
        }

        // POST: /Processes/CreateProcess - Handles creation of a new process (Admin only)
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateProcess(CreateProcessViewModel model)
        {       
            if (model.Title != null)
                if (_processRepository.ExistsAlready(model.Title)) {
                    ModelState.AddModelError("Title", "Ein Prozess mit diesem Titel existiert bereits!");
                }

            if (ModelState.IsValid)
            {
                try
                {   
                    Process process = new() {Title = model.Title, Description = model.Description, AuthorizedRoles = model.ChoosedRoles};
                    _processRepository.CreateProcess(process);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {   
                    Console.WriteLine(ex);
                    Console.WriteLine("Fehler beim Erstellen des Prozesses.");
                    ModelState.AddModelError("", "Fehler beim Erstellen des Prozesses. Bitte versuchen Sie es erneut.");
                }
            }
            
            return View(model);
        }

        // POST: /Processes/DeleteProcess - Handles deletion of a process (Admin only)
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteProcess(DeleteProcessViewModel model)
        {
            try
            {
                var process = await _processRepository.LoadProcess(int.Parse(model.Id));
                if (process == null)
                {
                    return NotFound(); // Prozess nicht gefunden
                }

                _processRepository.DeleteProcess(process);
                return RedirectToAction("Index"); // Erfolgreich gelöscht
            }
            catch (Exception ex)
            {   
                Console.WriteLine(ex);
                Console.WriteLine("Fehler beim Löschen des Prozesses.");
                return BadRequest("Fehler beim Löschen des Prozesses. Bitte versuchen Sie es erneut.");
            }
        }


        // GET: /Processes/CreateTask - Displays form to create a new task if user has edit permissions
        [HttpGet]
        public async Task<IActionResult> CreateTask(int processId)
        {       

            var currentUser = await _userManager.GetUserAsync(User);
            
            if (await _processRepository.CanEditProcess(processId, currentUser.Roles)) {
                var roles = await _roleManager.Roles.ToArrayAsync();
                string[] roleStrings = new string[roles.Length];
                for (var i = 0; i < roleStrings.Length; i++) {
                    roleStrings[i] = roles[i].Title?? "";
                }

                CreateTaskViewModel model = new() {ProcessId = processId, Roles = roleStrings};
                return View(model);
            } else {
                return RedirectToAction("Index");
            }



            
        }


        /// <summary>Handles Task-Create</summary>
        /// <param name="model">contains required information for task creation</param>
        /// <author>Daniel Feustel</author>
        // POST: /Processes/CreateTask
        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskViewModel model)
        {       
            var currentUser = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid && await _processRepository.CanEditProcess(model.ProcessId, currentUser.Roles))
            {   
                Console.WriteLine("-------------------------");
                
                if (model.Description.Contains('\n')) {
                    Console.WriteLine(model.Description);
                }
                try
                {   
                  
                    DueDate dueDate = new () {DefaultValue = model.DueDateDefault, Counter = model.DueDateCounter??0, TimeUnit = model.DueDateTimeUnit??"", IsBefore = model.DueDateIsBefore??false};
                    // ITaskResponsible responsible;
                    User? respUser = null;
                    Role? respRole = null;
                    string responsible;
                    if (model.TaskResponsible == "role" && model.TaskResponsibleRole != null) {
                        
                        Role role =  await _roleManager.FindByNameAsync(model.TaskResponsibleRole);
                        responsible = role.Name;
                        respRole = role;

                    } else if (model.TaskResponsible == "referenceperson" || model.TaskResponsible == "responsibleperson") {
                        User user = new();
                        responsible = "user";
                        responsible = user.Email;

                    } else {
                        ModelState.AddModelError("TaskResponsible", "Eine explizite Rolle oder eine abstrakte Person muss gesetzt sein");
                        var roles = await _roleManager.Roles.ToArrayAsync();
                        string[] roleStrings = new string[roles.Length];
                        for (var i = 0; i < roleStrings.Length; i++) {
                            roleStrings[i] = roles[i].Title?? "";
                        }
                        model.Roles = roleStrings;
                        return View(model);
                    }
                    Process process = await _processRepository.LoadProcess(model.ProcessId);
                    TaskBluePrint task = new() {Title = model.Title, Description = model.Description, ProcessId = model.ProcessId, DueDate = dueDate, Departments = model.Departments, ContractTypes = model.ContractTypes, TaskResponsibleName = responsible, Process = process, TaskResponsibleType = model.TaskResponsible};
                    
                    _processRepository.CreateTask(task);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("Fehler beim Speichern der Aufgabe.");
                    ModelState.AddModelError("", "Fehler beim Speichern der Aufgabe. Bitte versuchen Sie es erneut.");
                }
            }

            return View(model);
        }



        /// <summary>
        /// This method deletes a task from a process. for this functionality is a delete button
        /// in the TaskDetails View. Cause there is already a CanEditProcess check in the taskDetails view there 
        /// does not have to be another in the delete function.
        /// 
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId, int processId)
        {
            // Task laden
            var task = await _processRepository.LoadTask(taskId, processId);

            // if the task is found
            if (task != null)
            {
                // deleting the task from the process Repository
                _processRepository.DeleteTask(task);

                // Redirecting to Index after Deleting is successfull
                return RedirectToAction("Index");
            } 
            else
            {
                // Task not found 
                return NotFound();
            }
        }


        // GET: /Processes/TaskDetails - Displays task details if user has edit permissions
        [HttpGet]
        public async Task<IActionResult> TaskDetails(int taskId, int processId) {
            var currentUser = await _userManager.GetUserAsync(User);

            if (await _processRepository.CanEditProcess(processId, currentUser.Roles)) {
                TaskBluePrint task = await _processRepository.LoadTask(taskId, processId);
                var roles = await _roleManager.Roles.ToArrayAsync();
                string[] roleStrings = new string[roles.Length];
                for (var i = 0; i < roleStrings.Length; i++) {
                    roleStrings[i] = roles[i].Title?? "";
                }
                
                TaskDetailsViewModel model = new() {
                    Id = task.Id, 
                    Title = task.Title,
                    Description = task.Description, 
                    ProcessId = task.ProcessId, 
                    ContractTypes = task.ContractTypes?? Array.Empty<string>(), 
                    Departments = task.Departments?? Array.Empty<string>(), 
                    TaskResponsible = task.TaskResponsibleType, 
                    TaskResponsibleRole = task.TaskResponsibleType == "role" ?task.TaskResponsibleName : "", 
                    Roles = roleStrings,
                    DueDateDefault = task.DueDate.DefaultValue ?? "", 
                    DueDateCounter = task.DueDate.Counter, 
                    DueDateTimeUnit = task.DueDate.TimeUnit?? "", 
                    DueDateIsBefore = task.DueDate.IsBefore};
                
                return View(model);
            } else {
                return RedirectToAction("Index");
            }

            
        }
        /// <summary>Handles Task-Updates</summary>
        /// <param name="model">contains information required to update a task</param>
        /// <author>Daniel Feustel</author>
        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskDetailsViewModel model) {
            
            try {   
                var currentUser = await _userManager.GetUserAsync(User);

                if (await _processRepository.CanEditProcess(model.ProcessId, currentUser.Roles)) {
                    DueDate dueDate = new () {DefaultValue = model.DueDateDefault, Counter = model.DueDateCounter??0, TimeUnit = model.DueDateTimeUnit??"", IsBefore = model.DueDateIsBefore??false};
                    // ITaskResponsible responsible;
                    User? respUser = null;
                    Role? respRole = null;
                    string responsible;
                    if (model.TaskResponsible == "role" && model.TaskResponsibleRole != null) {
                        
                        Role role =  await _roleManager.FindByNameAsync(model.TaskResponsibleRole);
                        respRole = role;
                        responsible = role.Title;

                    } else if (model.TaskResponsible == "referenceperson" || model.TaskResponsible == "responsibleperson") {
                        User user = new();
                        responsible = user.Email;
                        respUser = user;
                        
                    } else {
                        ModelState.AddModelError("TaskResponsible", "Eine explizite Rolle oder eine abstrakte Person muss gesetzt sein");
                        var roles = await _roleManager.Roles.ToArrayAsync();
                        string[] roleStrings = new string[roles.Length];
                        for (var i = 0; i < roleStrings.Length; i++) {
                            roleStrings[i] = roles[i].Title?? "";
                        }
                        model.Roles = roleStrings;
                        return View(model);
                    }
                    Process process = await _processRepository.LoadProcess(model.ProcessId);
                    TaskBluePrint task = new() {
                        Id = model.Id, 
                        Title = model.Title, 
                        Description = model.Description, 
                        ProcessId = model.ProcessId, 
                        DueDate = dueDate, 
                        TaskResponsibleName = responsible,
                        Departments = model.Departments, 
                        ContractTypes = model.ContractTypes, 
                        Process = process, 
                        TaskResponsibleType = model.TaskResponsible
                        };
                    
                    _processRepository.UpdateTask(task);
                }
                    return RedirectToAction("Index");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            
            return View(model);
        }
        /// <summary>Returns the activate process view</summary>
        /// <param name="processId">the id of the process, that should be activated</param>
        /// <author>Daniel Feustel</author>
        [HttpGet]
        public async Task<IActionResult> ActivateProcess(int processId) {

            var currentUser = await _userManager.GetUserAsync(User);

            if (await _processRepository.CanEditProcess(processId, currentUser.Roles)) {
                Process process = await _processRepository.LoadProcess(processId);
                // List<User> users = _userManager.Users.ToList();
                List<User> users = await _userRepository.GetActiveUsersAsync(); // method from UserRepository, filters the list

                DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                ActivateProcessViewModel model = new(){Process = process, Users = users, TargetDate = date, CurrentUserEmail = currentUser.Email};
                return View(model);
            } else {
                return RedirectToAction("Index");
            }
        }



        /// <summary>Handles the Activate-Process Request</summary>
        /// <param name="model">contains information required to update a task</param>
        /// <author>Daniel Feustel</author>    
        [HttpPost] 
        public async Task<IActionResult> ActivateProcess(ActivateProcessViewModel model) {
            var currentUser = await _userManager.GetUserAsync(User);
            if (await _processRepository.CanEditProcess(model.ProcessId, currentUser.Roles)) {
                if (DateOnly.TryParse(model.TargetDateString, out var targetDate)) {

                    if (model.Department == null) {
                        ModelState.AddModelError("Department", "Die Abteilung muss ausgewählt sein.");
                    }

                    if (string.IsNullOrEmpty(model.ReferencePersonEmail)) {
                        ModelState.AddModelError("ReferencePersonEmail", "Die Bezugsperson muss ausgewählt sein.");
                    }
                   
                    if(model.ContractType == null){
                        ModelState.AddModelError("ContractType", "Der Vertragstyp muss ausgewählt sein.");
                    }

                    if (ModelState.IsValid) {
                    Process process = await _processRepository.LoadProcess(model.ProcessId);
                    User refPerson = await _userManager.FindByEmailAsync(model.ReferencePersonEmail);
                    User persInCharge = await _userManager.FindByEmailAsync(model.PersonInChargeEmail);
                    model.Process = process;
                    model.PersonInCharge = persInCharge;
                    model.ReferencePerson = refPerson;
                    model.TargetDate = targetDate;

                    Operation operation = _processConverter.Convert(model);
                    _operationRepository.Create(operation);
                } else {
                    model.Users = _userManager.Users.ToList();
                    return RedirectToAction("Index");
                }
                } else {
                    Console.WriteLine("Error While Parsing the targetDate");
                }
                    
                


                
                
            }
           
            return LocalRedirect("/Operations");
            
        }
    }