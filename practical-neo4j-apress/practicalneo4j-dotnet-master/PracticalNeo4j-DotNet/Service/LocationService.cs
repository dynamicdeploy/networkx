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
    public class LocationService : LocationInterface
    {
        private readonly IGraphClient _graphClient;
        
        public LocationService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public List<MappedLocation> returnLocationsWithinDistance(double lat, double lon, double distance, string locationType)
        {
            var q = string.Format(distanceQueryAsString(lat, lon, distance));
            
            List<MappedLocation> mappedLocations = _graphClient.Cypher
                .Start(new { n = Node.ByIndexQuery("geom", q) })
                .Where(" n.type = {locationType} ")
                .WithParam("locationType", locationType)
                .Return(() => Return.As<MappedLocation>("{locationId: n.locationId, address: n.address , " +
                        " city: n.city , state: n.state, zip: n.zip , name: n.name, lat: n.lat , lon: n.lon}"))
                    .Results.ToList();
            addDistanceTo(mappedLocations, lat, lon);

            return mappedLocations;
        }
        

        // filter locations within distance that have product
        public GraphStory returnLocationsWithinDistanceAndHasProduct(GraphStory graphStory, double lat, double lon, double distance)
        {
            string q = distanceQueryAsString(lat, lon, distance);
            List<MappedLocation> mappedLocations= _graphClient.Cypher
                .Start(new { n = Node.ByIndexQuery("geom", q), p = graphStory.product.noderef})
                .Match(" n-[:HAS]->p ")
                .Return(() => Return.As<MappedLocation>(" { locationId: n.locationId, address: n.address , " +
                     " city: n.city , state: n.state, zip: n.zip , name: n.name, lat: n.lat , lon: n.lon} "))
                 .Results.ToList();
            
            addDistanceTo(mappedLocations, lat, lon);
            graphStory.mappedLocations = mappedLocations;

            return graphStory;
        }

        private void addDistanceTo(List<MappedLocation> locations, Double lat, Double lon) {

		    foreach (MappedLocation location in locations) {
			    location.distanceToLocation = distance(location.lat, location.lon, lat, lon, 'M');
		    }

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

        private String distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            StringBuilder distance = new StringBuilder();

            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);

            char K = 'K';
            char N = 'N';
            char M = 'M';

            if (unit == M)
            {
                dist = dist * 60 * 1.1515;
                
                distance.Append(dist.ToString("#.00") + " Miles Away");
            }
            else if (unit == K)
            {
                dist = dist * 1.609344;
                distance.Append(dist + "Kilometers Away");
            }
            else if (unit == N)
            {
                dist = dist * 0.8684;
                distance.Append(dist + "Nautical Miles Away");
            }
            return distance.ToString();
        }

        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        /*::  This function converts decimal degrees to radians             :*/
        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        /*::  This function converts radians to decimal degrees             :*/
        /*:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::*/
        private double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }

        // connect products to location
        public void addProductToLocation(long locationNodeId, long productNodeId)
        {

        }

        // connect products to location
        public void removeProductFromLocation(long locationNodeId, long productNodeId)
        {

        }

        // create/edit location
        public Location save(Location location)
        {
            return null;
        }
    }
}