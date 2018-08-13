using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class InterestController :  SecurityController
    {
        public InterestController(GraphStoryInterface graphStoryService) : base(graphStoryService)  { }

        public ActionResult Index(GraphStory graphStory, string tag, string usercontent)
        {
            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory = graphStoryService.tagInterface.tagsInMyNetwork(graphStory);

            if (!String.IsNullOrEmpty(tag) && !String.IsNullOrEmpty(usercontent))
            {
                graphStory.content = graphStoryService.contentInterface.getContentByTag(graphStory.user.username, tag, Boolean.Parse(usercontent));
            }

            return View(graphStory);
        }
	}
}