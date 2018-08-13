using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalNeo4j_DotNet.ViewModels;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Service
{
    public interface LocationInterface
    {
        List<MappedLocation> returnLocationsWithinDistance(double lat, double lon, double distance, string locationType);

        // filter locations within distance that have product
        GraphStory returnLocationsWithinDistanceAndHasProduct(GraphStory graphStory, double lat, double lon, Double distance);

        // connect products to location
        void addProductToLocation(long locationNodeId, long productNodeId);

        // connect products to location
        void removeProductFromLocation(long locationNodeId, long productNodeId);

        // create/edit location
        Location save(Location location);
    }
}
