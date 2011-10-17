using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumCrawler
{
    public class Page
    {
        private IWebDriver Browser { get; set; }

        public String LinkName { get; private set; }
        public Uri Url { get; private set; }
        public List<Page> Pages { get; private set; }

        public Page(IWebDriver browser, String name, Uri url)
        {
            Browser = browser;
            LinkName = name;
            Url = url;
        }

        public void CollectLinks()
        {
            Pages = new List<Page>();
            Browser.Navigate().GoToUrl(Url.AbsoluteUri);

            var elements = Browser.FindElements(By.TagName("a"));
            foreach (var e in elements)
            {
                var href = e.GetAttribute("href");
                if (href == null)
                {
                    //href = new string();
                    href = "http://Found_a_tag.without.href";
                }

                //var page = new Page(Browser, e.Text, new Uri(e.GetAttribute("href")));
                var page = new Page(Browser, e.Text, new Uri(href));
                
                // TODO: Make sure still on the same domain
                //if (!Pages.Contains(page))
                Pages.Add(page);
            }
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
