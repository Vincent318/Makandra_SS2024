//author: Vincent Arnold
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Replay.Tests.IntegrationTests
{
    public class RoleFrontendTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _appUrl = "http://localhost:6969";

        public RoleFrontendTest()
        {
            // Initialize the WebDriver for Chrome
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            _driver = new ChromeDriver(chromeOptions);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Fact]
        public void CreateRoleAndVerifyInIndex_ReturnsSuccess()
        {
            // Step 1: Login
            _driver.Navigate().GoToUrl($"{_appUrl}/Account/Login");

            var emailInput = _driver.FindElement(By.Id("email"));
            var passwordInput = _driver.FindElement(By.Id("password"));
            var submitButton = _driver.FindElement(By.XPath("//input[@type='submit']"));

            emailInput.SendKeys("admin@replay.de"); // Change email and password accordingly
            passwordInput.SendKeys("Admin1!");        // Change password accordingly
            submitButton.Click();

            // Wait for redirection to the home page after login
            System.Threading.Thread.Sleep(2000);
            Assert.Equal($"{_appUrl}/", _driver.Url);

            // Step 2: Navigate to Role Creation Page
            _driver.Navigate().GoToUrl($"{_appUrl}/Roles/CreateRole");

            // Step 3: Fill in the Role Creation Form
            var titleInput = _driver.FindElement(By.Name("Title"));
            var descriptionInput = _driver.FindElement(By.Name("Description"));
            var roleSubmitButton = _driver.FindElement(By.CssSelector("input[type='submit']"));

            titleInput.SendKeys("Test Role");
            descriptionInput.SendKeys("This is a test role description.");
            roleSubmitButton.Click();

            // Step 4: Navigate to the Roles Index Page
            _driver.Navigate().GoToUrl($"{_appUrl}/Roles");

            // Step 5: Verify that the new role appears in the index
            var pageSource = _driver.PageSource;

            Assert.Contains("Test Role", pageSource);
            Assert.Contains("This is a test role description.", pageSource);
        }

        public void Dispose()
        {
            // Close the WebDriver after the test
            _driver.Quit();
            _driver.Dispose();
        }
    }
}