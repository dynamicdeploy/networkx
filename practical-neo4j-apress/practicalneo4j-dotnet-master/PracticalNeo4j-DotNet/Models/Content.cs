using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;

namespace PracticalNeo4j_DotNet.Models
{
    public class Content
    {
        public long nodeId { get; set; }
        public NodeReference noderef { get; set; }
        public string contentId { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string tagstr { get; set; }
        public long timestamp { get; set; }
        public string userNameForPost { get; set; }
        private string TimestampAsStr;
        public string timestampAsStr
        {
            get
            {
                System.DateTime datetime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                datetime = datetime.AddSeconds(this.timestamp).ToLocalTime();

                this.timestampAsStr = datetime.ToString("MM/dd/yyyy") + " at " + datetime.ToString("h:mm tt");
                return TimestampAsStr;
            }
            set
            {
                TimestampAsStr = value;
            }
        }
        public List<Tag> tags { get; set; }
        public User user { get; set; }
        public Content next { get; set; }

    }
}