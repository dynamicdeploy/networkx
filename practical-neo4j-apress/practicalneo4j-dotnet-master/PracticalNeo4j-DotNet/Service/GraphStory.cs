using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PracticalNeo4j_DotNet.Models;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Service
{
    public class GraphStory
    {
        public User user {get;set;}

        public Boolean haserror { get; set; }
        
        public string error { get; set; }

        public Product product { get; set; }

        public long productNodeId { get; set; }

        public List<MappedProduct> products { get; set; }

        public List<MappedContent> content { get; set; }

        public List<MappedContentTag> tagsInNetwork;

        public List<MappedContentTag> userTags;

        public List<MappedLocation> mappedLocations { get; set; }

        public List<MappedProductUserPurchase> mappedProductUserPurchases { get; set; }

        public MappedUserLocation mappedUserLocation { get; set; }

        public List<MappedProductUserViews> productTrail { get; set; }

        public List<MappedProductUserTag> usersWithMatchingTags { get; set; }

        public bool showform { get; set; }

        public bool next { get; set; }

        public string nextPageUrl { get; set; }

        public string producttitle { get; set; }
    }
}