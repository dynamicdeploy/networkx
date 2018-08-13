using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class LocationController : SecurityController
    {
        public LocationController(GraphStoryInterface graphStoryService) : base(graphStoryService) { }

        public ActionResult Index(GraphStory graphStory, double distance = 0, string productNodeId="")
        {
            graphStory.user = graphStoryService.userInterface.getByUserName(graphstoryUserAuthValue);
            graphStory.mappedUserLocation = graphStoryService.userInterface.getUserLocation(graphstoryUserAuthValue);

            if(distance != 0){
                if(!String.IsNullOrEmpty(productNodeId)){
                    graphStory.productNodeId = Convert.ToInt64(productNodeId);
                    graphStory = graphStoryService.productInterface.getProduct(graphStory);
                    graphStory = graphStoryService.locationInterface.returnLocationsWithinDistanceAndHasProduct(graphStory,graphStory.mappedUserLocation.lat,graphStory.mappedUserLocation.lon,distance);
                    
                }
                else
                {
                    graphStory.mappedLocations = graphStoryService.locationInterface.returnLocationsWithinDistance(graphStory.mappedUserLocation.lat, graphStory.mappedUserLocation.lon, distance,"business");
                }
            }

            return View(graphStory);
        }
        public JsonResult Productsearch(string q)
        {
            MappedProductSearch[] mappedProductSearch = null;

            if(!String.IsNullOrEmpty(q)){
                mappedProductSearch = graphStoryService.productInterface.search(q);
            }
            
            return Json(mappedProductSearch, JsonRequestBehavior.AllowGet);
        }
	}
}