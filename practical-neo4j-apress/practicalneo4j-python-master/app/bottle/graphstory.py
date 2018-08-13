from bottle import run, route, debug, redirect, request, response, put
from bottle import default_app
from content import Content
from location import Location
from product import Product
from purchase import Purchase
from tag import Tag
from user import User
from userlocation import UserLocation
from py2neo import neo4j, node, rel
from bottle.ext.pystache import view, template
from json import dumps
import logging
from test.test_popen import python

logging.basicConfig(filename='logs/practicalneo4j-python-debug.log', level=logging.INFO)

graph_db = neo4j.GraphDatabaseService("https://username:password@host:7473/db/data/")


# home layout
homelayout = "public/templates/global/base-home.html"
# app layout
applayout = "public/templates/global/base-app.html"
# cookie name for login
graphstoryUserAuthKey = "graphstoryUserAuthKey"

# home page
@route('/')
def index():
    return template("public/templates/home/index.html", layout=homelayout, title="Home")


# login
@route('/login', method='POST')
def login():
    # make sure username was passed
    username = request.forms.get('username').strip().lower()

    if username:

        # look for username
        user = User().get_user_by_username(graph_db, username)
        if user:
            # user found, set cookie and redirect
            response.set_cookie(graphstoryUserAuthKey, user["username"], path="/")
            redirect("/social")

        else:
            # otherwise send back with not found message
            return template("public/templates/home/index.html", layout=homelayout, title="Home",
                            error="The username you entered was not found.")
    # otherwise send back
    else:
        return template('public/templates/home/index.html', layout=homelayout, title="Home",
                        error="Please enter a username.")


# sign up
@route('/signup/add', method='POST')
def signup():
    username = request.forms.get('username').strip().lower()

    # make sure username was passed
    if username:

        # check if username exists
        user = User().get_user_by_username(graph_db, username)
        if user:

            # user found, show message
            return template('public/templates/home/index.html', layout=homelayout, title="Home",
                            error='The username ' + username + ' already exists. Please use a different username.')
        else:

            # save user
            User().save_user(graph_db, username)
            redirect("/msg?u=" + username)

    # otherwise send back
    else:
        return template('public/templates/home/index.html', layout=homelayout,
                        title="Home", error="Please enter a username.")


# create user success - always say 'thank you!'
@route('/msg', method='GET')
def msg():
    username = request.query.u
    # personalize with the username
    return template('public/templates/home/message.html', layout=homelayout, title="Thank You!",
                    msg="Thank you, " + username + " !")

############################################################
# user & friends 
############################################################


# show user
@route('/user', method='GET')
def user():
    user = User().get_user_by_username(graph_db, request.get_cookie(graphstoryUserAuthKey))
    return template('public/templates/graphs/social/user.html', layout=applayout, user=user.get_properties(),
                    title="User Settings")


# edit user's first/last name
@route('/user/edit', method='PUT')
def user_edit():
    User().update_user(graph_db, request.get_cookie(graphstoryUserAuthKey),
                      request.json["firstname"], request.json["lastname"])

    response.content_type = 'application/json'

    return {"msg": "ok"}


# show connected users via FOLLOW relationship
@route('/friends', method='GET')
def friends():
    following = User().following(graph_db, request.get_cookie(graphstoryUserAuthKey))

    return template('public/templates/graphs/social/friends.html',
                    following=following, layout=applayout, title="Friends")


# search for users / returns collection of users as json
@route('/searchbyusername/<username>', method='GET')
def search_by_username(username):
    # get the users' the current user is following
    users = User().search_by_username_not_following(graph_db, 
                                                    request.get_cookie(graphstoryUserAuthKey), username)

    response.content_type = 'application/json'

    # return as json    
    return dumps({"users": User().users_results_as_array(users)})


# follow a user
@route('/follow/<username>', method='GET')
def follow(username):
    following = User().follow(graph_db, request.get_cookie(graphstoryUserAuthKey), username)

    response.content_type = 'application/json'

    return dumps({"following": User().users_results_as_array(following)})


# unfollow a user
@route('/unfollow/<username>', method='GET')
def unfollow(username):
    following = User().unfollow(graph_db, request.get_cookie(graphstoryUserAuthKey), username)

    response.content_type = 'application/json'

    return dumps({"following": User().users_results_as_array(following)})

############################################################
# social graph
############################################################

# view status updates
@route('/social', method='GET')
def social():
    contents = Content().get_content(graph_db, request.get_cookie(graphstoryUserAuthKey), 0)

    if len(contents) > 3:
        morecontent = True
        contents = contents[0:3]
    else:
        morecontent = False

    return template('public/templates/graphs/social/posts.html', layout=applayout, contents=contents,
                    morecontent=morecontent, title="Social")


