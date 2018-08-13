using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;

namespace PracticalNeo4j_DotNet.Models
{
    public class User
    {
        public long nodeId { get; set; }
        public NodeReference noderef { get; set; }
        public string userId { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}