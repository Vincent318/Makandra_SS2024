using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Replay.Tests.FrontendTests;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Replay.Tests
{
    /// <author>Raphael Huber</author>
    public class DelegateTest : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly string _appUrl = "http://localhost:6969";
        public DelegateTest()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(chromeOptions);
        }

        [Fact]
        public void Delegate()
        {
            while (Authcookie.AuthCookies == null) 
            System.Threading.Thread.Sleep(10);

            _driver.Navigate().GoToUrl($"{_appUrl}");
            Authcookie.AddCookies(_driver);
            _driver.Navigate().GoToUrl($"{_appUrl}/Operations/");
            
            // Open table
            var table_item = _driver.FindElement(By.CssSelector("td.parent[data-child='h_table_0']"));
            table_item.Click();
            
            //Change task-responsible person to Admin
            var selectElement = _driver.FindElement(By.Name("responibleTaskOwner"));
            var select = new SelectElement(selectElement);
            select.SelectByText("Admin");

            //Accept the selection
            var buttonElement = _driver.FindElement(By.CssSelector("div.save-img button"));
            buttonElement.Click();
            IAlert alert = _driver.SwitchTo().Alert();
            alert.Accept();

            //After redirection test if the element has been saved

            // Open table
            table_item = _driver.FindElement(By.CssSelector("td.parent[data-child='h_table_0']"));
            table_item.Click();
            
            //Get the current element of the selector
            selectElement = _driver.FindElement(By.Name("responibleTaskOwner"));
            select = new SelectElement(selectElement);
            var selectedOption = select.SelectedOption;
            string selectedValue = selectedOption.GetAttribute("value");
        

            //Now it should've been changed to the admin
            Assert.Equal("admin@replay.de", selectedValue);



            //2nd test: changing back to the role
            select.SelectByText("IT");

            //Accept the selection
            buttonElement = _driver.FindElement(By.CssSelector("div.save-img button"));
            buttonElement.Click();
            alert = _driver.SwitchTo().Alert();
            alert.Accept();

            //After redirection test if the element has been saved

            // Open table
            table_item = _driver.FindElement(By.CssSelector("td.parent[data-child='h_table_0']"));
            table_item.Click();
            
            //Get the current element of the selector
            selectElement = _driver.FindElement(By.Name("responibleTaskOwner"));
            select = new SelectElement(selectElement);
            selectedOption = select.SelectedOption;
            selectedValue = selectedOption.GetAttribute("value");
        

            //Now it should've been changed to the admin
            Assert.Equal("IT", selectedValue);


            Dispose();
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
