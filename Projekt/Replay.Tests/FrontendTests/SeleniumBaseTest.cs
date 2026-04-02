using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

/// <author>Noah Engelschall</author>

namespace Replay.Tests
{
        public class SeleniumBaseTest : IDisposable
        {
                protected IWebDriver _driver;
                protected string _appUrl = "http://localhost:6969";

                public SeleniumBaseTest()
                {
                        var chromeOptions = new ChromeOptions();
                        chromeOptions.AddArgument("--ignore-certificate-errors");
                        chromeOptions.AddArgument("--disable-popup-blocking");
                        chromeOptions.AddArgument("--disable-default-apps");
                        chromeOptions.AddArgument("--no-sandbox");
                        chromeOptions.AddArgument("--disable-dev-shm-usage");
                        string userDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chrome-user-data");
                        chromeOptions.AddArgument($"--user-data-dir={userDataDir}");

                        // Dynamically set the path to first_run_preferences.json
                        string projectRootPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
                        string prefsPath = Path.Combine(projectRootPath, "config", "first_run_preferences.json");

                        if (File.Exists(prefsPath))
                        {
                                var prefs = File.ReadAllText(prefsPath);
                                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(prefs);
                                foreach (var kvp in dict)
                                {
                                        chromeOptions.AddUserProfilePreference(kvp.Key, kvp.Value);
                                }
                        }

                        _driver = new ChromeDriver(chromeOptions);

                        PerformLogin("admin@replay.de", "Admin1!");
                }

                protected void PerformLogin(string email, string password)
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/Account/Login");

                        var emailInput = _driver.FindElement(By.Id("email"));
                        var passwordInput = _driver.FindElement(By.Id("password"));
                        var submitButton = _driver.FindElement(By.XPath("//input[@type='submit']"));

                        emailInput.SendKeys(email);
                        passwordInput.SendKeys(password);

                        submitButton.Click();

                        System.Threading.Thread.Sleep(1000);

                        Assert.Equal($"{_appUrl}/", _driver.Url);
                }

                public void Dispose()
                {
                        _driver.Quit();
                        _driver.Dispose();
                }
        }
}