# returns status updates as json
@route('/postsfeed/<pagenum:int>', method='GET')
def index(pagenum):
    contentsAsJson = Content().content_results_as_list(graph_db, request.get_cookie(graphstoryUserAuthKey), pagenum)

    response.content_type = 'application/json'

    return dumps(contentsAsJson)


# view single status update
@route('/viewpost/<contentId>', method='GET')
def viewpost(contentId):
    statusupdate = Content().get_status_update(graph_db, request.get_cookie(graphstoryUserAuthKey), contentId)

    return template('public/templates/graphs/social/post.html', layout=applayout, content=statusupdate,
                    title=statusupdate[0].title)


# add status update
@route('/posts/add', method='POST')
def add_content():
    
    # get json from the request
    content = request.json
    
    #save the status update
    content=Content().add_content(graph_db, request.get_cookie(graphstoryUserAuthKey), content)
    
    # set response type
    response.content_type = 'application/json'
    
    # return the saved content
    return dumps(content)

# edit the status update
@route('/posts/edit', method='POST')
def edit_content():
        
    # get json from the request
    content = request.json
    
    #update the status update
    content=Content().edit_content(graph_db, request.get_cookie(graphstoryUserAuthKey), content)
    
    # set response type
    response.content_type = 'application/json'
    
    # return the saved content
    return dumps(content)


# delete a status update
@route('/posts/delete/<contentId>', method='GET')
def delete_content(contentId):
    
    #delete the status update
    delete = Content().delete_content(graph_db, request.get_cookie(graphstoryUserAuthKey), contentId)
    
    # set response type
    response.content_type = 'application/json'
    
    # return the response
    return {"msg": "ok"}


############################################################
# interest graph
############################################################

# show tags within the user's network (theirs and those being followed)
@route('/interest')
def interest():
    # get the user's tags
    userTags = Tag().user_tags(graph_db, request.get_cookie(graphstoryUserAuthKey))
    # get the tags of user's friends
    tagsInNetwork = Tag().tags_in_network(graph_db, request.get_cookie(graphstoryUserAuthKey))

    # if the user's content was requested
    if request.query.get('userscontent') == "true":
        contents = Content().get_user_content_with_tag(graph_db,
                                                   request.get_cookie(graphstoryUserAuthKey),
                                                   request.query.get('tag'))
    # if the user's friends' content was requested
    else:
        contents = Content().get_following_content_with_tag(graph_db,
                                                        request.get_cookie(graphstoryUserAuthKey),
                                                        request.query.get('tag'))

    return template('public/templates/graphs/interest/index.html', layout=applayout, userTags=userTags,
                    tagsInNetwork=tagsInNetwork, contents=contents, title="Interest")


# show status updates based on a tag
@route('/tag/<q>')
def index(q):
    response.content_type = 'application/json'
    tags=Tag().search_tags(graph_db,q)
    
    tagArray = []
    # build array
    for t in tags:
            tagArray.append({"name":t.name, "id":t.id, "label":t.label})
    
    return dumps(tagArray)


############################################################
# consumption graph
############################################################

# show products and products VIEWED by user
@route('/consumption', method='GET')
def consumption():
    products = Product().get_products(graph_db, 0)
    next = True
    nextPageUrl = "/consumption/10"

    productTrail = Product().get_product_trail(graph_db, request.cookies[graphstoryUserAuthKey])

    return template('public/templates/graphs/consumption/index.html', 
                    layout=applayout, products=products,
                    productTrail=productTrail, next=next, nextPageUrl=nextPageUrl, title="Consumption")


# return partial - helps to perform auto scroll of products
@route('/consumption/<pagenum>', method='GET')
def index(pagenum):
    curpage = int(pagenum)

    # get products for this page
    products = Product().get_products(graph_db, curpage)

    # bump the page total
    next = True
    curpage = curpage + 10

    # set the next GET route
    nextPageUrl = "/consumption/" + str(curpage)

    return template('public/templates/graphs/consumption/product-list.html', 
                    products=products, next=next,
                    nextPageUrl=nextPageUrl)


# add a product via VIEWED relationship and return VIEWED products
@route('/consumption/add/<productNodeId:int>', method='GET')
def consumption_add(productNodeId):
    
    #save the view and return the full list of views
    productTrailAsJson=Product().create_user_view_and_return_views(graph_db, 
                                                                   request.cookies[graphstoryUserAuthKey], 
                                                                   productNodeId)
    
    #set the response type
    response.content_type = 'application/json'
    
    #return the list of views
    return dumps(productTrailAsJson)


