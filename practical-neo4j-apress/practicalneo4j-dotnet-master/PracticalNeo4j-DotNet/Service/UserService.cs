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
    public class UserService : UserInterface
    {
        private readonly IGraphClient _graphClient;

        private User tempuser;

        public UserService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public GraphStory save(GraphStory graphStory)
        {
            graphStory.user.username = graphStory.user.username.ToLower();
            
            if (userExists(graphStory.user.username)==false)
            {
                graphStory.user.userId = Guid.NewGuid().ToString();

              User u=   _graphClient.Cypher
               .Create(" (user:User {user}) ")
               .WithParam("user", graphStory.user)
               .Return(user => user.As<User>())
               .Results.Single();

              graphStory.user = u;
            }
            else
            {
                graphStory.haserror = true;
                graphStory.error = "The username you entered already exists.";
            }
            
            return graphStory;
        }

        public GraphStory login(GraphStory graphStory)
        {
            tempuser = getByUserName(graphStory.user.username.ToLower());
            if (tempuser!=null)
            {
               
                graphStory.user = tempuser;
            }
            else{
                graphStory.haserror = true;
                graphStory.error = "The username you entered does not exist.";
            }

            return graphStory;
       }

        public User update(User user)
        {
            _graphClient.Cypher
                .Match(" (user:User {username:{user}} ) ")
                .WithParam("user", user.username.ToLower())
                .Set("user.firstname = {fn}, user.lastname = {ln} ")
                .WithParams(new {fn=user.firstname,ln=user.lastname })
                .ExecuteWithoutResults();

            return user;
        }

        private bool userExists(string username)
        {
            bool userFound = false;

            if (getByUserName(username) != null)
            {
                userFound = true;
            }

            return userFound;
        }

        public User getByUserName(string username)
        {
            User u = null;
            try
            {
                Node<User> n = _graphClient.Cypher
                .Match(" (user:User {username:{user}} ) ")
                .WithParam("user", username.ToLower())
                .Return(user => user.As<Node<User>>())
                .Results.Single();

                // set user
               u = n.Data;
                // set node id
                u.noderef = n.Reference;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            

            return u;
        }

        public List<User> following(string username)
        {
            List<User> following = _graphClient.Cypher
                .Match(" (user { username:{u}})-[:FOLLOWS]->(users)  ")
                .WithParam("u", username.ToLower())
                .Return(users => users.As<User>())
                .OrderBy("users.username")
                .Results.ToList<User>();

            return following;
        }

        public List<User> searchNotFollowing(String currentusername, String username)
        {

            username = username.ToLower() + ".*";

            List<User> following = _graphClient.Cypher
                .Match(" (n:User), (user { username:{c}})   ")
                .WithParam("c", currentusername.ToLower())
                .Where(" (n.username =~ {u} AND n <> user) ")
                .AndWhere("(NOT (user)-[:FOLLOWS]->(n)) ")
                .WithParam("u", username)
                .Return(n => n.As<User>())
                .OrderBy("n.username")
                .Results.ToList<User>();
            
            return following;
        }

        public MappedUserLocation getUserLocation(string currentusername)
        {
            MappedUserLocation mappedUserLocation = _graphClient.Cypher
               .Match(" (u:User { username : {u} } )-[:HAS]-(l:Location) ")
               .WithParam("u", currentusername)
               .Return(() => Return.As<MappedUserLocation>("{ username: u.username, address: l.address," +
                   " city:l.city, state: l.state, zip: l.zip, lat: l.lat, lon: l.lon} "))
               .Results.First();

            return mappedUserLocation;

        }

        // follows a user and also returns the list of users being followed
        public List<User> follow(String currentusername, String username)
        {
            List<User> following = _graphClient.Cypher
                .Match(" (user1:User {username:{cu}} ), (user2:User {username:{u}} ) ")
                .WithParams(new { cu = currentusername.ToLower(), u = username.ToLower() })
                .CreateUnique("user1-[:FOLLOWS]->user2")
                .With(" user1 ")
                .Match("  (user1)-[f:FOLLOWS]->(users) ")
                .Return(users => users.As<User>())
                .OrderBy("users.username")
                .Results.ToList<User>();

            return following;
        }

        // unfollows a user and also returns the list of users being followed
        public List<User> unfollow(String currentusername, String username)
        {
                List<User> following = _graphClient.Cypher
                .Match(" (user1:User {username:{cu}} )-[f:FOLLOWS]->(user2:User {username:{u}} ) ")
                .WithParams( new {cu = currentusername.ToLower(), u = username.ToLower()} )
                .Delete("f")
                .With(" user1 ")
                .Match("  (user1)-[f:FOLLOWS]->(users) ")
                .Return(users => users.As<User>())
                .OrderBy("users.username")
                .Results.ToList<User>();
                return following;
        }
        
    }
}