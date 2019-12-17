using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDLCReport.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult searchMap() 
        {
            return View();
        }


        public ActionResult HomeIndex() 
        {
            return View();
        }

        public ActionResult DistanceRoute() 
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult AddNewObject() 
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult distanceList()
        {
            ViewBag.Message = "Your distanceList page.";

            return View();
        }
        public ActionResult PopulateMapObj()
        {
            ViewBag.Message = "Your PopulateMapObj page.";

            return View();
        }
    }
}