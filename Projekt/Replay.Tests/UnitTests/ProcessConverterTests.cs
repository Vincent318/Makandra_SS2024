using Xunit;
using System.Collections.Generic;
using Moq;
using Replay.Repositories;
using Replay.Controllers;
using Replay.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Replay.Services;
using Replay.ViewModels.ProcessViewModels;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace Replay.Tests.UnitTests;

/// <author>Raphael Huber</author>
public class ProcessConverterTests
{   
    private readonly ActivateProcessViewModel proc1;
    private readonly Operation op1;

    public ProcessConverterTests() {

        TaskBluePrint taskBluePrint = new() {
            Id = 1, 
            Title = "TestTitle", 
            Description = "TestDesc", 
            ProcessId = 1, 
            DueDate = new() {Id = 1, DefaultValue = "ASAP"}, 
            DepartmentsString = "", 
            ContractTypesString = "", 
            TaskResponsibleType = "role", 
            TaskResponsibleName = "Administrator"
        };

        Process process = new() {
            Id = 2,
            Title = "ProcessTest",
            Description = "ProcessDesc",
            ContractType = "",
            AuthorizedRoles = new[] {""},
            Department = "",
            Tasks = new() {taskBluePrint},

        };

        proc1 = new() {
            Title = "Onboarding",
            Description = "Onboarding für Max Mustermann",
            Process = process,
            ProcessId = 2,
            ReferencePerson = new User() {Email = "karl@replay.de"},
            PersonInCharge = new User() {Email = "wilma@replay.de"},
            ReferencePersonEmail = "karl@replay.de",
            PersonInChargeEmail = "wilma@replay.de",
            Archived = false,
            TargetDate = new(2000, 12, 22),
            TargetDateString = "2000,12,22",
            Users = new() {new() {Email = "karl@replay.de"}},
            Department = "",
            ContractType = "",
            CurrentUserEmail = "admin@replay.de",
        };

        

        ActiveTask task = new() {
            Title = "TestTitle",
            Instruction = "TestDesc",
            TargetDate = DateOnly.FromDateTime(DateTime.Now),
            StatusString = "unbearbeitet",
            ContractTypes = new[] {""},
            Departments = new[] {""},
            ResponsibleTaskOwnerType = "role",
            ResponsibleTaskOwnerName = "Administrator",
            OperationId = 4,
        };


        op1 = new() {
            Id = 0, 
            Title = "Onboarding", 
            Description = "Onboarding für Max Mustermann", 
            TargetDate = new(2000, 12, 22), 
            ContractTypeString = "", 
            DepartmentString = "", 
            ReferencePerson = new User() {Email = "karl@replay.de"}, 
            PersonInCharge = new User() {Email = "wilma@replay.de"}, 
            ReferencePersonMail = "karl@replay.de",
            PersonInChargeMail = "wilma@replay.de",
            Tasks = new() {task}, 
            Archived = false
        };
    }




    [Fact]
    public async void ProcessConverter_GenerateActiveTask_Test() {
        var processConvert = new ProcessConverterService();
        
        // Action
        var op = processConvert.Convert(proc1);

        // Asserts
        Assert.Equal(op1.Title, op.Title);
        Assert.Equal(op1.Description, op.Description);
        Assert.Equal(op1.TargetDate, op.TargetDate);
        Assert.Equal(op1.ContractTypeString, op.ContractTypeString);
        Assert.Equal(op1.DepartmentString, op.DepartmentString);
        Assert.Equal(op1.ReferencePersonMail, op.ReferencePersonMail);
        Assert.Equal(op1.PersonInChargeMail, op.PersonInChargeMail);
        Assert.Equal(op1.Archived, op.Archived);
        Assert.Equal(op1.Tasks.Count, op.Tasks.Count);
        Assert.Equal(op1.Tasks[0].Title, op.Tasks[0].Title);
        Assert.Equal(op1.Tasks[0].Instruction, op.Tasks[0].Instruction);
        Assert.Equal(op1.Tasks[0].TargetDate, op.Tasks[0].TargetDate);
        Assert.Equal(op1.Tasks[0].StatusString, op.Tasks[0].StatusString);
        Assert.Equal(op1.Tasks[0].ContractTypes, op.Tasks[0].ContractTypes);
        Assert.Equal(op1.Tasks[0].Departments, op.Tasks[0].Departments);
        Assert.Equal(op1.Tasks[0].ResponsibleTaskOwnerType, op.Tasks[0].ResponsibleTaskOwnerType);
        Assert.Equal(op1.Tasks[0].ResponsibleTaskOwnerName, op.Tasks[0].ResponsibleTaskOwnerName);
    }
}