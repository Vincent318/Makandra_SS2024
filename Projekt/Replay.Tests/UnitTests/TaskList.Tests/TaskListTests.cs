using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Replay.Models;
using Replay.Repositories;
using Replay.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

/// <author>Noah Engelschall</author>

namespace Replay.Tests.UnitTests.TaskList.Tests
{
        public class TaskListTests
        {
                private readonly Mock<OperationRepository> _operationRepositoryMock;
                private readonly Mock<UserManager<User>> _userManagerMock;
                private readonly TaskListController _controller;

                public TaskListTests()
                {
                        _operationRepositoryMock = new Mock<OperationRepository>(null);
                        _userManagerMock = new Mock<UserManager<User>>(
                            new Mock<IUserStore<User>>().Object,
                            null, null, null, null, null, null, null, null
                        );

                        _controller = new TaskListController(_operationRepositoryMock.Object, _userManagerMock.Object);

                        var httpContext = new DefaultHttpContext();
                        _controller.ControllerContext = new ControllerContext
                        {
                                HttpContext = httpContext
                        };
                }

                // Test for Index method without filter and non-admin user
                [Fact]
                public async Task Index_ReturnsViewResult_WithCorrectViewModel_ForNonAdminUser()
                {
                        var user = new User { Email = "user@example.com" };
                        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                        _operationRepositoryMock.Setup(or => or.GetTasks(user)).Returns(new List<ActiveTask>
            {
                new ActiveTask { Id = 1, Title = "Task 1", StatusString = "offen", ResponsibleTaskOwnerName = "user@example.com" },
                new ActiveTask { Id = 2, Title = "Task 2", StatusString = "bearbeitet", ResponsibleTaskOwnerName = "user@example.com" }
            });

                        var result = await _controller.Index();

                        var viewResult = Assert.IsType<ViewResult>(result);
                        var viewModel = Assert.IsType<TaskListViewModel>(viewResult.Model);
                        Assert.Equal("Alle", viewModel.ViewType);
                        Assert.Single(viewModel.Tasks);
                        Assert.Equal("Task 1", viewModel.Tasks[0].Title);
                }

                // Test for Index method with filter and admin user
                [Fact]
                public async Task Index_ReturnsViewResult_WithCorrectViewModel_ForAdminUser()
                {
                        // Arrange: Set up mock admin user and tasks
                        var user = new User { Email = "admin@example.com" };
                        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                        _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Administrator")).ReturnsAsync(true);
                        _operationRepositoryMock.Setup(or => or.GetTasks(user)).Returns(new List<ActiveTask>
            {
                new ActiveTask { Id = 1, Title = "Task 1", StatusString = "in Bearbeitung", ResponsibleTaskOwnerName = "user@example.com" },
                new ActiveTask { Id = 2, Title = "Task 2", StatusString = "offen", ResponsibleTaskOwnerName = "admin@example.com" }
            });

                        var result = await _controller.Index(viewType: "Meine", filter: "Task", filterType: "Aufgabe");

                        var viewResult = Assert.IsType<ViewResult>(result);
                        var viewModel = Assert.IsType<TaskListViewModel>(viewResult.Model);
                        Assert.Equal("Meine", viewModel.ViewType);
                        Assert.Single(viewModel.Tasks);
                        Assert.Equal("Task 1", viewModel.Tasks[0].Title);
                }

                // Test for ClaimTask method
                [Fact]
                public async Task ClaimTask_RedirectsToIndex_WhenTaskExists()
                {
                        var user = new User { Email = "user@example.com" };
                        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                        _operationRepositoryMock.Setup(or => or.GetTasks(user)).Returns(new List<ActiveTask>
            {
                new ActiveTask { Id = 1, OperationId = 100, Title = "Task 1", StatusString = "offen" }
            });

                        var result = await _controller.ClaimTask(1);

                        _operationRepositoryMock.Verify(or => or.ClaimTask(100, 1, user), Times.Once);
                        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                        Assert.Equal("Index", redirectResult.ActionName);
                }

                // Test for CompleteTask method
                [Fact]
                public async Task CompleteTask_RedirectsToIndex_WhenTaskExists()
                {
                        var user = new User { Email = "user@example.com", TasksFinished = 0 };
                        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                        _operationRepositoryMock.Setup(or => or.GetTasks(user)).Returns(new List<ActiveTask>
            {
                new ActiveTask { Id = 1, OperationId = 100, Title = "Task 1", StatusString = "offen" }
            });

                        var result = await _controller.CompleteTask(1);

                        _operationRepositoryMock.Verify(or => or.CompleteTask(100, 1), Times.Once);
                        _userManagerMock.Verify(um => um.UpdateAsync(It.Is<User>(u => u.TasksFinished == 1)), Times.Once);
                        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                        Assert.Equal("Index", redirectResult.ActionName);
                }

                // Additional tests for filters and roles
                [Fact]
                public async Task Index_ReturnsViewResult_WithFilteredTasks()
                {
                        var user = new User { Email = "user@example.com" };
                        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                        _operationRepositoryMock.Setup(or => or.GetTasks(user)).Returns(new List<ActiveTask>
            {
                new ActiveTask { Id = 1, Title = "Important Task", StatusString = "offen", ResponsibleTaskOwnerName = "user@example.com" },
                new ActiveTask { Id = 2, Title = "Another Task", StatusString = "offen", ResponsibleTaskOwnerName = "user@example.com" }
            });

                        var result = await _controller.Index(filter: "Important", filterType: "Aufgabe");

                        var viewResult = Assert.IsType<ViewResult>(result);
                        var viewModel = Assert.IsType<TaskListViewModel>(viewResult.Model);
                        Assert.Single(viewModel.Tasks);
                        Assert.Equal("Important Task", viewModel.Tasks[0].Title);
                }

                [Fact]
                public async Task Index_ReturnsViewResult_WithFilteredTasks_ForAdmin()
                {
                        var user = new User { Email = "admin@example.com" };
                        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
                        _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Administrator")).ReturnsAsync(true);
                        _operationRepositoryMock.Setup(or => or.GetTasks(user)).Returns(new List<ActiveTask>
            {
                new ActiveTask { Id = 1, Title = "Admin Task 1", StatusString = "in Bearbeitung", ResponsibleTaskOwnerName = "user@example.com" },
                new ActiveTask { Id = 2, Title = "Admin Task 2", StatusString = "offen", ResponsibleTaskOwnerName = "admin@example.com" }
            });

                        var result = await _controller.Index(viewType: "Meine", filter: "Admin", filterType: "Aufgabe");

                        var viewResult = Assert.IsType<ViewResult>(result);
                        var viewModel = Assert.IsType<TaskListViewModel>(viewResult.Model);
                        Assert.Single(viewModel.Tasks);
                        Assert.Equal("Admin Task 1", viewModel.Tasks[0].Title);
                }
        }
}
