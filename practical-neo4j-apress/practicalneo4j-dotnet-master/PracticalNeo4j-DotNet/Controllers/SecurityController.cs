using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class SecurityController : GraphStoryController
    {
        public string graphstoryUserAuthValue;

        public SecurityController(GraphStoryInterface graphStoryService): base(graphStoryService) { }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookieCollection cookies = Request.Cookies;

            if (cookies[graphstoryUserAuthKey] != null)
            {
                graphstoryUserAuthValue = Request.Cookies["graphstoryUserAuthKey"].Value;
                
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("/");
                return;
            }
        }
	}
}