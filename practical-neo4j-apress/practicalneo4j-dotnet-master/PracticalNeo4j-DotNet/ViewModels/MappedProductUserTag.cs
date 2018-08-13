using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PracticalNeo4j_DotNet.ViewModels
{
    public class MappedProductUserTag
    {
        public string title {get; set;}

        public List<string> users { get; set; }

        public List<string> tags { get; set; }
    }
}