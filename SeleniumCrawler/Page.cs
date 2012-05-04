using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

// fake change to test GIT integration in VS

namespace SeleniumCrawler
{
    public class Page
    {
        private readonly    IWebDriver _browser;

        public String       Title { get; private set; }
        public Uri          Url { get; private set; }
        public Page         Parrent { get; private set; }
        public List<Page>   Children { get; set; }
        public int          PageDepth { get; set; }
        public String       Source;
        public bool         PassedTest { get; set; }
        
        public Page(IWebDriver browser, IWebElement link, Page parrent)
        {
            _browser = browser;
            Parrent = parrent;
            Children = new List<Page>();

            // Root page
            if (link == null || parrent == null)
            {
                Title = "Root page";
                Url = new Uri(_browser.Url);
                PageDepth = 0;
            }
            // All other pages
            else
            {
                Title = link.Text;
                Url = new Uri(link.GetAttribute("href"));
                PageDepth = parrent.PageDepth+1;
            }
            
        }

        public void CollectLinks(Regex regExp, int maxDepth)
        {
            int count = _browser.FindElements(By.TagName("a")).Count;

            for (int i = 0; i < count; i++)
            {
                var webElement = _browser.FindElements(By.TagName("a")).ElementAtOrDefault(i);
                var child = new Page(_browser, webElement, this);
                Children.Add(child);

                var m = regExp.Match(child.Url.AbsoluteUri);
                if (m.Success && PageDepth < maxDepth)
                {
                    webElement.Click();
                    child.Source = _browser.PageSource;
                    child.CollectLinks(regExp, maxDepth);
                    _browser.Navigate().Back();
                }
                
            }
        }
    }
}
