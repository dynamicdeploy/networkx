using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PracticalNeo4j_DotNet.ViewModels
{
    public class MappedProductUserPurchase
    {
        public string productId { get; set; }

        public string title { get; set; }

        public List<string> fullname { get; set; }

        public string wordPhrase { get; set; }

        public int cfriends { get; set; }
    }
}