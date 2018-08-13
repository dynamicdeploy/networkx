using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PracticalNeo4j_DotNet.ViewModels
{
    public class MappedContent
    {
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
       
        public bool owner { get; set; }
    }
}