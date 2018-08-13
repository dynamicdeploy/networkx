using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neo4jClient;
using Neo4jClient.Cypher;
using PracticalNeo4j_DotNet.Models;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Service
{
    public class ProductService : ProductInterface
    {
        private readonly IGraphClient _graphClient;

        public ProductService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        // retrieve products
        public GraphStory getProducts(GraphStory graphStory, int pagenum)
        {
           graphStory.products= _graphClient.Cypher
                .Match("(p:Product)")
                .Return(() => Return.As<MappedProduct>("{nodeId: ID(p), title: p.title," +
                    "  description: p.description, tagstr: p.tagstr}"))
                .OrderBy("p.title")
                .Skip(pagenum)
                .Limit(11)
                .Results.ToList();

            if(graphStory.products.Count()>10){
                graphStory.next = true;
                graphStory.products = graphStory.products.GetRange(0, 10);
            }
            else
            {
                graphStory.next = false;
            }
           
            return graphStory;
        }

        public GraphStory getProduct(GraphStory graphStory)
        {
            graphStory.product = getProduct(graphStory.productNodeId);
            return graphStory;
        }

        private Product getProduct(long productNodeId)
        {

            Node<Product> np = _graphClient.Cypher
                .Match(" (p:Product) ")
                .Where("id(p)={id}")
                .WithParam("id", productNodeId)
                .Return(p => p.As<Node<Product>>())
                .Results.Single();

            Product product = np.Data;
            product.nodeId = np.Reference.Id;
            product.noderef = np.Reference;

            return product;
        }


        // capture view and return all views
        public List<MappedProductUserViews> createUserViewAndReturnViews(string username, long productNodeId)
        {
            DateTime datetime = DateTime.UtcNow;
            string timestampAsStr = datetime.ToString("MM/dd/yyyy") + " at " + 
                datetime.ToString("h:mm tt");
            long timestampAsLong = (long)(datetime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            List<MappedProductUserViews> mappedProductUserViews =  _graphClient.Cypher
                .Match(" (p:Product), (u:User { username:{u} }) ")
                .WithParam("u",username)
                .Where("id(p) = {productNodeId}")
                .WithParam("productNodeId",productNodeId)
                .With(" u,p")
                .Merge(" (u)-[r:VIEWED]->(p)")
                .Set(" r.dateAsStr={d}, r.timestamp={t} ")
                .WithParams(new { d = timestampAsStr, t = timestampAsLong })
                .With(" u ")
                .Match("  (u)-[r:VIEWED]->(p) ")
                .Return(() => Return.As<MappedProductUserViews>("{ title: p.title, "+
                    "dateAsStr: r.dateAsStr }"))
                .OrderByDescending("r.timestamp")
                .Results.ToList();
            return mappedProductUserViews;
        }

        // product trail
        public List<MappedProductUserViews> getProductTrail(string username) {

            List<MappedProductUserViews> mappedProductUserViews =  _graphClient.Cypher
                .Match("(u:User { username: {username} })-[r:VIEWED]->(p)")
                .WithParam("username", username)
                .Return(() => Return.As<MappedProductUserViews>("{ title: p.title, "+
                    "dateAsStr: r.dateAsStr }"))
                .OrderByDescending("r.timestamp")
                .Results.ToList();

            return mappedProductUserViews;
        }

        // consumption filter for marketing (matching tags) / tag is optional
        public List<MappedProductUserTag> usersWithMatchingTags(string tag) {
            List<MappedProductUserTag> mappedProductUserTags = null;
            if(!String.IsNullOrEmpty(tag)){
                mappedProductUserTags = _graphClient.Cypher
               .Match("(t:Tag { wordPhrase: {wp} })")
               .WithParam("wp", tag)
               .Match(" (p:Product)-[:HAS]->(t)<-[:USES]-(u:User) ")
               .Return(() => Return.As<MappedProductUserTag>("{ title: p.title , " +
                   "users: collect(u.username), tags: collect(distinct t.wordPhrase) }"))
               .Results.ToList(); 
            }else{
                mappedProductUserTags = _graphClient.Cypher
                .Match(" (p:Product)-[:HAS]->(t)<-[:USES]-(u:User) ")
                .Return(() => Return.As<MappedProductUserTag>("{ title: p.title ,  " +
                "users: collect(u.username), tags: collect(distinct t.wordPhrase) }"))
                .Results.ToList(); 
            }

            

            return mappedProductUserTags;
        }

        // search products via string
        public MappedProductSearch[] search(String q) {

            q = q.Trim().ToLower() + ".*";

            MappedProductSearch[] mappedProductSearch=_graphClient.Cypher
                .Match("(p:Product)")
                .Where("lower(p.title) =~ {q}")
                .WithParam("q", q)
                .Return(() => Return.As<MappedProductSearch>(" { name: count(*), " +
                " id: TOSTRING(ID(p)), label: p.title  }"))
                .OrderBy("p.title")
                .Limit(5)
                .Results.ToArray();

            return mappedProductSearch;
        }
    }
}