using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using Xunit;
using Replay.Models;
using System.Linq;
using System;


namespace Replay.Tests.FrontendTests.ProcessTests;

public class CreateProcessTest {
    private readonly IWebDriver _driver;
    private readonly string _appUrl = "http://localhost:6969";

    public CreateProcessTest() {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--ignore-certificate-errors");
         chromeOptions.AddArgument("--disable-search-engine-choice-screen");
         chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-dev-shm-usage");

        _driver = new ChromeDriver(chromeOptions);
        
    }

    [Fact]
    public void CreateProcessTest_Create_With_Tasks()
    {   
        try { 
            while (Authcookie.AuthCookies == null) 
            System.Threading.Thread.Sleep(10);

            _driver.Navigate().GoToUrl($"{_appUrl}");
            Authcookie.AddCookies(_driver);
            _driver.Navigate().GoToUrl($"{_appUrl}/Processes/CreateProcess");

            var titleInput = _driver.FindElement(By.Id("title"));
            var descriptionInput = _driver.FindElement(By.Id("description"));
            var roleBtns = _driver.FindElements(By.ClassName("role-button"));
            var submitBtn = _driver.FindElement(By.XPath("//button[@type='submit']"));

            titleInput.SendKeys("OnboardingTest");
            descriptionInput.SendKeys("Dieser Prozess wird für Onboardings verwendet");

            foreach (var role in roleBtns) {
                if (role.Text == "Personal") {
                    role.Click();
                }
            }
            submitBtn.Click();

            var addTaskBtn = _driver.FindElement(By.Id("add-task-btn"));

            addTaskBtn.Click();
            System.Threading.Thread.Sleep(1000);
            CreateTask(_driver, "Laptop einrichten", "Neuer Laptop neues Glueck", new string[]{"Entwicklung", "Operations", "UI/UX", "Projektmanagement", "Backoffice", "Sales"}, new string[]{"Werkstudent", "Festanstellung", "Praktikum"}, "Am ersten Arbeitstag", "Bezugsperson");
            addTaskBtn = _driver.FindElement(By.Id("add-task-btn"));
            addTaskBtn.Click();
            System.Threading.Thread.Sleep(1000);
            CreateTask(_driver, "ssh-key anlegen", "Ein ssh key muss angelegt werden", new string[]{"Entwicklung", "Operations"}, new string[]{"Werkstudent", "Festanstellung"}, "ASAP", "Vorgangsverantwortiche(r)");

            var toggleTableBtn = _driver.FindElement(By.Id("toggle-table"));
            toggleTableBtn.Click();
            System.Threading.Thread.Sleep(1000);

            _driver.FindElement(By.XPath("//*[text()='Onboarding']"));
            Assert.Equal(24, Int32.Parse(_driver.FindElement(By.Id("task-count")).Text));
        } finally {
            Dispose();
        }
        
    }
    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }


    public void CreateTask(IWebDriver driver, string title, string description, string[] departments, string[] contractTypes, string duedate, string responsible) {
        var taskTitleInput = _driver.FindElement(By.Id("title"));
        var taskDescriptionInput = _driver.FindElement(By.Id("description"));
        var taskDepartmentBtns = _driver.FindElements(By.ClassName("select-btn-dpts"));
        var taskDueDateDropdown = _driver.FindElement(By.Id("dueDate"));
        SelectElement selectDueDate = new SelectElement(taskDueDateDropdown);
        var taskResponsibleDopdown = _driver.FindElement(By.Id("responsible"));
        SelectElement selectResponsible = new SelectElement(taskResponsibleDopdown);
        var taskContractTypeBtns = _driver.FindElements(By.ClassName("contract-types"));
        var taskSubmitBtn = _driver.FindElement(By.XPath("//button[@type='submit']"));

        
        taskTitleInput.SendKeys(title);
        taskDescriptionInput.SendKeys(description);
        foreach (var dep in taskDepartmentBtns) {
            if (departments.Contains(dep.Text)) {
                dep.Click();
            }
        }
        foreach (var contract in taskContractTypeBtns) {
            if (contractTypes.Contains(contract.Text)) {
                contract.Click();
            }
        }
        
        selectDueDate.SelectByText(duedate);
        selectResponsible.SelectByText(responsible);
        taskSubmitBtn.Click();
    }

}

