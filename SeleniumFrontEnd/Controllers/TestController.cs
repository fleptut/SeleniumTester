using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeleniumCrawler;
using System.Text.RegularExpressions;
using System.Text;

namespace SeleniumFrontEnd.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        private CrawlTests _tester = new CrawlTests();

        public Regex RegExp { get; private set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CollectLinks(FormCollection col)
        {
            SeleniumCrawler.EBrowser browser = EBrowser.None;
            string rootUrl = col["rootUrl"];

            //if (rootUrl.Length <= 0)
            //    return RedirectToAction("Index", "Home");

            if (!rootUrl.Contains("://"))
                rootUrl = "http://" + rootUrl;

             if (Convert.ToInt32(col["browser"]) == 1)
                 browser = EBrowser.Firefox;
             else if (Convert.ToInt32(col["browser"]) == 2)
                browser = EBrowser.InternetExplorer;
            else if (Convert.ToInt32(col["browser"]) == 3)
                browser = EBrowser.Chrome;

            var builder = new StringBuilder();
            string regExpPattern = "{0}://{1}(\\.{2})(\\.{3})";

            if (col["custom"].Length > 0)
                regExpPattern = col["custom"];
            else
            {
                string protocol = "(http|https)";
                string serverName = @"[\w\-_]+";
                string domain = @"[\w\-_]+";
                string extension = @"[\w\-_]+";

                if (col["protocol"].Length > 0)
                    protocol = col["protocol"];

                if (col["server"].Length > 0)
                    serverName = col["server"];

                if (col["domain"].Length > 0)
                    domain = col["domain"];

                if (col["extension"].Length > 0)
                    extension = col["extension"];

                builder.AppendFormat(regExpPattern, protocol, serverName, domain, extension);
            }
            RegExp = new Regex(builder.ToString());

            int maxDepth = 0;
            if (col["depth"].Length > 0)
                maxDepth = Convert.ToInt32(col["depth"]);


            _tester.Init(rootUrl, browser, RegExp, maxDepth);
            _tester.TimerStart();
            _tester.Start();
            //ViewBag.NumberOfLinksFound = _tester.GetCollectedLinks().Count;
            //ViewBag.FailedTests = 0;
            ViewBag.ElapsedTime = _tester.TimerStop();
            ViewBag.RootPage = _tester.RootPage;
            _tester.CleanUp();

            return View();
        }

        public ActionResult ViewCollectedLinks()
        {
            //ViewBag.Links = _tester.GetCollectedLinks();

            return View();
        }

    }
}
