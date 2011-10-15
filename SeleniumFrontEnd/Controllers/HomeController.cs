using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeleniumCrawler;


namespace SeleniumFrontEnd.Controllers
{
    public class HomeController : Controller
    {
        //private SeleniumCrawler.EBrowser _browser = EBrowser.None;
        //private Dictionary<string, string> _links;

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the Ultimate tester";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
