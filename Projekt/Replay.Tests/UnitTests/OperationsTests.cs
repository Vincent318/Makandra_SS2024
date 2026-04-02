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
namespace Replay.Tests.UnitTests;

public class OperationsTests
{   
    private readonly OperationRepository _repo;
    private readonly OperationsController _controller;
    private readonly Mock<UserManager<User>> _usermanager;
    private readonly Mock<RoleManager<Role>> _rolemanager;
    private readonly Mock<DbSet<Operation>> _dbSet;
    private readonly Mock<OperationDBContext> _context;
    private readonly Mock<UserRepository> _userRepositoryMock;
    private List<Operation> dataList;
    private Operation op1 = new() {Id = 1, Title = "Opertaion1", Description = "op1", TargetDate = new DateOnly(), ContractTypeString = "Werkstudent", DepartmentString = "Operations", ReferencePerson = new User(), PersonInCharge = new User(), Tasks = new List<ActiveTask>(), Archived = false};
    private Operation op2 = new() {Id = 2, Title = "Opertaion2", Description = "op2", TargetDate = new DateOnly(), ContractTypeString = "Fest", DepartmentString = "GL", ReferencePerson = new User(), PersonInCharge = new User(), Tasks = new List<ActiveTask>(), Archived = false};
    private Operation op3 = new() {Id = 3, Title = "Opertaion3", Description = "op3", TargetDate = new DateOnly(), ContractTypeString = "Teilzeit", DepartmentString = "Personal", ReferencePerson = new User(), PersonInCharge = new User(), Tasks = new List<ActiveTask>(), Archived = false};

    public OperationsTests() {
        //Arrange
        _dbSet = new Mock<DbSet<Operation>>();
        _context = new Mock<OperationDBContext>();

        dataList = new() {op1, op2, op3};
        _dbSet.Setup(x => x.Add(It.IsAny<Operation>())).Callback<Operation>(op => {
            dataList.Add(op);
        });

        var queryList = dataList.AsQueryable();
        
        _dbSet.As<IQueryable<Operation>>().Setup(x => x.Provider).Returns(queryList.Provider);
        _dbSet.As<IQueryable<Operation>>().Setup(x => x.Expression).Returns(queryList.Expression);
        _dbSet.As<IQueryable<Operation>>().Setup(x => x.ElementType).Returns(queryList.ElementType);
        _dbSet.As<IQueryable<Operation>>().Setup(x => x.GetEnumerator()).Returns(queryList.GetEnumerator());
        
        _context.Setup(x => x.Operations).Returns(_dbSet.Object);

        _repo = new (_context.Object);

        _usermanager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        _rolemanager = new Mock<RoleManager<Role>>(
            Mock.Of<IRoleStore<Role>>(), null, null, null, null);
       
        _userRepositoryMock = new Mock<UserRepository>(_usermanager.Object);

        _controller = new OperationsController(_repo, _usermanager.Object, _rolemanager.Object, _userRepositoryMock.Object);
    }

    //ControllerTests
    [Fact]
    public async void Controller_ArchiveOperation_Returns_RedirectToActionResult()
    {       
        //Action
        var result = _controller.ArchiveOperation(2);
        
        //Assert
        var redirectRes = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectRes.ActionName);
        Assert.NotEmpty(_repo.ReadArchived());
    }

    
    //RepositoryTests
    [Fact]
    public async void Repository_Create_Adds_To_DbSet() {
        //Arrange
        User refPerson = new();
        User inCharge = new();
        Operation operation = new() {Id = 4, Title = "Op4", Description = "Op4", TargetDate = new DateOnly(), ContractTypeString = "Werkstudent", DepartmentString = "Operations", ReferencePerson =  refPerson, PersonInCharge = inCharge, Tasks = new List<ActiveTask>(), Archived = false};

        //Action
        _repo.Create(operation);

        //Assert
        _dbSet.Verify(d => d.Add(operation), Times.Once);
        _context.Verify(d => d.SaveChanges(), Times.Once);
        Assert.Equal(4, _dbSet.Object.Count());
    }
}