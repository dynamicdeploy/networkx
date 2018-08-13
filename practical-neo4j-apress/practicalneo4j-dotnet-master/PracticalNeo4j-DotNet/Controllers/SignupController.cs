using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Controllers
{

    public class SignupController : Controller
    {
        private GraphStoryInterface graphStoryService;

        public SignupController(GraphStoryInterface graphStoryService) {
            this.graphStoryService = graphStoryService;
        }

        public ActionResult Add(GraphStory graphStory)
        {            
            graphStory = graphStoryService.userInterface.save(graphStory);
      
            if (graphStory.haserror==false)
            {
                return RedirectToRoute(new { controller = "Home", action = "msg", msg = "Thank you," + graphStory.user.username });
            }
            else
            {
                ViewBag.error = graphStory.error;
                return View("~/Views/Home/Index.cshtml");
            }
            
        }
	}
}