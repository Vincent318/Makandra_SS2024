using Replay.Data;
using Replay.Enumerations;
using Replay.Models;
using Replay.Repositories;
using Replay.ViewModels.ProcessViewModels;

namespace Replay.Services;
/// <author>Daniel Feustel</author>
public class ProcessConverterService {
    public ProcessConverterService() {
    } 

    public Operation Convert(ActivateProcessViewModel model) {
        
        Operation newOperation = new() {
            Title = model.Title,
            Description = model.Description,
            TargetDate = model.TargetDate,
            ContractTypeString = model.ContractType,
            DepartmentString = model.Department,
            ReferencePerson = model.ReferencePerson,
            ReferencePersonMail = model.ReferencePerson.Email,
            PersonInCharge = model.PersonInCharge,
            PersonInChargeMail = model.PersonInCharge.Email,
            Archived = model.Archived
        };

        List<ActiveTask> activeTasks = new();
        foreach (var task in model.Process.Tasks) {
            Console.WriteLine(task.Departments[0]);
            if ((task.Departments != null) && (task.Departments.Count() != 0) && (task.Departments[0] != "") && !task.Departments.Contains(model.Department)) {
                continue;
            }
            if ((task.ContractTypes != null) && (task.ContractTypes.Count() != 0) && (task.ContractTypes[0] != "") && !task.ContractTypes.Contains(model.ContractType)) {
                continue;
            }
            activeTasks.Add(GenerateActiveTask(task, newOperation));
        }
        newOperation.Tasks = activeTasks;
        return newOperation;
    }


    private ActiveTask GenerateActiveTask(TaskBluePrint task, Operation operation) {
        
        DateOnly convertedDate = DueDateConverterService.Convert(task.DueDate, operation.TargetDate);
        string taskResponsibleName;
        string taskResponsibleType;
        if (task.TaskResponsibleType == "role") {
            taskResponsibleType = "role";
            taskResponsibleName = task.TaskResponsibleName;
        } else if (task.TaskResponsibleType == "referenceperson") {
            taskResponsibleType = "user";
            taskResponsibleName = operation.ReferencePerson.Email;
        } else if (task.TaskResponsibleType == "responsibleperson") {
            taskResponsibleType = "user";
            taskResponsibleName = operation.PersonInCharge.Email;
        } else {
            throw new Exception("!!!!Kein gültiger Taskresponsibletype!!!! Fehler");
        }
        string defaultStatus = taskResponsibleType == "user" ? EnumMapperService.GetTaskStatusName(Enumerations.TaskStatus.IN_BEARBEITUNG) : EnumMapperService.GetTaskStatusName(Enumerations.TaskStatus.UNBEARBEITET);

        ActiveTask newTask = new() {
            Title = task.Title,
            Instruction = task.Description,
            TargetDate = convertedDate,
            StatusString = defaultStatus,
            ContractTypes = task.ContractTypes,
            Departments = task.Departments,
            ResponsibleTaskOwnerType = taskResponsibleType,
            ResponsibleTaskOwnerName = taskResponsibleName,
            OperationId = operation.Id,
            Operation = operation
        };

        return newTask;
    }




}