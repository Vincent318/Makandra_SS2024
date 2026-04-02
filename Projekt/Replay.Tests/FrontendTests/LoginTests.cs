using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

/// <author>Noah Engelschall</author>

namespace Replay.Tests
{
        public class LoginTests : IDisposable
        {
                private readonly IWebDriver _driver;
                private readonly string _appUrl = "http://localhost:6969";
                public LoginTests()
                {
                        var chromeOptions = new ChromeOptions();
                        chromeOptions.AddArgument("--ignore-certificate-errors");
                        chromeOptions.AddArgument("--disable-search-engine-choice-screen");
                        chromeOptions.AddArgument("--no-sandbox");
                        chromeOptions.AddArgument("--disable-dev-shm-usage");
                        _driver = new ChromeDriver(chromeOptions);
                }

                [Fact]
                public void InitialLoginTest()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/Initial");

                        var firstNameField = _driver.FindElement(By.Id("firstname"));
                        var lastNameField = _driver.FindElement(By.Id("lastname"));
                        var emailField = _driver.FindElement(By.Id("email"));
                        var passwordField = _driver.FindElement(By.Id("password"));
                        var confirmPasswordField = _driver.FindElement(By.Name("ConfirmPassword"));
                        var submitButton = _driver.FindElement(By.XPath("//input[@type='submit']"));

                        firstNameField.SendKeys("John");
                        lastNameField.SendKeys("Doe");
                        emailField.SendKeys("john.doe@example.com");
                        passwordField.SendKeys("Password123!");
                        confirmPasswordField.SendKeys("Password123!");

                        submitButton.Click();

                        System.Threading.Thread.Sleep(2000);
                        Assert.Equal($"{_appUrl}/", _driver.Url);
                }

                public void Dispose()
                {
                        _driver.Quit();
                        _driver.Dispose();
                }
        }
}
