using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System.Text.RegularExpressions;


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
        private List<Page> _collectedPages;
        private List<Page> _testedPages;
        private int _timerStart = 0;
        private Regex _regExp;
        private int _depth = 0;
        private int _maxDepth = 0;

        public int NumberOfLinksFound { get; set; }
        public int ElapsedTime { get; set; }
        //public Dictionary<string, Page> Links { get; private set; }

        public void Init(string url, EBrowser browser, Regex regExp, int maxDepth)
        {
            _testedPages = new List<Page>();
            _collectedPages = new List<Page>();
            _timerStart = Environment.TickCount;
            _regExp = regExp;
            _maxDepth = maxDepth;

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

       #region Methods

        public void LinkTest()
        {
            LoadPage(_rootPage);
            CleanUp();
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
            return _collectedPages.Count;
        }

        public List<Page> GetCollectedLinks()
        {
            return _collectedPages;
        }

        private void LoadPage(Page page)
        {
            //Only follow links to max depth, max depth == 0 => infinite
            if (_depth < _maxDepth || _maxDepth == 0)
            {
                page.CollectLinks(_regExp, _depth++);
                _testedPages.Add(page);
                _collectedPages.AddRange(page.Pages);
                

                foreach (var p in page.Pages)
                {
                    if (!_testedPages.Any(x => x.Url.Equals(p.Url)))
                    {
                        p.CollectLinks(_regExp, _depth);
                        _collectedPages.AddRange(p.Pages);
                        LoadPage(p);
                    }
                }
            }
        }

        #endregion

        public void CleanUp()
        {
            if (_driver != null) _driver.Close();
        }
    }
}
