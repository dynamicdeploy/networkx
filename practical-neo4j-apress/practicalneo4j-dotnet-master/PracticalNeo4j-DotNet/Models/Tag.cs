using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;
namespace PracticalNeo4j_DotNet.Models
{
    public class Tag
    {
        public long nodeId { get; set; }
        public NodeReference noderef { get; set; }
        public string wordPhrase { get; set; }
    }
}