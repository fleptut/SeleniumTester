using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeleniumCrawler;

namespace SeleniumFrontEnd.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        private CrawlTests _tester = new CrawlTests();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CollectLinks(FormCollection col)
        {
            SeleniumCrawler.EBrowser browser = EBrowser.None;
            string rootUrl = col["rootUrl"];

            if (rootUrl.Length <= 0)
                return RedirectToAction("Index", "Home");

            if (!rootUrl.Contains("://"))
                rootUrl = "http://" + rootUrl;

             if (Convert.ToInt32(col["browser"]) == 1)
                 browser = EBrowser.Firefox;
             else if (Convert.ToInt32(col["browser"]) == 2)
                browser = EBrowser.InternetExplorer;
            else if (Convert.ToInt32(col["browser"]) == 3)
                browser = EBrowser.Chrome;


            _tester.Init(rootUrl, browser);
            _tester.TimerStart();
            _tester.LinkTest();
            ViewBag.NumberOfLinksFound = _tester.GetCollectedLinks().Count;
            ViewBag.FailedTests = 0;
            ViewBag.ElapsedTime = _tester.TimerStop();
            ViewBag.Links = _tester.GetCollectedLinks();
            
            return View();
        }

        public ActionResult ViewCollectedLinks()
        {
            ViewBag.Links = _tester.GetCollectedLinks();

            return View();
        }

    }
}
