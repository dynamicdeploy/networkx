using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Models;
using PracticalNeo4j_DotNet.Service;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class UserController  : SecurityController
    {
        public UserController(GraphStoryInterface graphStoryService) : base(graphStoryService) { }
        
        public ActionResult Index()
        {
            User user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);

            return View(user);
        }
        public JsonResult Edit(User user)
        {
            user.username = graphstoryUserAuthValue;
            graphStoryService.userInterface.update(user);
            return Json(User);
        }
        public ActionResult Friends()
        {
            ViewBag.following = graphStoryService.userInterface.following(graphstoryUserAuthValue); 

            return View();
        }

        public JsonResult Searchbyusername(String username)
        {
            return Json(graphStoryService.userInterface.searchNotFollowing(graphstoryUserAuthValue, username), JsonRequestBehavior.AllowGet);
        }

        public JsonResult follow(String username)
        {
            return Json(graphStoryService.userInterface.follow(graphstoryUserAuthValue, username), JsonRequestBehavior.AllowGet);
        }

        public JsonResult unfollow(String username)
        {
            return Json(graphStoryService.userInterface.unfollow(graphstoryUserAuthValue, username), JsonRequestBehavior.AllowGet);
        }
	}
}