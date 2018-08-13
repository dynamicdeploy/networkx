using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PracticalNeo4j_DotNet.Service;

namespace PracticalNeo4j_DotNet.Service
{
    public class GraphStoryService : GraphStoryInterface
    {
        public ContentInterface contentInterface { get; set; }
        public LocationInterface locationInterface { get; set; }
        public ProductInterface productInterface { get; set; }
        public PurchaseInterface purchaseInterface { get; set; }
        public TagInterface tagInterface { get; set; }
        public UserInterface userInterface { get; set; }

        public GraphStoryService(ContentInterface contentInterface, LocationInterface locationInterface, ProductInterface productInterface, PurchaseInterface purchaseInterface, TagInterface tagInterface, UserInterface userInterface)
        {
            this.contentInterface = contentInterface;
            this.locationInterface = locationInterface;
            this.productInterface = productInterface;
            this.purchaseInterface = purchaseInterface;
            this.tagInterface = tagInterface;
            this.userInterface = userInterface;
        }

        
    }
}