using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Replay.Data;
using Replay.Models;
using Replay.Services;
using System.Linq;

namespace Replay.Repositories;

    /// <summary>
    /// The class which is used for calls on the operation-database
    /// </summary>
    /// <author>Raphael Huber</author>
    public class OperationRepository
    {
        /// <summary>The DB-Context the repository accesses</summary>
        /// <author>Raphael Huber</author>
        private readonly OperationDBContext db;

        /// <summary>
        /// Sets the db-Context via dependency-injection
        /// </summary>
        /// <param name="context">the Operation-DbContext</param>
        /// <author>Raphael Huber</author>
        public OperationRepository(OperationDBContext context)
        {
            db = context;
        }

        /// <summary>
        /// Saves a new operation inside the operation
        /// </summary>
        /// <param name="operation">the Operation that should be saved into the database</param>
        /// <author>Raphael Huber</author>
        public virtual void Create(Operation operation) 
        {
            db.Operations.Add(operation);
            db.SaveChanges();
        }

        /// <summary>
        /// Checks if the Operation already exists in the database
        /// </summary>
        /// <param name="op">the Operation that should be tested for</param>
        /// <returns>true if the operation exists in the database, otherwise false</returns>
        /// <author>Raphael Huber</author>
        public virtual bool OperationExists(Operation op) 
        {
            var operation = db.Operations.FirstOrDefault(p => p.Id == op.Id);
            return operation is not null;
        }

        /// <summary>
        /// Returns all operations that the database currently has
        /// </summary>
        /// <returns>List of all Operations stored in the database</returns>
        /// <author>Raphael Huber</author>
        public List<Operation> ReadAll()
        {
            return db.Operations
                .Include(o => o.Tasks)
                .ToList();
        }

        /// <summary>
        /// Returns all archived operations that the database currently has
        /// </summary>
        /// <returns>List of all archived Operations stored in the database</returns>
        /// <author>Raphael Huber</author>
        public List<Operation> ReadArchived()
        {
            return db.Operations
                .Include(o => o.Tasks)
                .Where(e => e.Archived)
                .ToList();
        }

        /// <summary>
        /// Returns all non-archived operations that the database currently has
        /// </summary>
        /// <returns>List of all non-archived Operations stored in the database</returns>
        /// <author>Raphael Huber</author>
        public List<Operation> ReadActive()
        {
            return db.Operations
                .Include(o => o.Tasks)
                .Where(e => !e.Archived)
                .ToList();
        }

        /// <summary>
        /// Archive an operation and save the change in the database
        /// </summary>
        /// <param name="id">The id of the operation that should be archived</param>
        /// <author>Raphael Huber</author>
        public virtual void ArchiveOperation(int id)
        {    
            var op = db.Operations.SingleOrDefault(e => e.Id == id);
            db.Operations.Attach(op);
            op.Archived = true;
            db.SaveChanges();
        }

        /// <summary>Get all tasks that the given user should see in their task-list
        /// One of the following points have to be fulfilled for a task to be returned:
        /// the user is Admin or the responsbile person for the operation the task is from
        /// the task-responsible is the user or a role the user has
        /// </summary>
        /// <param name="user">the user that the tasks have to be given for</param>
        /// <returns>a list of all tasks for the given user</returns>
        /// <author>Raphael Huber</author>
        public virtual List<ActiveTask> GetTasks(User user)
        {
            List<ActiveTask> tasks = new();
            List<Operation> operations = ReadActive();
            foreach (var op in operations) {
                if (op.Tasks is null) {
                    continue;
                }
                foreach (var task in op.Tasks) {
                    if (user.Roles != null)
                    {
                        if( user.Roles.Contains("Administrator")){
                            tasks.Add(task);
                            continue;
                        }
                    }
                     
                    if (task.ResponsibleTaskOwnerType == "user" && task.ResponsibleTaskOwnerName == user.Email) {
                            tasks.Add(task);
                            continue;
                    } else if (task.ResponsibleTaskOwnerType == "role" && user.Roles.Contains(task.ResponsibleTaskOwnerName)) {
                            tasks.Add(task);
                            continue;
                    }
                    if (op.PersonInCharge == user) {
                        tasks.Add(task);
                        continue;
                    }
                }
            }
            return tasks;
        }

        /// <summary>Sets the responslible task owner to the given user
        /// </summary>
        /// <param name="opId">operation the task is owned by</param>
        /// <param name="taskId">the id of the task</param>
        /// <param name="user">the user that claims the task</param>
        /// <author>Raphael Huber</author>
        public virtual void ClaimTask(int opId, int taskId, User user) {
            var op = db.Operations.Include(o => o.Tasks).SingleOrDefault(e => e.Id == opId);
            if (op.Tasks is null) {
                return;
            }
            var task = op.Tasks.Single(e => e.Id == taskId);
            task.ResponsibleTaskOwnerName = user.Email;
            task.ResponsibleTaskOwnerType = "user";
            task.StatusString = EnumMapperService.GetTaskStatusName(Replay.Enumerations.TaskStatus.IN_BEARBEITUNG);
            db.Update(op);
            db.SaveChanges();
        }

        /// <summary>Save a new target date for the task
        /// </summary>
        /// <param name="opId">operation the task is owned by</param>
        /// <param name="taskId">the id of the task</param>
        /// <param name="date">the new date</param>
        /// <author>Raphael Huber</author>
        public virtual void SaveDate(int opId, int taskId, DateOnly date) {
            var op = db.Operations.Include(o => o.Tasks).Single(e => e.Id == opId);
        
            if (op.Tasks is null) {
                return;
            }
            var task = op.Tasks.Single(e => e.Id == taskId);
            task.TargetDate = date;
            db.Update(op);
            db.SaveChanges();
        }

        /// <summary>Change the repsonsible task owner to a role or a user
        /// </summary>
        /// <param name="opId">operation the task is owned by</param>
        /// <param name="taskId">the id of the task</param>
        /// <param name="name">name of the new role or email of the new user</param>
        /// <param name="type">the type of the new owner ("role" or "user")</param>
        /// <author>Raphael Huber</author>
        public virtual void ChangeResponsibleTaskOwner(int opId, int taskId, string name, string type) {
            var op = db.Operations.Include(o => o.Tasks).Single(e => e.Id == opId);
            if (op.Tasks is null) {
                return;
            }
            var task = op.Tasks.Single(e => e.Id == taskId);
            task.ResponsibleTaskOwnerName = name;
            task.ResponsibleTaskOwnerType = type;
            db.Update(op);
            db.SaveChanges();
        }

        public virtual void CompleteTask(int opId, int taskId) {  
            var op = db.Operations.Include(o => o.Tasks).Single(e => e.Id == opId);
            if (op.Tasks is null) {
                return;
            }
            var task = op.Tasks.Single(e => e.Id == taskId);
            task.StatusString = EnumMapperService.GetTaskStatusName(Replay.Enumerations.TaskStatus.BEARBEITET);
            db.Update(op);
            db.SaveChanges();
        }
    }