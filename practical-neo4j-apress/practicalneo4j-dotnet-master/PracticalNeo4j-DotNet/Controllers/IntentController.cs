using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class IntentController : SecurityController
    {
        public IntentController(GraphStoryInterface graphStoryService) : base(graphStoryService) { }

        public ActionResult Index(GraphStory graphStory)
        {
            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory.mappedProductUserPurchases = graphStoryService.purchaseInterface.friendsPurchase(graphStory.user.userId);

            ViewBag.Title = "Products Purchased by Friends";
            graphStory.showform = false;

            return View(graphStory);
        }
        public ActionResult friendsPurchaseByProduct(GraphStory graphStory, string producttitle ="")
        {
            if(String.IsNullOrEmpty(producttitle)){
                producttitle = "Star Wars Mimobot Thumb Drives";
            }

            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory.mappedProductUserPurchases = graphStoryService.purchaseInterface.friendsPurchaseByProduct(graphStory.user.userId, producttitle);
            graphStory.producttitle = producttitle;

            ViewBag.Title = "Specific Products Purchased by Friends";
            graphStory.showform = true;
            
            return View("~/Views/Intent/Index.cshtml", graphStory);
        }
        public ActionResult friendsPurchaseTagSimilarity(GraphStory graphStory)
        {
            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory.mappedProductUserPurchases = graphStoryService.purchaseInterface.friendsPurchaseTagSimilarity(graphStory.user.userId);

            ViewBag.Title = "Products Purchased by Friends and Matches Users Tags";
            graphStory.showform = false;
            return View("~/Views/Intent/Index.cshtml", graphStory);
        }
        public ActionResult friendsPurchaseTagSimilarityAndProximityToLocation(GraphStory graphStory)
        {
            double dist = 10.00;
            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory.mappedUserLocation = graphStoryService.userInterface.getUserLocation(graphstoryUserAuthValue);
            graphStory.mappedProductUserPurchases = graphStoryService.purchaseInterface.friendsPurchaseTagSimilarityAndProximityToLocation(
                    graphStory.mappedUserLocation.lat, graphStory.mappedUserLocation.lon, dist, graphStory.user.userId);

            ViewBag.Title = "Products Purchased by Friends Nearby and Matches Users Tags";
            graphStory.showform = false;
            
            return View("~/Views/Intent/Index.cshtml", graphStory);
        }
	}
}