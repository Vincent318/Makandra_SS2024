using OpenQA.Selenium;
using System.Collections.Generic;


namespace Replay.Tests.FrontendTests;

public static class Authcookie {

    public static List<Cookie> AuthCookies {get; set;}


    public static void AddCookies(IWebDriver _driver) {
        foreach (var cookie in AuthCookies) {
            _driver.Manage().Cookies.AddCookie(cookie);
        }
    }
}   