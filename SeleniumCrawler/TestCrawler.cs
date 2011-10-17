using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;


namespace SeleniumCrawler
{
    public enum EBrowser
    {
        None = 0,
        Firefox,
        InternetExplorer,
        Chrome
    }

    public class CrawlTests
    {
        private IWebDriver _driver;
        private EBrowser _browser = EBrowser.None;
        private Page _rootPage;
        private int _timerStart = 0;

        public int NumberOfLinksFound { get; set; }
        public int ElapsedTime { get; set; }
        //public Dictionary<string, Page> Links { get; private set; }

        public void Init(string url, EBrowser browser)
        {
            //Links = new Dictionary<string, Page>();
            _timerStart = Environment.TickCount;

            // Set which browser to test
            _browser = browser;

            switch (_browser)
            {
                case EBrowser.Chrome:
                    _driver = new ChromeDriver();
                    break;
                case EBrowser.Firefox:
                    _driver = new FirefoxDriver();
                    break;
                case EBrowser.InternetExplorer:
                    _driver = new InternetExplorerDriver();
                    break;
                default:
                    _driver = new RemoteWebDriver(DesiredCapabilities.HtmlUnitWithJavaScript());
                    break;
            }

            _rootPage = new Page(_driver, "Root page", new Uri(url));
            _driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 30));
        }

        //public void TestSetUp()
        //{
        //    // Go to root page of site to be tested
        //    LoadPage(_url);
        //}


        //[Test]
        //public void CollectLinks(string url)
        //{

        //    NumberOfLinksFound = Links.Count;
        //    ElapsedTime = Environment.TickCount - _timerStart;

        //    while (_counter >= 0)
        //    {
        //        foreach (var link in Links)
        //        {
        //            _driver.Navigate().GoToUrl(link.Key);
        //            CollectLinks(link.Key);
        //        }
        //        _counter--;
        //    }

        //}

        #region Methods

        public void LinkTest()
        {
            LoadPage(_rootPage);
        }

        public void TimerStart()
        {
            _timerStart = Environment.TickCount;
        }

        public int TimerStop()
        {
            return Environment.TickCount - _timerStart;
        }

        public int NumberOfCollectedLinks()
        {
            return _rootPage.Pages.Count;
        }

        public List<Page> GetCollectedLinks()
        {
            return _rootPage.Pages;
        }

        private void LoadPage(Page page)
        {
            page.CollectLinks();
            //_driver.Close();
            //var brokenPages = page.TestLinks();


            // Add any new links
            //AddNewLinks(links);

            foreach (var p in page.Pages)
            {
                p.CollectLinks();
                _driver.Navigate().Back();
                //LoadPage(p);
            }
        }

        //private void AddNewLinks(IEnumerable<Link> links)
        //{
        //    foreach (var link in links.Where(link => !Links.ContainsKey(link.Href.AbsoluteUri) &&
        //                                                    IsSameDomain(link.Href)))
        //    {
        //        Links.Add(link.Href.AbsolutePath, link);
        //    }
        //}

        //private IEnumerable<Link> CollectLinks()
        //{
        //    var elements = _driver.FindElements(By.TagName("a"));

        //    // Collect all links
        //    return elements.Select(e => new Link {Href = new Uri(e.GetAttribute("href")), Name = e.Text}).ToList();
        //}

        #endregion

        public void CleanUp()
        {
            if (_driver != null) _driver.Close();
        }
    }
}
