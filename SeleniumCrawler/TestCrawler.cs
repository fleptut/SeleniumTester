using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using System.Text.RegularExpressions;
using Selenium;


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
        private int _timerStart = 0;
        private Regex _regExp;
        private int _maxDepth = 0;

        public int NumberOfLinksFound { get; set; }
        public int ElapsedTime { get; set; }
        public Page RootPage { get; private set; }

        public void Init(string url, EBrowser browser, Regex regExp, int maxDepth)
        {
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
            
            // Load root page
            _driver.Navigate().GoToUrl(url);
            RootPage = new Page(_driver, null, null) { Source = _driver.PageSource };
#if DEBUG
            _driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 1));
#else
            _driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, 5));
#endif
        }

       #region Methods

        public void Start()
        {
            // Init proxy server
            int portNo = 9091;
            var serverName = "http://localhost:9090/proxy";
            var request = WebRequest.Create(serverName);
            request.Method = WebRequestMethods.Http.Post;
            String jsonPort = "{ port :" + portNo + " }";

            request.ContentLength = jsonPort.Length;
            var proxyServer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            proxyServer.Write(jsonPort);
            proxyServer.Close();

            // Setup HAR
            request = WebRequest.Create(serverName + "/" + portNo + "/har");
            request.Method = WebRequestMethods.Http.Post;
            String jsonHARInit = "{ initialPageRef=Foo }";
            request.ContentLength = jsonHARInit.Length;
            proxyServer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            proxyServer.Write(jsonHARInit);
            proxyServer.Close();


            ////Collect all a tags
            //var count = _driver.FindElements(By.TagName("a")).Count;

            //for (int i = 0; i < count; i++)
            //{
            //    var webElement = _driver.FindElements(By.TagName("a")).ElementAtOrDefault(i);
            //    var child = new Page(_driver, webElement, RootPage) { Source = _driver.PageSource };
            //    RootPage.Children.Add(child);

            //    var m = _regExp.Match(child.Url.AbsoluteUri);
            //    if (m.Success && RootPage.PageDepth < _maxDepth)
            //    {
            //        webElement.Click();
            //        child.Source = _driver.PageSource;
            //        child.CollectLinks(_regExp, _maxDepth);
            //        _driver.Navigate().Back();
            //    }
            //}

            request = WebRequest.Create(serverName + "/" + portNo + "/har");
            request.Method = WebRequestMethods.Http.Get;
            proxyServer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            proxyServer.Write("");
            proxyServer.Close();

            request = WebRequest.Create(serverName + "/" + portNo);
            request.Method = "DELETE";
            proxyServer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            proxyServer.Write("");
            proxyServer.Close();
        }
        
        //public List<Page> TestLinks()
        //{
        //    var brokenPages = new List<Page>();

        //    foreach (var page in Pages)
        //    {
        //        Browser.Navigate().GoToUrl(page.Url.AbsoluteUri);

        //        // TODO: Need to check case sensisivity!!
        //        if (Browser.PageSource.Contains("404 not found"))
        //            brokenPages.Add(page);
        //    }

        //    return brokenPages;
        //}

        public void TimerStart()
        {
            _timerStart = Environment.TickCount;
        }

        public int TimerStop()
        {
            return Environment.TickCount - _timerStart;
        }

        #endregion

        public void CleanUp()
        {
            if (_driver != null) _driver.Close();
        }
    }
}
