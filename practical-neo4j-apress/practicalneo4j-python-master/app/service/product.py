from py2neo import neo4j, ogm, node, rel
import time
from datetime import datetime


class Product():
    def get_products(self, graph_db, skip):
        # get collection of products limit at 10
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (p:Product) RETURN ID(p) as nodeId, p.title as title, " +
                                  " p.description as description, p.tagstr  as tagstr " +
                                  " ORDER BY p.title " +
                                  " SKIP {skip} LIMIT 10 ")
        params = {"skip": skip}
        result = query.execute(**params)
        return result

    def get_product_trail(self, graph_db, username):
        # get the product trail details from the relationships
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (u:User { username: {username} })-[r:VIEWED]->(p) " +
                                  " RETURN p.title as title,  r.dateAsStr as dateAsStr " +
                                  " ORDER BY r.timestamp desc ")
        params = {"username": username}
        result = query.execute(**params)
        return result

    # the method to add a user view of a product and return all views
    def create_user_view_and_return_views(self, graph_db, username, productNodeId):
        
        # create timestamp and string display
        ts = time.time()
        timestampAsStr = datetime.fromtimestamp(int(ts)).strftime(
            '%m/%d/%Y') + " at " + datetime.fromtimestamp(int(ts)).strftime('%I:%M %p')
        
        query = neo4j.CypherQuery(graph_db,
                " MATCH (p:Product), (u:User { username:{u} })" +
                " WHERE id(p) = {productNodeId}" +
                " WITH u,p" +
                " MERGE (u)-[r:VIEWED]->(p)" +
                " SET r.dateAsStr={timestampAsStr}, r.timestamp={ts}" +
                " WITH u " +
                " MATCH (u)-[r:VIEWED]->(p)" +
                " RETURN p.title as title,  r.dateAsStr as dateAsStr" +
                " ORDER BY r.timestamp desc")
        params = {"productNodeId": productNodeId,"u": username,
                  "timestampAsStr": timestampAsStr,"ts": ts }
        result = query.execute(**params)
        
        result=self.get_product_trail_results_as_json(result)
        
        return result

    def get_product_trail_results_as_json(self,productTrail):

        # put the product trail results in an array to return as json
        ptArray = []
        for pt in productTrail:
            ptArray.append({"title": pt.title, "dateAsStr": pt.dateAsStr})

        return {"productTrail": ptArray}

    def get_products_has_a_tag_and_user_uses_a_matching_tag(self, graph_db):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (p:Product)-[:HAS]->(t)<-[:USES]-(u:User) " +
                                  " RETURN p.title as title , collect(u.username) as users, " +
                                  " collect(distinct t.wordPhrase) as tags ")
        result = query.execute()
        return result

    def get_products_has_specific_tag_and_user_uses_specific_tag(self, graph_db, wp):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (t:Tag { wordPhrase: {wp} }) " +
                                  " WITH t " +
                                  " MATCH (p:Product)-[:HAS]->(t)<-[:USES]-(u:User) " +
                                  " RETURN p.title as title,collect(u) as u, collect(distinct t) as t ")
        params = {"wp": wp}
        result = query.execute(**params)
        return result

    def product_search(self, graph_db, q):
        query = neo4j.CypherQuery(graph_db, "MATCH (p:Product) " +
                                  " WHERE lower(p.title) =~ {q} " +
                                  " RETURN TOSTRING(ID(p)) as id, count(*) as name, " +
                                  " p.title as label  " +
                                  " ORDER BY p.title LIMIT 5")
        params = {"q": q}
        result = query.execute(**params)
        return result

    def product_results_as_list(self, productsFound):
        products = []
        for r in productsFound:
            products.append({"id": r.id, "title": r.name, "label": r.label})
        return products
