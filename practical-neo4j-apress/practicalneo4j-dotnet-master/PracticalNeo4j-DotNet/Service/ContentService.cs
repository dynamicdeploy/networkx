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
    public class ContentService : ContentInterface
    {
        private readonly IGraphClient _graphClient;

        public ContentService(IGraphClient graphClient)
        {
            _graphClient = graphClient;
        }
        public GraphStory getContent(GraphStory graphStory, string username, int page, int pagesize)
        {

            graphStory.content = _graphClient.Cypher.Match(" (u:User {username: {u} }) ")
                .WithParam("u", username)
                .With("u")
                .Match(" (u)-[:FOLLOWS*0..1]->f  ")
                .With(" DISTINCT f,u ")
                .Match(" f-[:CURRENTPOST]-lp-[:NEXTPOST*0..3]-p")
                .Return(() => Return.As<MappedContent>("{contentId: p.contentId, title: p.title, url: p.url," +
                    " tagstr: p.tagstr, timestamp: p.timestamp, userNameForPost: f.username, owner: f=u}"))
                .OrderByDescending("p.timestamp")
                .Skip(page)
                .Limit(pagesize)
                .Results.ToList();
        
            return graphStory;
        }

        public List<MappedContent> getContentByTag(string username, string tag, bool isCurrentUser)
        {
            List<MappedContent> mappedContent = null;
            
            if (isCurrentUser == true)
            {
                mappedContent = _graphClient.Cypher.Match(" (u:User {username: {u} }) ")
                .WithParam("u", username)
                .With("u")
                .Match(" u-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-p ")
                .With(" DISTINCT u,p ")
                .Match(" p-[:HAS]-(t:Tag {wordPhrase : {wp} } )")
                .WithParam("wp",tag)
                .Return(() => Return.As<MappedContent>(" { contentId: p.contentId, title: p.title, url: p.url," +
                    " tagstr: p.tagstr, timestamp: p.timestamp, userNameForPost: u.username, owner: true } "))
                .OrderByDescending("p.timestamp")
                .Results.ToList();
            }
            else
            {
                mappedContent = _graphClient.Cypher.Match(" (u:User {username: {u} }) ")
                .WithParam("u", username)
                .With("u")
                .Match(" (u)-[:FOLLOWS]->f ")
                .With(" DISTINCT f ")
                .Match(" f-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-p ")
                .With(" DISTINCT f,p ")
                .Match(" p-[:HAS]-(t:Tag {wordPhrase : {wp} } )")
                .WithParam("wp", tag)
                .Return(() => Return.As<MappedContent>(" { contentId: p.contentId, title: p.title, url: p.url, " +
                    "tagstr: p.tagstr, timestamp: p.timestamp, userNameForPost: f.username, owner: false } "))
                .OrderByDescending("p.timestamp")
                .Results.ToList();

            }

            return mappedContent;
        }
        public MappedContent getContentItem(string contentId, string username)
        {
            MappedContent mappedContentItem = _graphClient.Cypher
                .Match(" (c:Content {contentId:{contentId}})-[:NEXTPOST*0..]-()-[:CURRENTPOST]-(o:User), " +
                " (u:User {username: {u} })  ")
                .WithParams(new { contentId = contentId, u = username })
                .Return(() => Return.As<MappedContent>(" { contentId: c.contentId, title: c.title, url: c.url," +
                " tagstr: c.tagstr, timestamp: c.timestamp, userNameForPost: o.username, owner: o=u } "))
                .Results.Single();

            return mappedContentItem;
        }
        public MappedContent add(Content content, string username)
        {
            content.contentId = Guid.NewGuid().ToString();
            content.timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            content.tagstr = removeTrailingComma(content.tagstr);

            // splits up the comma separated string into arrays and removes any empties. 
            // each tag uses MERGE and connected to the the content node thru the HAS, e.g content-[:HAS]->tag
            // remember that MERGE will create if it doesn't exist otherwise based on the properties provided 
            String[] tags = content.tagstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            MappedContent contentItem = _graphClient.Cypher
            .Match(" (user { username: {u}}) ")
            .WithParam("u", username)
            .CreateUnique(" (user)-[:CURRENTPOST]->(newLP:Content { title:{title}, url:{url}, " +
            " tagstr:{tagstr}, timestamp:{timestamp}, contentId:{contentId} }) ")
            .WithParams(new { title = content.title, url = content.url, 
                tagstr = content.tagstr, timestamp=content.timestamp, contentId=content.contentId})
            .With("user, newLP")
            .ForEach(" (tagName in {tags} | " +
            "MERGE (t:Tag {wordPhrase:tagName})" +
            " MERGE (newLP)-[:HAS]->(t) " +
            " )")
            .WithParam("tags",tags)
            .With("user, newLP")
            .OptionalMatch(" (newLP)<-[:CURRENTPOST]-(user)-[oldRel:CURRENTPOST]->(oldLP)")
            .Delete(" oldRel ")
            .Create(" (newLP)-[:NEXTPOST]->(oldLP) ")
            .With("user, newLP")
            .Return(() => Return.As<MappedContent>(" { contentId: newLP.contentId, title: newLP.title, url: newLP.url," +
            " tagstr: newLP.tagstr, timestamp: newLP.timestamp, userNameForPost: {u}, owner: true } "))
            .Results.Single();

            return contentItem;
        }

        public MappedContent edit(Content content, string username)
        {
            content.tagstr = removeTrailingComma(content.tagstr);

            // splits up the comma separated string into arrays and removes any empties. 
            // each tag uses MERGE and connected to the the content node thru the HAS, e.g content-[:HAS]->tag
            // remember that MERGE will create if it doesn't exist otherwise based on the properties provided 
            String[] tags = content.tagstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            MappedContent mappedContent = _graphClient.Cypher
                .Match(" (c:Content {contentId:{contentId}})-[:NEXTPOST*0..]-()-[:CURRENTPOST]-(user { username: {u}}) ")
                .WithParams(new { u = username, contentId = content.contentId})
                .Set(" c.title = {title}, c.url = {url}, c.tagstr = {tagstr} ")
                .WithParams(new {title=content.title, url=content.url, tagstr=content.tagstr })
                .ForEach(" (tagName in {tags} | " +
                "MERGE (t:Tag {wordPhrase:tagName})" +
                " MERGE (c)-[:HAS]->(t) " +
                " )")
                .WithParam("tags", tags)
                .With("user, c")
                .Return(() => Return.As<MappedContent>(" { contentId: c.contentId, title: c.title, url: c.url," +
            " tagstr: c.tagstr, timestamp: c.timestamp, userNameForPost: user.username, owner: true } "))
            .Results.Single();
                
            return mappedContent;
        }

        public void delete(string contentId, string username)
        {
            _graphClient.Cypher
                .Match("(u:User { username: {u} }), (c:Content { contentId: {contentId} })")
                .WithParams(new { u = username, contentId = contentId})
                .With("u,c")
                .Match(" (u)-[:CURRENTPOST]->(c)-[:NEXTPOST]->(nextPost) ")
                .Where("nextPost is not null ")
                .CreateUnique(" (u)-[:CURRENTPOST]->(nextPost) ")
                .With(" count(nextPost) as cnt ")
                .Match(" (before)-[:NEXTPOST]->(c:Content { contentId: {contentId}})-[:NEXTPOST]->(after) ")
                .Where(" before is not null AND after is not null ")
                .CreateUnique(" (before)-[:NEXTPOST]->(after) ")
                .With(" count(before) as cnt ")
                .Match(" (c:Content { contentId: {contentId} })-[r]-() ")
                .Delete("c,r")
                .ExecuteWithoutResults();

        }

        private String removeTrailingComma(string s)
        {
            s = s.Trim();
            if (s.EndsWith(","))
            {
                s = s.Substring(0, s.Length- 1);
            }

            return s;
        }

        
    }
}