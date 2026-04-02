using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V125.Target;
using Xunit;


namespace Replay.Tests.FrontendTests.AuthenticationTests;

public class AuthenticationTests {

     private readonly IWebDriver _driver;
    private readonly string _appUrl = "http://localhost:6969";
    public AuthenticationTests() {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--ignore-certificate-errors");
        chromeOptions.AddArgument("--disable-search-engine-choice-screen");
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-dev-shm-usage");

        _driver = new ChromeDriver(chromeOptions);
    }

    [Fact]
    public void LoginTest() {

        try {
            string email = "admin@replay.de";
            string password = "Admin1!";

            _driver.Navigate().GoToUrl($"{_appUrl}/Account/Login");

            var emailInput = _driver.FindElement(By.Id("email"));
            var passwordInput = _driver.FindElement(By.Id("password"));
            var submitButton = _driver.FindElement(By.XPath("//input[@type='submit']"));

            emailInput.SendKeys(email);
            passwordInput.SendKeys(password);

            submitButton.Click();

            System.Threading.Thread.Sleep(1000);

            Assert.Equal($"{_appUrl}/", _driver.Url);
            var authCookies = new List<Cookie>(_driver.Manage().Cookies.AllCookies);
            Authcookie.AuthCookies = authCookies;
        } finally {
            Dispose();
        }

        
        
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }


}