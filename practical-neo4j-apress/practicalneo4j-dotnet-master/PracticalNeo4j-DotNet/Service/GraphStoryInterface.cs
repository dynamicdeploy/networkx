using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalNeo4j_DotNet.Service
{
    public interface GraphStoryInterface
    {
        ContentInterface contentInterface { get; set; }
        LocationInterface locationInterface { get; set; }
        ProductInterface productInterface { get; set; }
        PurchaseInterface purchaseInterface { get; set; }
        TagInterface tagInterface { get; set; }
        UserInterface userInterface { get; set; }
    }
}
