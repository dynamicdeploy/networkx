using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PracticalNeo4j_DotNet.ViewModels
{
    public class MappedProduct
    {
        public string nodeId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string tagstr { get; set; }
    }
}