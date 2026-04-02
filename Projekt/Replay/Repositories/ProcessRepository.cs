// @author: Vincent Arnold
using Replay.Data;
using Replay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Replay.Repositories
{
    /// <summary>
    /// Provides methods to interact with process data in the database.
    /// </summary>
    public class ProcessRepository
    {
        private readonly ILogger<ProcessRepository> _logger;
        private readonly ProcessDBContext _dbContext;
        
        /// <summary>
        /// Initializes a new instance of the ProcessRepository class.
        /// </summary>
        /// <param name="logger">Logger for logging errors.</param>
        /// <param name="dbContext">Database context for accessing process data.</param>
        public ProcessRepository(ILogger<ProcessRepository> logger, ProcessDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // Method to create a new process in the database
        public void CreateProcess(Process process)
        {
            try
            {
                _dbContext.Processes.Add(process);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating the process.");
                throw; // Optionally handle the exception more specifically
            }
        }

        // Method to load a process by its ID, including associated tasks
        public async Task<Process> LoadProcess(int processId)
        {
            var process = await _dbContext.Processes
                .Include(p => p.Tasks).ThenInclude(t => t.DueDate)
                // .Include(p => p.Tasks).ThenInclude(t => t.TaskRespnsibleRole)
                .FirstOrDefaultAsync(p => p.Id == processId);
            if (process == null)
            {
                throw new InvalidOperationException($"Process with ID {processId} was not found.");
            }
            return process;
        }

        // Method to load processes that are authorized for a given role
        public async Task<List<Process>> LoadProcesses(string role)
        {
            var processes = _dbContext.Processes
                .Include(p => p.Tasks).ThenInclude(t => t.DueDate)
                // .Include(p => p.Tasks).ThenInclude(t => t.TaskRespnsibleRole)
                .AsEnumerable()
                .Where(p => p.AuthorizedRoles.Contains(role))
                .ToList();

            return processes;
        } 

        // Method to check if a process can be edited by given roles
        public async Task<bool> CanEditProcess(int processId, string[] roles)
        {
            if (roles.Contains("Administrator"))
            {
                return true;
            }
            Process process = await LoadProcess(processId);

            foreach (var r in roles)
            {
                if (process.AuthorizedRoles.Contains(r))
                    return true;
            }
            return false;
        }

        // Method to load all processes, including associated tasks
        public async Task<List<Process>> LoadAllProcesses()
        {
            var processes = await _dbContext.Processes
                .Include(p => p.Tasks).ThenInclude(t => t.DueDate)
                .ToListAsync();
            if (processes == null)
            {
                throw new InvalidOperationException("No Processes available");
            }
            return processes;
        }

        // Method to delete a process from the database
        public void DeleteProcess(Process process)
        {
            try
            {
                _dbContext.Processes.Remove(process);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting the process.");
                throw;
            }
        }

        // Method to update an existing process in the database
        public void UpdateProcess(Process process)
        {
            try
            {
                _dbContext.Processes.Update(process);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the process.");
                throw;
            }
        }

        // Method to check if a process exists by its ID
        public virtual bool ProcessExists(Process op)
        {
            var operation = _dbContext.Processes.FirstOrDefault(p => p.Id == op.Id);
            return operation != null;
        }
        
        // Method to check if a process with the given title already exists
        public bool ExistsAlready(string title)
        {
            return _dbContext.Processes.Any(p => p.Title == title);
        }

        // Method to create a task and associate it with a process
        public async void CreateTask(TaskBluePrint task)
        {
            var process = await LoadProcess(task.ProcessId);
            if (process != null)
            {
                _dbContext.Processes.Attach(process);
                process.Tasks.Add(task);
                _dbContext.Processes.Update(process);
                await _dbContext.SaveChangesAsync();
            }
        }

        // Method to load a task by its ID
        public async Task<TaskBluePrint> LoadTask(int taskId)
        {
            var process = await _dbContext.Processes
                .Include(p => p.Tasks).ThenInclude(t => t.DueDate)
                // .Include(p => p.Tasks).ThenInclude(t => t.TaskRespnsibleRole)
                .SingleOrDefaultAsync(p => p.Tasks.Any(t => t.Id == taskId));
            if (process != null)
            {
                var task = process.Tasks.SingleOrDefault(t => t.Id == taskId);
                return task;
            }
            return null;
        }

        // Method to load a task by its ID and its associated process ID
        public async Task<TaskBluePrint> LoadTask(int taskId, int processId)
        {
            var process = await LoadProcess(processId);
            if (process != null)
            {
                var task = process.Tasks.SingleOrDefault(t => t.Id == taskId);
                return task;
            }
            return null;
        }

        // Method to update an existing task in a process
        public async void UpdateTask(TaskBluePrint task)
        {
            var process = await LoadProcess(task.ProcessId);
            if (process != null)
            {
                _dbContext.Entry(process).State = EntityState.Detached;

                _dbContext.Processes.Attach(process);
                var tasks = process.Tasks;
                foreach (var t in tasks)
                {
                    if (t.Id == task.Id)
                    {
                        t.ContractTypes = task.ContractTypes;
                        t.Departments = task.Departments;
                        t.Description = task.Description;
                        t.DueDate = task.DueDate;
                        t.Title = task.Title;
                        t.TaskResponsibleType = task.TaskResponsibleType;
                        t.TaskResponsibleName = task.TaskResponsibleName;
                    }
                }
                process.Tasks = tasks;
                
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("ERROR Updating the task " + task.Title);
            }
        }

        // Method to delete a task from a process
        public async void DeleteTask(TaskBluePrint task)
        {
            var process = await LoadProcess(task.ProcessId);
            if (process != null)
            {
                _dbContext.Entry(process).State = EntityState.Detached;

                _dbContext.Processes.Attach(process);
                var tasks = process.Tasks;
                tasks.Remove(task);
                process.Tasks = tasks;
                
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("ERROR Deleting the task " + task.Title);
            }
        }
    }
}