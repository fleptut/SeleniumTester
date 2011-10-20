using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace SeleniumCrawler
{
    public class Page
    {
        private IWebDriver Browser { get; set; }

        public String LinkName { get; private set; }
        public Uri Url { get; private set; }
        public List<Page> Pages { get; private set; }
        public int PageDepth { get; private set; }

        public Page(IWebDriver browser, String name, Uri url)
        {
            Browser = browser;
            LinkName = name;
            Url = url;
        }

        public void CollectLinks(Regex regExp, int pageDepth)
        {
            Pages = new List<Page>();
            Browser.Navigate().GoToUrl(Url);

            var elements = Browser.FindElements(By.TagName("a"));
            foreach (var e in elements)
            {
                var href = e.GetAttribute("href");
                if (href != null)
                {
                    // Only add if we match the wanted criteria
                    Match m = regExp.Match(href);
                    if (m.Success)
                    {
                        var page = new Page(Browser, e.Text, new Uri(href));
                        page.PageDepth = pageDepth;
                        Pages.Add(page);
                    }
                    
                }
            }
            Browser.Navigate().Back();
        }

        public List<Page> TestLinks()
        {
            var brokenPages = new List<Page>();

            foreach (var page in Pages)
            {
                Browser.Navigate().GoToUrl(page.Url.AbsoluteUri);

                // TODO: Need to check case sensisivity!!
                if (Browser.PageSource.Contains("404 not found"))
                    brokenPages.Add(page);
            }

            return brokenPages;
        }

        //private bool IsSameDomain(Page page)
        //{
        //    return _rootPage.Contains(page.);
        //}
    }
}
