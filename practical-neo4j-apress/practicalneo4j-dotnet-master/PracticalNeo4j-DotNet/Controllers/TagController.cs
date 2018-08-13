using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class TagController :  SecurityController
    {
        public TagController(GraphStoryInterface graphStoryService) : base(graphStoryService)  { }

        public JsonResult search(string q)
        {
            MappedContentTag[] mappedContentTag = null;

            if (!String.IsNullOrEmpty(q))
            {
                mappedContentTag = graphStoryService.tagInterface.search(q);
            }

            return Json(mappedContentTag, JsonRequestBehavior.AllowGet);
        }
	}
}