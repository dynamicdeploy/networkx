using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PracticalNeo4j_DotNet.Service;

namespace PracticalNeo4j_DotNet
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("addpost", "posts/add", new { controller = "Social", action = "add" });
            routes.MapRoute("editpost", "posts/edit", new { controller = "Social", action = "edit" });
            routes.MapRoute("deletepost", "posts/delete/{contentId}", new { controller = "Social", action = "delete", contentId = "" });
            routes.MapRoute("tag", "tag/{q}", new { controller = "Tag", action = "search", q = "0" });
            routes.MapRoute("friends", "friends", new { controller = "User", action = "Friends" });
            routes.MapRoute("useredit", "user/edit", new { controller = "User", action = "edit" });
            routes.MapRoute("searchbyusername", "searchbyusername/{username}", new { controller = "User", action = "searchbyusername", username = "" });
            routes.MapRoute("follow", "follow/{username}", new { controller = "User", action = "follow", username="" });
            routes.MapRoute("unfollow", "unfollow/{username}", new { controller = "User", action = "unfollow", username = "" });
            routes.MapRoute("postsfeed", "postsfeed/{skip}", new { controller = "Social", action = "Postsfeed", skip=0 });
            routes.MapRoute("viewpost", "viewpost/{contentId}", new { controller = "Social", action = "Viewpost", contentId="0" });
            routes.MapRoute("productsearch", "productsearch/{q}", new { controller = "Location", action = "Productsearch", q = "0" });
            routes.MapRoute("consumptionConsole", "consumption/Console", new { controller = "Consumption", action = "Console", tag = UrlParameter.Optional});
            routes.MapRoute("consumption", "consumption/{pagenum}", new { controller = "Consumption", action = "Index", pagenum = 0 });
            routes.MapRoute("consumptionAdd", "consumption/add/{productNodeId}", new { controller = "Consumption", action = "CreateUserProductViewRel", productNodeId = 0 });
            routes.MapRoute("Interest", "Interest", new { controller = "Interest", action = "Index", tag = UrlParameter.Optional, userscontent = UrlParameter.Optional });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
