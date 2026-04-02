using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

/// <author>Noah Engelschall</author>

namespace Replay.Tests
{
        public class TaskListTests : SeleniumBaseTest
        {
                public TaskListTests() : base() { }

                [Fact]
                public void VerifyTaskListPageLoads()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/TaskList");

                        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
                        var header = wait.Until(d => d.FindElement(By.CssSelector(".header-font")));

                        Assert.Contains("AUFGABEN", header.Text);
                }

                [Fact]
                public void ToggleViewButtonWorks()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/TaskList");

                        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
                        var meineButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'Meine')]")));
                        var alleButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'Alle')]")));

                        meineButton.Click();
                        wait.Until(d => d.Url.Contains("viewType=Meine"));
                        Assert.Contains("viewType=Meine", _driver.Url);

                        alleButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'Alle')]")));

                        alleButton.Click();
                        wait.Until(d => d.Url.Contains("viewType=Alle"));
                        Assert.Contains("viewType=Alle", _driver.Url);
                }

                [Fact]
                public void SortButtonWorks()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/TaskList");

                        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
                        var sortIcon = wait.Until(d => d.FindElement(By.Id("sort-icon")));

                        sortIcon.Click();

                        var earliestFirst = wait.Until(d => d.FindElement(By.XPath("//a[text()='Früheste zuerst']")));
                        earliestFirst.Click();

                        var taskRows = wait.Until(d => d.FindElements(By.CssSelector("tbody tr")));
                        var dates = taskRows.Select(row => DateTime.Parse(row.FindElement(By.CssSelector("td:nth-child(3)")).Text)).ToList();
                        var sortedDates = dates.OrderBy(d => d).ToList();
                        Assert.Equal(sortedDates, dates);

                        sortIcon = wait.Until(d => d.FindElement(By.Id("sort-icon")));
                        sortIcon.Click();

                        var latestFirst = wait.Until(d => d.FindElement(By.XPath("//a[text()='Späteste zuerst']")));
                        latestFirst.Click();

                        taskRows = wait.Until(d => d.FindElements(By.CssSelector("tbody tr")));
                        dates = taskRows.Select(row => DateTime.Parse(row.FindElement(By.CssSelector("td:nth-child(3)")).Text)).ToList();
                        sortedDates = dates.OrderByDescending(d => d).ToList();
                        Assert.Equal(sortedDates, dates);
                }

                [Fact]
                public void FilterInputWorks()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/TaskList");

                        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
                        var filterInput = wait.Until(d => d.FindElement(By.Id("filter-input")));

                        filterInput.SendKeys("Testfilter");
                        filterInput.SendKeys(Keys.Enter);

                        wait.Until(d => d.Url.Contains("filter=Testfilter"));
                        Assert.Contains("filter=Testfilter", _driver.Url);
                }

                [Fact]
                public void ClaimButtonWorks()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/TaskList");

                        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
                        var claimButton = wait.Until(d => d.FindElement(By.CssSelector("button.claim-button")));

                        claimButton.Click();

                        var status = wait.Until(d => d.FindElement(By.XPath("//td[text()='in Bearbeitung']")));
                        Assert.NotNull(status);
                }

                [Fact]
                public void CompleteButtonWorks()
                {
                        _driver.Navigate().GoToUrl($"{_appUrl}/Tasklist?viewType=Meine");

                        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
                        var completeButton = wait.Until(d => d.FindElement(By.CssSelector("button.claim-button")));

                        completeButton.Click();
                }
        }
}
