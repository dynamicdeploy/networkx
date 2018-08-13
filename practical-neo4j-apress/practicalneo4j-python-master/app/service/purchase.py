from py2neo import neo4j, ogm, node, rel


class Purchase():
    # products purchased by friends
    def friends_purchase(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db,
                  " MATCH (u:User {username: {u} } )-[:FOLLOWS]-(f)-[:MADE]->()-[:CONTAINS]->p" +
                  " RETURN p.productId as productId,  " +
                  " p.title as title, " +
                  " collect(f.firstname + ' ' + f.lastname) as fullname, " +
                  " null as wordPhrase, count(f) as cfriends " +
                  " ORDER BY cfriends desc, p.title ")
        params = {"u": username}
        result = query.execute(**params)
        return result

    # a specific product purchased by friends
    def friends_purchase_by_product(self, graph_db, username, title):
        query = neo4j.CypherQuery(graph_db,
                  " MATCH (p:Product) " +
                  " WHERE lower(p.title) =lower({title}) " +
                  " WITH p " +
                  " MATCH (u:User {username: {u} } )-[:FOLLOWS]-(f)-[:MADE]->()-[:CONTAINS]->(p) " +
                  " RETURN p.productId as productId,  " +
                  " p.title as title, " +
                  " collect(f.firstname + ' ' + f.lastname) as fullname, " +
                  " null as wordPhrase, count(f) as cfriends " +
                  " ORDER BY cfriends desc, p.title ")
        params = {"u": username, "title": title}
        result = query.execute(**params)
        return result

    # products purchased by friends that match the user's tags
    def friends_purchase_tag_similarity(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db,
                  " MATCH (u:User {username: {u} } )-[:FOLLOWS]-(f)-[:MADE]->()-[:CONTAINS]->p " +
                  " WITH u,p,f " +
                  " MATCH u-[:USES]->(t)<-[:HAS]-p " +
                  " RETURN p.productId as productId,  " +
                  " p.title as title, " +
                  " collect(f.firstname + ' ' + f.lastname) as fullname, " +
                  " t.wordPhrase as wordPhrase, " +
                  " count(f) as cfriends " +
                  " ORDER BY cfriends desc, p.title ")
        params = {"u": username}
        result = query.execute(**params)
        return result

    # user's friends' purchases who are nearby and the products match the user's tags
    def friends_purchase_tag_similarity_and_proximity_to_location(self, graph_db, username, lq):
        query = neo4j.CypherQuery(graph_db,
                  " START n = node:geom({lq}) " +
                  " WITH n " +
                  " MATCH (u:User {username: {u} } )-[:USES]->(t)<-[:HAS]-p " +
                  " WITH n,u,p,t " +
                  " MATCH u-[:FOLLOWS]->(f)-[:HAS]->(n) " +
                  " WITH p,f,t " +
                  " MATCH f-[:MADE]->()-[:CONTAINS]->(p) " +
                  " RETURN p.productId as productId, " +
                  " p.title as title, " +
                  " collect(f.firstname + ' ' + f.lastname) as fullname, " +
                  " t.wordPhrase as wordPhrase, " +
                  " count(f) as cfriends " +
                  " ORDER BY cfriends desc, p.title ")
        params = {"u": username, "lq": lq}
        result = query.execute(**params)
        return result