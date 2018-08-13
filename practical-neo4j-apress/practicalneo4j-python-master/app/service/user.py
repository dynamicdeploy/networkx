from py2neo import neo4j, ogm, node, rel


class User():
    def get_user_by_username(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (user:User {username: {username}}) " +
                                  " RETURN user ")
        params = {"username": username}
        result = query.execute_one(**params)
        return result

    def save_user(self, graph_db, username):
        # create user
        newuser, = graph_db.create({"username": username})
        # add the label
        newuser.add_labels("User")
        return newuser

    def update_user(self, graph_db, username, firstname, lastname):
        query = neo4j.CypherQuery(graph_db,
                                  "MATCH (user:User {username:{u}} )  " +
                                  "SET user.firstname = {fn}, user.lastname = {ln}")
        params = {"u": username, "fn": firstname, "ln": lastname}
        result = query.execute(**params)
        return result
    
    # the following method in the User class
    def following(self, graph_db, username):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (user { username:{u}})-[:FOLLOWS]->(users) " +
                                  " RETURN users.firstname as firstname, users.lastname as lastname,"+
                                  " users.username as username " +
                                  " ORDER BY users.username")
        params = {"u": username}
        result = query.execute(**params)
        return result

    # search by user returns users in the network that aren't already being followed
    def search_by_username_not_following(self, graph_db, currentusername, username):
        username = username.lower() + ".*"
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (n:User), (user { username:{cu}}) " +
                                  " WHERE (n.username =~ {u} AND n <> user) " +
                                  " AND (NOT (user)-[:FOLLOWS]->(n)) " +
                                  " RETURN n.firstname as firstname, n.lastname as lastname,"+
                                  " n.username as username")
        params = {"u": username, "cu": currentusername}
        result = query.execute(**params)
        return result

    # follow a user
    def follow(self, graph_db, currentusername, username):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (user1:User {username:{cu}} ), (user2:User {username:{u}} ) " +
                                  " CREATE UNIQUE user1-[:FOLLOWS]->user2 " +
                                  " WITH user1" +
                                  " MATCH (user1)-[f:FOLLOWS]->(users)" +
                                  " RETURN users.firstname as firstname, users.lastname as lastname, " +
                                  " users.username as username " +
                                  " ORDER BY users.username")
        params = {"cu": currentusername,"u": username}
        result = query.execute(**params)
        return result

    # unfollow a user
    def unfollow(self, graph_db, currentusername, username):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (user1:User {username:{cu}} )-[f:FOLLOWS]->(user2:User {username:{u}} ) " +
                                  " DELETE f " +
                                  " WITH user1" +
                                  " MATCH (user1)-[f:FOLLOWS]->(users)" +
                                  " RETURN users.firstname as firstname, users.lastname as lastname, "+
                                  " users.username as username " +
                                  " ORDER BY users.username")
        params = {"cu": currentusername, "u": username}
        result = query.execute(**params)
        return result

    def users_results_as_array(self, users):
        usersArray = []
        # build array
        for u in users:
            usersArray.append({"firstname": u.firstname, "lastname": u.lastname, "username": u.username})
        # return json       
        return usersArray
    
