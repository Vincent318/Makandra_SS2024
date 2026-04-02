//author: Vincent Arnold
using Xunit;
using Moq;
using Replay.Controllers;
using Replay.Models;
using Replay.ViewModels;
using Replay.ViewModels.RoleViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Replay.Tests.UnitTests
{
    public class RolesUnitTests
    {
        private readonly RolesController _controller;
        private readonly Mock<RoleManager<Role>> _mockRoleManager;
        private readonly Mock<ILogger<RolesController>> _mockLogger;

        public RolesUnitTests()
        {
            _mockRoleManager = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(),
                null,
                null,
                null,
                null
            );
            _mockLogger = new Mock<ILogger<RolesController>>();

            _controller = new RolesController(_mockLogger.Object, _mockRoleManager.Object);
        }

        [Fact]
        public async Task CreateRole_Returns_RedirectToActionResult_When_ModelState_IsValid()
        {
            var model = new CreateRoleViewModel
            {
                Title = "Manager",
                Description = "Manages the team"
            };
            
            var role = new Role { Title = model.Title, Description = model.Description };
            _mockRoleManager.Setup(m => m.CreateAsync(It.IsAny<Role>()))
                .ReturnsAsync(IdentityResult.Success);

            _controller.ModelState.Clear(); // Ensure model state is valid

            var result = await _controller.CreateRole(model);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            _mockRoleManager.Verify(m => m.CreateAsync(It.Is<Role>(r => r.Title == model.Title && r.Description == model.Description)), Times.Once);
        }

        [Fact]
        public async Task CreateRole_Returns_ViewResult_When_ModelState_IsInvalid()
        {
            _controller.ModelState.AddModelError("Title", "Required");
            var model = new CreateRoleViewModel
            {
                Title = "Manager",
                Description = "Manages the team"
            };

            var result = await _controller.CreateRole(model);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model); // Optional: Check if the model is correctly passed
            _mockRoleManager.Verify(m => m.CreateAsync(It.IsAny<Role>()), Times.Never);
        }
    }
}