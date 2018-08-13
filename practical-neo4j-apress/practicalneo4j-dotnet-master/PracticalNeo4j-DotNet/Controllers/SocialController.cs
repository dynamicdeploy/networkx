using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Models;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class SocialController : SecurityController
    {
        private int limitContent = 4;
        public SocialController(GraphStoryInterface graphStoryService) : base(graphStoryService)  { }
        
        public ActionResult Index(GraphStory graphStory)
        {
            graphStory = graphStoryService.contentInterface.getContent(graphStory, graphstoryUserAuthValue, 0, limitContent);

            return View("~/Views/Social/Posts.cshtml",graphStory);
        }
        public JsonResult Postsfeed(int skip, GraphStory graphStory)
        {
            graphStory = graphStoryService.contentInterface.getContent(graphStory, graphstoryUserAuthValue, skip, limitContent);
            return Json(graphStory,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Viewpost(string contentId)
        {
            return View("~/Views/Social/Post.cshtml", graphStoryService.contentInterface.getContentItem(contentId, graphstoryUserAuthValue));
        }

        // add content
        [HttpPost]
        public JsonResult add(Content jsonObj) {
            MappedContent mappedContent = graphStoryService.contentInterface.add(jsonObj,graphstoryUserAuthValue);
            mappedContent.userNameForPost = graphstoryUserAuthValue;
            return  Json(jsonObj,JsonRequestBehavior.AllowGet);
        }

        //edit content
        public JsonResult edit(Content jsonObj)
        {
            MappedContent mappedContent = graphStoryService.contentInterface.edit(jsonObj, graphstoryUserAuthValue);
            mappedContent.userNameForPost = graphstoryUserAuthValue;
            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }

	    // delete my content
	    public void delete(string contentId) {
               graphStoryService.contentInterface.delete(contentId, graphstoryUserAuthValue);
        }
        
	}
}