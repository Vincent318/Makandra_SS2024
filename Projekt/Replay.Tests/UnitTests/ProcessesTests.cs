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
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Replay.Services;
using Replay.ViewModels.ProcessViewModels;

namespace Replay.Tests.UnitTests;

/// <author>Raphael Huber</author>
public class ProcessesTests
{   

    [Fact]
    public async void UpdateProcessTest() {
        var mockLogger = new Mock<ILogger<ProcessRepository>>();
        var dbSet = new Mock<DbSet<Process>>();
        var dbContext = new Mock<ProcessDBContext>(new DbContextOptions<ProcessDBContext>());

        dbContext.Setup(m => m.Processes).Returns(dbSet.Object);

        var repo = new ProcessRepository(mockLogger.Object, dbContext.Object);
        
        // Setup test data
        List<TaskBluePrint> tasks = new() {new() {Id = 1, Title = "Test-Task", Description = "Test-Desc", ProcessId = 1, DueDate = new() {DefaultValue = "ASAP"}, TaskResponsibleName = "admin@replay.de", TaskResponsibleType = "user", ContractTypes = new[] {"Festanstellung"}, Departments = new[] {"Entwicklung"}}};
        Process p1 = new() {Id = 1, Title = "p1", Description = "P1-Test", AuthorizedRoles = new[] {"Administrator"}, ContractType = "Festanstellung", Department = "", Tasks = tasks};

        //Action
        repo.UpdateProcess(p1);

        //Assertions
        dbContext.Verify(d => d.Processes.Update(p1), Times.Once);
        dbContext.Verify(d => d.SaveChanges(), Times.Once);
    }
}