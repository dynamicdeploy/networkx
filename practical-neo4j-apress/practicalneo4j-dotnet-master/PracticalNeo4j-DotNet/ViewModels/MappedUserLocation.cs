using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PracticalNeo4j_DotNet.ViewModels
{
    public class MappedUserLocation
    {
        public string username { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }
    }
}