using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class ConsumptionController : SecurityController
    {
        public ConsumptionController(GraphStoryInterface graphStoryService) : base(graphStoryService) { }

        public ActionResult Index(GraphStory graphStory, int pagenum = 0)
        {
            
            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory = graphStoryService.productInterface.getProducts(graphStory, pagenum);
            int nextpage = pagenum + 1;
            graphStory.nextPageUrl = "/consumption/" + nextpage.ToString();
            
            if(pagenum==0){
                graphStory.productTrail = graphStoryService.productInterface.getProductTrail(graphstoryUserAuthValue);
                return View(graphStory);
            }else{
                
                return View("~/Views/Consumption/product-list.cshtml",graphStory);
            }
            
        }
        public JsonResult CreateUserProductViewRel(int productNodeId, GraphStory graphStory)
        {

            return Json(graphStoryService.productInterface.createUserViewAndReturnViews(graphstoryUserAuthValue, productNodeId), JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult Console(GraphStory graphStory, string tag = "")
        {
            graphStory.usersWithMatchingTags = graphStoryService.productInterface.usersWithMatchingTags(tag); 
            
            return View(graphStory);
        }
	}
}
