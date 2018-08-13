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
    public class TagService : TagInterface
    {
        private readonly IGraphClient _graphClient;

        public TagService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public List<Tag> saveTags(string tagText)
        {
            List<Tag> tags = new List<Tag>();
            
            tagText = tagText.Trim().ToLower();
            string[] tagArray = tagText.Split(',');
            Tag t;

            foreach (string tagval in tagArray)
            {
                string tval = tagval.Trim();
                if(!String.IsNullOrEmpty(tval)){
                    t = save(tval);
                    tags.Add(t);
                }

            }

            return tags;
        }

        // save a tag
        public Tag save(string wp)
        {
            wp = wp.Trim();

            Tag t = find(wp);

            if(t == null){
                Node<Tag> nodetag = _graphClient.Cypher
                    .Create("(tag:Tag {tag})")
                    .WithParam("tag", newTag(wp))
                    .Return(tag => tag.As<Node<Tag>>())
                    .Results.Single();

                t = nodetag.Data;
                t.noderef = nodetag.Reference;
            }

            return t;
          
        }

        private Tag find(string wp)
        {
           wp = wp.Trim();
           Tag t = null;

           Node<Tag> nodetag= _graphClient.Cypher
                .Match(" (tag:Tag {wordPhrase: {t}}) ")
                .WithParam("t", wp)
                .Return(tag => tag.As<Node<Tag>>())
                .Results.Single();

           if (nodetag!=null)
           {
               t = nodetag.Data;
               t.noderef = nodetag.Reference;
           }

            return t;
           
        }

        private Tag newTag(string wp)
        {
            return new Tag { wordPhrase = wp };
        }

        public MappedContentTag[] search(string q)
        {
            q = q.Trim().ToLower() + ".*";

            MappedContentTag[] mappedContentTag = _graphClient.Cypher.Match(" (c:Content)-[:HAS]->(t:Tag) ")
                    .Where("lower(t.wordPhrase) =~ {q} ")
                    .WithParam("q",q)
                    .Return(() => Return.As<MappedContentTag>("{name: t.wordPhrase," +
                    "  label: t.wordPhrase,  id:  count(*) }"))
                    .OrderBy("t.wordPhrase")
                    .Limit(5)
                    .Results.ToArray();
            

            return mappedContentTag;
            
        }

        // count tags in aggregate
        public GraphStory tagsInMyNetwork(GraphStory graphStory)
        {
            graphStory.tagsInNetwork = _graphClient.Cypher
                .Start(new { u = graphStory.user.noderef })
                .Match("u-[:FOLLOWS]->f")
                .With("distinct f")
                .Match(" f-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-c")
                .With("distinct c")
                .Match(" c-[ct:HAS]->(t)")
                .With("distinct ct, t")
                .Return(() => Return.As<MappedContentTag>("{name: t.wordPhrase, label: t.wordPhrase, " + 
                    " id: count(*) }"))
                .OrderByDescending("count(*) ")
                .Results.ToList();

            graphStory.userTags = _graphClient.Cypher
                .Start(new { u = graphStory.user.noderef })
                .Match("u-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-c")
			    .With("distinct c")
                .Match(" c-[ct:HAS]->(t)")
                .With("distinct ct, t")
                .Return(() => Return.As<MappedContentTag>(" {name: t.wordPhrase, label: t.wordPhrase, " + 
                    " id: count(*) }" ))
                .OrderByDescending("count(*) ")
                .Results.ToList();


            return graphStory;
        }
    }
}
