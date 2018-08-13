using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class LoginController : GraphStoryController
    {

        public LoginController(GraphStoryInterface graphStoryService) : base(graphStoryService) { }

        public ActionResult Index(GraphStory graphStory)
        {
            graphStory = this.graphStoryService.userInterface.login(graphStory);
            
            if (graphStory.haserror == false)
            {
                HttpCookie userCookie = new HttpCookie(graphstoryUserAuthKey);
                userCookie.Value = graphStory.user.username;
                userCookie.Expires = DateTime.Now.AddDays(20);
                Response.Cookies.Add(userCookie);

                return RedirectToRoute(new { controller = "Social", action = "Index" });
            }
            else
            {
                ViewBag.error = graphStory.error;
                return View("~/Views/Home/Index.cshtml");
            }
        }
	}
}