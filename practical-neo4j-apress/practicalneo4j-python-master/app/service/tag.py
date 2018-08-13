from py2neo import neo4j, ogm, node, rel


class Tag():
    def user_tags(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (u:User {username: {u} })-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-c " +
                                  " WITH distinct c " +
                                  " MATCH c-[ct:HAS]->(t) " +
                                  " WITH distinct ct,t " +
                                  " RETURN t.wordPhrase as name, t.wordPhrase as label, count(ct) as id " +
                                  " ORDER BY id desc " +
                                  " SKIP 0 LIMIT 30")
        params = {"u": username}
        result = query.execute(**params)
        return result

    def tags_in_network(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (u:User {username: {u} })-[:FOLLOWS]->f " +
                                  " WITH distinct f " +
                                  " MATCH f-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-c " +
                                  " WITH distinct c " +
                                  " MATCH c-[ct:HAS]->(t) " +
                                  " WITH distinct ct,t " +
                                  " RETURN t.wordPhrase as name, t.wordPhrase as label, count(ct) as id " +
                                  " ORDER BY id desc " +
                                  " SKIP 0 LIMIT 30")
        params = {"u": username}
        result = query.execute(**params)
        return result
    
    def search_tags(self,graph_db,q):
        q= q + ".*"
        query = neo4j.CypherQuery(graph_db,
                " MATCH (c:Content)-[:HAS]->(t:Tag) WHERE t.wordPhrase =~ {q} " + 
                " RETURN count(t) as name, TOSTRING(ID(t)) as id, t.wordPhrase as label  "+
                " ORDER BY t.wordPhrase " +
                " LIMIT 5")
        params = {"q": q}
        result = query.execute(**params)
        return result