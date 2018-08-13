from py2neo import neo4j, node, rel


class UserLocation():
    def get_user_location(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db, " MATCH (u:User { username : {u} } )-[:HAS]-(l:Location) " +
                                  " RETURN u.username as username, l.address as address, l.city as city, l.state as state, " +
                                  " l.zip as zip, l.lat as lat, l.lon as lon")
        params = {"u": username}
        result = query.execute(**params)
        return result

    def get_lq_distance_set(self, ul):
        # setup spatial query for spatial index
        lat = "{:.9f}".format(ul["lat"])
        lon = "{:.9f}".format(ul["lon"])
        cs = ","
        lq = ''.join(["withinDistance:[", lat, cs, lon, cs, "10.00", "]"])
        return lq

    def get_lq(self, ul, distance):
        # setup spatial query for spatial index
        lat = "{:.9f}".format(ul["lat"])
        lon = "{:.9f}".format(ul["lon"])
        cs = ","
        lq = ''.join(["withinDistance:[", lat, cs, lon, cs, distance, "]"])
        return lq
