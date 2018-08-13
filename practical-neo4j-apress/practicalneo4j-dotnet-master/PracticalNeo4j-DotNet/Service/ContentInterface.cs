using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalNeo4j_DotNet.ViewModels;
using PracticalNeo4j_DotNet.Models;
namespace PracticalNeo4j_DotNet.Service
{
    public interface ContentInterface
    {
        GraphStory getContent(GraphStory graphStory, string username, int page, int pagesize);
        List<MappedContent> getContentByTag(string username, string tag, bool isCurrentUser);
        MappedContent getContentItem(string contentId, string username);
        MappedContent add(Content content, string username);
        MappedContent edit(Content content, string username);
        void delete(string contentId, string username);
    }
}
