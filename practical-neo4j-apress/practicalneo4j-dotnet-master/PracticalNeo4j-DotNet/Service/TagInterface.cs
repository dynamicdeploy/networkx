using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalNeo4j_DotNet.ViewModels;
using PracticalNeo4j_DotNet.Models;

namespace PracticalNeo4j_DotNet.Service
{
    public interface TagInterface
    {
        // save tags from content
        List<Tag> saveTags(string tagText);

        // save a tag
        Tag save(string wp);

        MappedContentTag[] search(string q);

        // count tags in aggregate
        GraphStory tagsInMyNetwork(GraphStory graphStory);
    }
}
