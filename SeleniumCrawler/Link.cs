using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeleniumCrawler
{
    public class Link
    {
        public string   Name { get; set; }
        public Uri      Href { get; set; }
        public bool     Tested { get; set; }
        public bool     PassedTest { get; set; }

        public Link()
        {
            Tested = false;
            PassedTest = true;
        }
    }
}
