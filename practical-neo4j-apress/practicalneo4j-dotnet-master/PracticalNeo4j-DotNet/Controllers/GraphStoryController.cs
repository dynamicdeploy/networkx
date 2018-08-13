using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PracticalNeo4j_DotNet.Service;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Controllers
{
    public class GraphStoryController : Controller
    {
        public string graphstoryUserAuthKey = "graphstoryUserAuthKey";
        public GraphStoryInterface graphStoryService;

        public GraphStoryController(GraphStoryInterface graphStoryService)
        {
            this.graphStoryService = graphStoryService;
        }

	}
}