# displays products that are connected to users via a tag relationship
@route('/consumption/console', method='GET')
def index():
    # was tag supplied, then get product matches based on specific tag
    if request.query.get('tag'):
        usersWithMatchingTags = Product().get_products_has_specific_tag_and_user_uses_specific_tag(graph_db,
                                                                                          request.query.get('tag'))
    # otherwise return all product matches as long as at least one tag matches against the users
    else:
        usersWithMatchingTags = Product().get_products_has_a_tag_and_user_uses_a_matching_tag(graph_db)

    return template('public/templates/graphs/consumption/console.html', layout=applayout,
                    usersWithMatchingTags=usersWithMatchingTags, title="Consumption Console")


############################################################
# location graph
############################################################

# show locations nearby or locations that have a specific product
@route('/location')
def location():
    # get user location
    userlocations = UserLocation().get_user_location(graph_db, request.get_cookie(graphstoryUserAuthKey))

    distance = request.query.get('distance')

    # was distances provided
    if distance:

        # use first location
        ul = userlocations[0]

        productNodeId = request.query.get('productNodeId')

        # test for productNodeId
        if productNodeId:

            pnid = int(productNodeId)
            # get locations that have product
            locations = Location().locations_within_distance_with_product(graph_db,
                                                                      UserLocation().get_lq(ul, distance), 
                                                                      pnid, ul)
            productNode = graph_db.node(pnid)
            return template('public/templates/graphs/location/index.html', 
                            layout=applayout, title="Location",
                            productTitle=productNode["title"], locations=locations, 
                            mappedUserLocation=userlocations)
        # no product provided
        else:
            # get locations
            locations = Location().locations_within_distance(graph_db, 
                                                             UserLocation().get_lq(ul, distance), 
                                                             ul,"business")
            return template('public/templates/graphs/location/index.html', 
                            layout=applayout, title="Location",
                            locations=locations, mappedUserLocation=userlocations)

    # return search template for locations
    else:
        return template('public/templates/graphs/location/index.html', layout=applayout, title="Location",
                        mappedUserLocation=userlocations)


# return product array as json
@route('/productsearch/<q>')
def product_search(q):
    # get matches
    productsFound = Product().product_search(graph_db, q + ".*")

    # create array
    products = Product().product_results_as_list(productsFound)

    # set response type
    response.content_type = 'application/json'

    # return as json
    return dumps(products)


############################################################
# intent graph
############################################################

# purchases by friends
@route('/intent', method='GET')
def intent():
    # get result set
    result = Purchase().friends_purchase(graph_db, request.get_cookie(graphstoryUserAuthKey))

    return template('public/templates/graphs/intent/index.html', layout=applayout, 
                    title="Products Purchased by Friends",
                    mappedProductUserPurchaseList=result)

# specific product purchases by friends
@route('/intent/friendsPurchaseByProduct', method='GET')
def friends_purchase_by_product():
    # get or use default product title
    producttitle = request.query.producttitle or 'Star Wars Mimobot Thumb Drives'
    # get result set
    result = Purchase().friends_purchase_by_product(graph_db, request.get_cookie(graphstoryUserAuthKey), producttitle)
    return template('public/templates/graphs/intent/index.html', layout=applayout, 
                    title="Specific Products Purchased by Friends",
                    mappedProductUserPurchaseList=result, producttitle=producttitle)

# friends bought specific products. match these products to tags of the current user
@route('/intent/friendsPurchaseTagSimilarity', method='GET')
def friends_purchase_tag_similarity():
    # get result set
    result = Purchase().friends_purchase_tag_similarity(graph_db, request.get_cookie(graphstoryUserAuthKey))
    return template('public/templates/graphs/intent/index.html', layout=applayout, 
                    title="Products Purchased by Friends and Matches User's Tags",
                    mappedProductUserPurchaseList=result)

# friends that are nearby bought this product.
# the product should also matches tags of the current user
@route('/intent/friendsPurchaseTagSimilarityAndProximityToLocation', method='GET')
def friends_purchase_tag_similarity_and_proximity_to_location():
    # get user location
    userlocations = UserLocation().get_user_location(graph_db, request.get_cookie(graphstoryUserAuthKey))
    # use first location
    ul = userlocations[0]

    # get result set
    result = Purchase().friends_purchase_tag_similarity_and_proximity_to_location(graph_db,
                                                                           request.get_cookie(graphstoryUserAuthKey),
                                                                           UserLocation().get_lq_distance_set(ul))

    return template('public/templates/graphs/intent/index.html', layout=applayout, 
                    title="Products Purchased by Friends Nearby and Matches User's Tags",
                    mappedProductUserPurchaseList=result, mappedUserLocation=userlocations)

# turn off debug in PROD
debug(True)
