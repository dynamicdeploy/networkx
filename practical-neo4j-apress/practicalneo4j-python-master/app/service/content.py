from py2neo import neo4j, ogm, node, rel
import uuid
import time
from datetime import datetime


class Content():
    def get_content(self, graph_db, username, skip):
        query = neo4j.CypherQuery(graph_db,
                  " MATCH (u:User {username: {u} })-[:FOLLOWS*0..1]->f  " +
                  " WITH DISTINCT f,u " +
                  " MATCH f-[:CURRENTPOST]-lp-[:NEXTPOST*0..3]-p   " +
                  " RETURN  p.contentId as contentId, p.title as title, " +
                  " p.tagstr as tagstr, p.timestamp as timestamp, " +
                  " p.url as url, f.username as username, f=u as owner  " +
                  " ORDER BY p.timestamp desc SKIP {s} LIMIT 4 ")
        params = {"u": username, "s": skip}
        result = query.execute(**params)
        for r in result:
            setattr(r, "timestampAsStr",
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%m/%d/%Y') + " at " +
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%I:%M %p')
            )
        return result
    
    # add a status update
    def add_content(self, graph_db, username, content):
        
        tagstr=self.trim_content_tags(content["tagstr"])
        tags = tagstr.split(",")
        ts = time.time()
    	
        query = neo4j.CypherQuery(graph_db,
          " MATCH (user { username: {u}}) " +
          " CREATE UNIQUE (user)-[:CURRENTPOST]->(newLP:Content { title:{title}, " +
          " url:{url}, tagstr:{tagstr}, timestamp:{timestamp}, contentId:{contentId} }) " +
          " WITH user, newLP" +
          " FOREACH (tagName in {tags} |  " +
          " MERGE (t:Tag {wordPhrase:tagName}) " +
          " MERGE (newLP)-[:HAS]->(t) " +
          " )" +
          " WITH user, newLP " +
          " OPTIONAL MATCH  (newLP)<-[:CURRENTPOST]-(user)-[oldRel:CURRENTPOST]->(oldLP)" +
          " DELETE oldRel " +
          " CREATE (newLP)-[:NEXTPOST]->(oldLP) " +
          " RETURN newLP.contentId as contentId, newLP.title as title, newLP.tagstr as tagstr, " +
          " newLP.timestamp as timestamp, newLP.url as url, {u} as username, true as owner ")
        params = {"u": username, "title": content["title"].strip(), 
                  "url": content["url"].strip(),
                  "tagstr":tagstr, "timestamp":ts,"contentId": str(uuid.uuid1()), "tags":tags}
        result = query.execute(**params)
        for r in result:
            setattr(r, "timestampAsStr",
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%m/%d/%Y') + " at " +
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%I:%M %p')
            )
        return result
    
    # edit a status update
    def edit_content(self, graph_db, username, content):
        
        tagstr=self.trim_content_tags(content["tagstr"])
        tags = tagstr.split(",")
        
        query = neo4j.CypherQuery(graph_db,
          " MATCH (c:Content {contentId:{contentId}})-[:NEXTPOST*0..]-()-[:CURRENTPOST]-(user { username: {u}}) " +
          " SET c.title = {title}, c.url = {url}, c.tagstr = {tagstr}" +
          " FOREACH (tagName in {tags} |  " +
          " MERGE (t:Tag {wordPhrase:tagName}) " +
          " MERGE (c)-[:HAS]->(t) " +
          " )" +
          " RETURN c.contentId as contentId, c.title as title, c.tagstr as tagstr, " +
          " c.timestamp as timestamp, c.url as url, {u} as username, true as owner ")
        params = {"u": username, "contentId": content["contentId"], 
                  "title": content["title"], "url": content["url"],"tagstr":tagstr, "tags":tags}
        result = query.execute(**params)
        for r in result:
            setattr(r, "timestampAsStr",
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%m/%d/%Y') + " at " +
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%I:%M %p')
            )
        return self.content_array(result);

    # delete a status update
    def delete_content(self, graph_db, username, contentId):
        query = neo4j.CypherQuery(graph_db,
          " MATCH (u:User { username: {u} }), (c:Content { contentId: {contentId} }) " +
          " WITH u,c " +
          " MATCH (u)-[:CURRENTPOST]->(c)-[:NEXTPOST]->(nextPost) " +
          " WHERE nextPost is not null " +
          " CREATE UNIQUE (u)-[:CURRENTPOST]->(nextPost) " +
          " WITH count(nextPost) as cnt " +
          " MATCH (before)-[:NEXTPOST]->(c:Content { contentId: {contentId}})-[:NEXTPOST]->(after) " +
          " WHERE before is not null AND after is not null " +
          " CREATE UNIQUE (before)-[:NEXTPOST]->(after) " +
          " WITH count(before) as cnt " +
          " MATCH (c:Content { contentId: {contentId} })-[r]-() " +
          " DELETE c, r")
        params = {"u": username, "contentId": contentId}
        result = query.execute(**params)
        return result

    def get_status_update(self, graph_db, username, contentId):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (c:Content {contentId:{contentId}})-[:NEXTPOST*0..]-()-[:CURRENTPOST]-(o:User), " +
                                  " (u:User {username: {u} }) " +
                                  " RETURN c.contentId as contentId, c.title as title, c.tagstr as tagstr, " +
                                  " c.timestamp as timestamp, c.url as url, o.username as username, o=u as owner ")
        params = {"u": username, "contentId": contentId}
        result = query.execute(**params)
        for r in result:
            setattr(r, "timestampAsStr",
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%m/%d/%Y') + " at " +
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%I:%M %p')
            )
        return result

    def get_user_content_with_tag(self, graph_db, username, wordPhrase):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (u:User {username: {u} })-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-p " +
                                  " WITH DISTINCT u,p" +
                                  " MATCH p-[:HAS]-(t:Tag {wordPhrase : {wp} } )" +
                                  " RETURN  p.contentId as contentId, p.title as title, p.tagstr as tagstr, " +
                                  " p.timestamp as timestamp, p.url as url, u.username as username, true as owner" +
                                  " ORDER BY p.timestamp DESC")
        params = {"u": username, "wp": wordPhrase}
        result = query.execute(**params)
        for r in result:
            setattr(r, "timestampAsStr",
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%m/%d/%Y') + " at " +
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%I:%M %p')
            )
        return result

    def get_following_content_with_tag(self, graph_db, username, wordPhrase):
        query = neo4j.CypherQuery(graph_db,
                                  " MATCH (u:User {username: {u} })-[:FOLLOWS]->f" +
                                  " WITH DISTINCT f" +
                                  " MATCH f-[:CURRENTPOST]-lp-[:NEXTPOST*0..]-p" +
                                  " WITH DISTINCT f,p" +
                                  " MATCH p-[:HAS]-(t:Tag {wordPhrase : {wp} } )" +
                                  " RETURN  p.contentId as contentId, p.title as title, p.tagstr as tagstr, " +
                                  " p.timestamp as timestamp, p.url as url, f.username as username, false as owner" +
                                  " ORDER BY p.timestamp DESC")
        params = {"u": username, "wp": wordPhrase}
        result = query.execute(**params)
        for r in result:
            setattr(r, "timestampAsStr",
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%m/%d/%Y') + " at " +
                    datetime.fromtimestamp(int(r.timestamp)).strftime('%I:%M %p')
            )
        return result


    def content_results_as_list(self, graph_db, username, skip):
        # get content
        contents = self.get_content(graph_db, username, skip)

        contentArray = []
        # build array
        for c in contents:
            contentArray.append(
                {"contentId": c.contentId, "url": c.url, "title": c.title, "owner": c.owner, "tagstr": c.tagstr,
                 "username": c.username, "timestampAsStr": c.timestampAsStr})
        # return json
        return {"content": contentArray}
    
    def content_array(self, result):

        contentArray = []
        # build array
        for c in result:
            contentArray.append(
                {"contentId": c.contentId, "url": c.url, "title": c.title, "owner": c.owner, "tagstr": c.tagstr,
                 "username": c.username, "timestampAsStr": c.timestampAsStr})
        # return json
        return contentArray;

    def trim_content_tags(self,tagstr):
        tagstr = tgs=[x.strip() for x in tagstr.split(',')]
        tagstr = (",").join(tagstr)
        tagstr = tagstr.rstrip(',')
        return tagstr