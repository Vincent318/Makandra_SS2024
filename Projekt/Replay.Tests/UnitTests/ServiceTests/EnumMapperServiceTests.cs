using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Replay.Services;
using Replay.Enumerations;

namespace Replay.Tests.UnitTests.ServiceTests
{
    /// <summary>
    /// This class tests the EnumMapperService
    /// Theory in combination with InlineData tests a method with selected data 
    /// same principle for the whole test class
    /// </summary>
    /// <author> Robert Figl </author>

    /// <summary>
    /// This class tests the EnumMapperService
    /// Theory in combination with InlineData tests a method with selected data 
    /// same principle for the whole test class
    /// </summary>
    /// <author> Robert Figl </author>
    public class EnumMapperServiceTests
    {
        /// <summary>
        ///  checking if GetDepartmentName Returns correct names
        /// </summary>
        /// <param name="department">the Department which will be converted</param>
        /// <param name="expected">the expected outcome</param>
        /// <summary>
        ///  checking if GetDepartmentName Returns correct names
        /// </summary>
        /// <param name="department">the Department which will be converted</param>
        /// <param name="expected">the expected outcome</param>
        /// <author> Robert Figl </author>
        [Theory]
        [InlineData(Department.BACKOFFICE, "Backoffice")]
        [InlineData(Department.ENTWICKLUNG, "Entwicklung")]
        [InlineData(Department.OPERATIONS, "Operations")]
        [InlineData(Department.PROJEKTMANAGEMENT, "Projektmanagement")]
        [InlineData(Department.SALES, "Sales")]
        [InlineData(Department.UIUX, "UI/UX")]
        public void GetDepartmentName_ShouldReturnCorrectName(Department department, string expected)
        {
            var result = EnumMapperService.GetDepartmentName(department);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// checking if  GetTaskStatusName returns correct status
        /// </summary>
        /// <param name="status">the status which will be converted</param>
        /// <param name="expected">the expected result</param>
        /// <author> Robert Figl </author>
        [Theory]
        [InlineData(Enumerations.TaskStatus.UNBEARBEITET, "unbearbeitet")]
        [InlineData(Enumerations.TaskStatus.BEARBEITET, "bearbeitet")]
        [InlineData(Enumerations.TaskStatus.IN_BEARBEITUNG, "in Bearbeitung")]
        public void GetTaskStatusName_ShouldReturnCorrectName(Enumerations.TaskStatus status, string expected)
        {
            var result = EnumMapperService.GetTaskStatusName(status);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// checking if GetTaskStatus works correctly
        /// </summary>
        /// <param name="status">the status which will be converted</param>
        /// <param name="expected">the expected result</param>
        /// <author> Robert Figl </author>   
        [Theory]
        [InlineData("unbearbeitet", Enumerations.TaskStatus.UNBEARBEITET)]
        [InlineData("bearbeitet", Enumerations.TaskStatus.BEARBEITET)]
        [InlineData("in Bearbeitung", Enumerations.TaskStatus.IN_BEARBEITUNG)]
        public void GetTaskStatus_ShouldReturnCorrectEnum(string status, Enumerations.TaskStatus expected)
        {
            var result = EnumMapperService.GetTaskStatus(status);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// checking if GetContractType works correctly
        /// </summary>
        /// <param name="contract">the contract type which will be converted</param>
        /// <param name="expected">the expected result</param>
        /// <author> Robert Figl </author> 
        [Theory]
        [InlineData(ContractType.WERKSTUDENT, "Werkstudent")]
        [InlineData(ContractType.FESTANSTELLUNG, "Festanstellung")]
        [InlineData(ContractType.PRAKTIKUM, "Praktikum")]
        [InlineData(ContractType.TRAINEE, "Trainee")]
        public void GetContractType_ShouldReturnCorrectName(ContractType contract, string expected)
        {
            var result = EnumMapperService.GetContractType(contract);
            Assert.Equal(expected, result);
        }
    }
}