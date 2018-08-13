using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalNeo4j_DotNet.ViewModels;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Service
{
    public interface PurchaseInterface
    {
        List<MappedProductUserPurchase> friendsPurchase(string userId);

        List<MappedProductUserPurchase> friendsPurchaseByProduct(string userId, string title);

        List<MappedProductUserPurchase> friendsPurchaseTagSimilarity(string userId);

        List<MappedProductUserPurchase> friendsPurchaseTagSimilarityAndProximityToLocation(double lat, double lon, double distance, string userId);
    }
}
