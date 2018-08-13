from py2neo import neo4j, ogm, node, rel
import math


class Location():
    def locations_within_distance(self, graph_db, lq, mappedUserLocation,locationType):
        query = neo4j.CypherQuery(graph_db, " START n = node:geom({lq}) " +
                                  " WHERE n.type = {locationType} " +
                                  " RETURN n.locationId as locationId, n.address as address," +
                                  " n.city as city,  n.state as state, n.zip as zip, n.name as name, " +
                                  " n.lat as lat, n.lon as lon")
        params = {"lq": lq, "locationType":locationType}
        result = query.execute(**params)
    
        for r in result:
            # add the distance in miles
            setattr(r, "distanceToLocation", self.distance(float(r.lon), float(r.lat),
                                                           float(mappedUserLocation["lon"]),
                                                           float(mappedUserLocation["lat"])))
        return result

    def locations_within_distance_with_product(self, graph_db, lq, productNodeId, mappedUserLocation):
        query = neo4j.CypherQuery(graph_db, 
                      " START n = node:geom({lq}), " +
                      " p=node({productNodeId}) " +
                      " MATCH n-[:HAS]->p " +
                      " RETURN n.locationId as locationId, n.address as address, " +
                      " n.city as city,  n.state as state, n.zip as zip, n.name as name, " +
                      " n.lat as lat, n.lon as lon")
        params = {"lq": lq, "productNodeId": productNodeId}
        result = query.execute(**params)

        for r in result:
            # add the distance in miles
            setattr(r, "distanceToLocation", self.distance(float(r.lon), float(r.lat),
                                                           float(mappedUserLocation["lon"]),
                                                           float(mappedUserLocation["lat"])))
        return result

    def distance(self, lon1, lat1, lon2, lat2):
        radius = 3959  # miles

        dlat = math.radians(lat2 - lat1)
        dlon = math.radians(lon2 - lon1)
        a = math.sin(dlat / 2) * math.sin(dlat / 2) + math.cos(math.radians(lat1)) \
                                                      * math.cos(math.radians(lat2)) * math.sin(dlon / 2) * math.sin(
            dlon / 2)
        c = 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))
        d = radius * c

        return str(round(d, 2)) + " miles away"
