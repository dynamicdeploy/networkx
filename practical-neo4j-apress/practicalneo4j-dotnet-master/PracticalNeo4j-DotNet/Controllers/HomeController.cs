using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PracticalNeo4j_DotNet.Models;
namespace PracticalNeo4j_DotNet.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Msg(string msg)
        {
            ViewBag.Message = msg;
            return View();
        }
	}  
}