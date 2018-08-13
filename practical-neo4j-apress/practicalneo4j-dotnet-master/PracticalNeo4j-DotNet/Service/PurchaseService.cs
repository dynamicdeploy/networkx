using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Neo4jClient;
using Neo4jClient.Cypher;
using PracticalNeo4j_DotNet.Models;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Service
{
    public class PurchaseService : PurchaseInterface
    {
        private readonly IGraphClient _graphClient;

        public PurchaseService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        // purchases by friends
        public List<MappedProductUserPurchase> friendsPurchase(string userId)
        {
            return _graphClient.Cypher
                .Match("(u:User { userId : {userId} } )-[:FOLLOWS]-(f)-[:MADE]->()-[:CONTAINS]->p")
                .WithParam("userId", userId)
                .Return(() => Return.As<MappedProductUserPurchase>("{ productId:p.productId,title:p.title," +
                 "fullname:collect(f.firstname + ' ' + f.lastname),wordPhrase:null,cfriends: count(f) } "))
                .OrderByDescending("count(f)")

                .Results.ToList();
        }

        // userId is friends with a,b,c who made purchase of product title
        // AKA friends bought this product
        public List<MappedProductUserPurchase> friendsPurchaseByProduct(string userId, string title)
        {
            return _graphClient.Cypher
                 .Match("(p:Product)")
                 .Where("lower(p.title) =lower({title})")
                 .WithParam("title", title)
                 .With("p")
                 .Match("(u:User { userId : {userId} } )-[:FOLLOWS]-(f)-[:MADE]->()-[:CONTAINS]->(p) ")
                 .WithParam("userId", userId)
                 .Return(() => Return.As<MappedProductUserPurchase>("{productId:p.productId,title:p.title," +
                 "fullname:collect(f.firstname + ' ' + f.lastname),wordPhrase:null,cfriends: count(f) }"))
                 .OrderByDescending("count(f)")
                 .Results.ToList();
        }

        // x is friends with a,b,c who made purchase of various products and x uses tags which one or more products has in common
        // AKA friends bought products. match these products to tags of the current user
        public List<MappedProductUserPurchase> friendsPurchaseTagSimilarity(string userId)
        {
            return _graphClient.Cypher
                .Match("(u:User { userId : {userId} } )-[:FOLLOWS]-(f)-[:MADE]->()-[:CONTAINS]->p")
                .WithParam("userId", userId)
                .With("u,p,f")
                .Match("u-[:USES]->(t)<-[:HAS]-p")
                .Return(() => Return.As<MappedProductUserPurchase>("{productId:p.productId,title:p.title," +
                "fullname:collect(f.firstname + ' ' + f.lastname),wordPhrase:t.wordPhrase,cfriends: count(f)}"))
                .OrderByDescending("count(f)")
                .Results.ToList();
        }

        // x is friends with a,b,c who made purchase of product y. user x uses tags which product y has in common. x also lives within 10 miles of b
        // AKA friends that are nearby bought this product. the product also matches tags you use
        public List<MappedProductUserPurchase> friendsPurchaseTagSimilarityAndProximityToLocation(double lat, double lon, double distance, string userId)
        {
            var q = string.Format(distanceQueryAsString(lat, lon, distance));

            return  _graphClient.Cypher
                .Start(new { n = Node.ByIndexQuery("geom", q) })
                .With("n")
                .Match("(u:User { userId : {userId} } )-[:USES]->(t)<-[:HAS]-p")
                .WithParam("userId", userId)
                .With("n,u,p,t")
                .Match("u-[:FOLLOWS]->(f)-[:HAS]->(n) ")
                .With("p,f,t")
                .Match("f-[:MADE]->()-[:CONTAINS]->(p)")
                .Return(() => Return.As<MappedProductUserPurchase>("{productId:p.productId,title:p.title," +
                "fullname:collect(f.firstname + ' ' + f.lastname),wordPhrase:t.wordPhrase,cfriends: count(f)}"))
                .OrderByDescending("count(f)")
                .Results.ToList();


        }

        private String distanceQueryAsString(double lat, double lon, double distance)
        {
            StringBuilder lq = new StringBuilder();
            lq.Append("withinDistance:[");
            lq.Append(lat.ToString());
            lq.Append(",");
            lq.Append(lon.ToString());
            lq.Append(",");
            lq.Append(distance.ToString("#.00"));
            lq.Append("]");

            return lq.ToString();
        }
    }
}