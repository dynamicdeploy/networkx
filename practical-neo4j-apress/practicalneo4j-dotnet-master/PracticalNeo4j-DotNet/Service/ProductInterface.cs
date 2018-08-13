using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalNeo4j_DotNet.ViewModels;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Service
{
    public interface ProductInterface
    {
        // retrieve products
        GraphStory getProducts(GraphStory graphStory, int pagenum);

        GraphStory getProduct(GraphStory graphStory);

        // capture view and return them all based on user
        List<MappedProductUserViews> createUserViewAndReturnViews(string username, long productNodeId);

        // product trail
        List<MappedProductUserViews> getProductTrail(string username);

        // consumption filter for marketing (matching tags) / tag is optional
        List<MappedProductUserTag> usersWithMatchingTags(string tag);

        // search products via string
        MappedProductSearch[] search(String q);

    }
}
