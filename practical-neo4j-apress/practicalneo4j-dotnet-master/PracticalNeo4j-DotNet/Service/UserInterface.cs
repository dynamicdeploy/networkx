using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalNeo4j_DotNet.Models;
using PracticalNeo4j_DotNet.ViewModels;

namespace PracticalNeo4j_DotNet.Service
{
public interface UserInterface
{
    User getByUserName(string username);
    GraphStory login(GraphStory graphStory);
    GraphStory save(GraphStory graphStory);
    User update(User user);
    List<User> following(string username);
    MappedUserLocation getUserLocation(String currentusername);
    List<User> searchNotFollowing(String currentusername, String username);
    List<User> follow(String currentusername, String username);
    List<User> unfollow(String currentusername, String username);
}
}
