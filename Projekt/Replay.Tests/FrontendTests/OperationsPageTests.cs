using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;
using Replay.Tests.FrontendTests;
using System.Threading;

namespace Replay.Tests.FrontendTests
{
    /// <summary>
    /// Test if the user can see the progress bars
    /// </summary>
    /// <author> Robert Figl </author>
    public class OperationsPageTests : IDisposable
    {

        // IWebDriver enables automatization of web browsers
        // Webdrivers interacts with a web browser and allows automatic tests og web applications
        // This interface offers different methods for interating with a website and navigating the browser
        // Navigation:              - Navigate().GoToUrl(string url): Opens the Url in the Browser
        //                          - Navigate().Back(): Goes back in the browser history
        //                          - Navigate().Forward(): Goes forward in the browser history
        //                          - Navigate().Refresh(): Refreshes the current page
        // Finding Elements:        - FindElement(By by): finds the first element which fulfills the criteria
        //                          - FindElements(By by): finds all elements which fulfills the criteria
        // Browser Interaction:     - Manage().Cookies: Manages the Cookies of the browser
        //                          - Manage().Timeouts: Manages the timeouts of the browser        
        //                          - Manage().Window: Manages the window of the browser (maximize, minimize)
        //                          - Manage().TimesOuts(): Defines time limits when searching for elements
        private readonly IWebDriver _driver;

        // Url which is going to be tested saved as: _appUrl
        private readonly string _appUrl = "http://localhost:6969";

    
       
       /// <summary>
       ///  Constrcutor asserts that the test functions without complications by using ChromeOptions and initialises ChromDriver
       /// </summary>
       /// <author> Robert Figl </author>
        public OperationsPageTests()
        {
            // ChromeOptions is a class used to configure diffent setting for the ChromeDriver
            // most inportant is the AddArgument Method of ChromeOptions
            var chromeOptions = new ChromeOptions();
            // ignore certificates
            chromeOptions.AddArgument("--ignore-certificate-errors");
            // disables selecting search engine select browser window
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");

            _driver = new ChromeDriver(chromeOptions);
        }

        /// <summary>
        /// This test simualtes a User making an overview of the operation progress by looking at the progress bar
        /// on the index operation page.
        /// This test checks if progress bars and its css values are found.
        /// </summary>
        /// <author> Robert Figl </author>
        [Fact]
        public void TestOverviewProgress()
        {

            // giving the website time to load
            Thread.Sleep(4000);

            // Go to the basic page
            _driver.Navigate().GoToUrl(_appUrl);

            Thread.Sleep(4000);

            // logging in with cookies to enable test
            Authcookie.AddCookies(_driver);

            Thread.Sleep(4000);
            // Navigate to the Operation view, which is going to be tested
            _driver.Navigate().GoToUrl($"{_appUrl}/Operations/Index");

            // adding time, so the page can load fully
            Thread.Sleep(4000);

            // Checking if we are on the right site
            var pageTitle = _driver.Title;
            Assert.Equal("VORGÄNGE - Replay", pageTitle);

            Thread.Sleep(4000);

            // finding the progress bars to check the status of the operation
            var progressBars = _driver.FindElements(By.CssSelector(".progress"));

            // checking if progress bars are found
            // if none are found, the test failes and prints an message
            Assert.True(progressBars.Count > 0, "No progress bars found on the page.");

            // looking over every progressBar, for more checks 
            foreach(var progressBar in progressBars)
            {
                Thread.Sleep(4000);
                // Looks at the width of the progress Bar
                var width = progressBar.GetCssValue("width");

                // Looks at the label of the progress Bar
                var progressLabel = progressBar.FindElement(By.CssSelector(".progress-label")).Text;

                // Checks if the progressbar is found 
                // width Null or Empty does not mean that no tasks were done, but that the width could not be found
                // an undone progress bar has the same width as a completed progress bar, but one is gray the other is red
                Assert.False(string.IsNullOrEmpty(width), "Progress bar width is not as expected.");
            }
        }

        // Disposing the browser after the test is done
        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

    }
}