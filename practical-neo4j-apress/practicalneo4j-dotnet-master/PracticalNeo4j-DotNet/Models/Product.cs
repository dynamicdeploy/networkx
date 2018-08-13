using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;

namespace PracticalNeo4j_DotNet.Models
{
    public class Product
    {
        public long nodeId { get; set; }
        public NodeReference noderef { get; set; }
        public string productId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string tagstr { get; set; }
        public string content { get; set; }
        public string price { get; set; }
    }
}