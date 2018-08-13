import sys, os, bottle, py2neo

dir = os.path.dirname(__file__)

sys.path = [dir+'/bottle/'] + sys.path
sys.path = [dir+'/service/'] + sys.path

os.chdir(os.path.dirname(__file__))

# This loads your application
import graphstory 
application = bottle.default_app()

# This monitors your application for changes
import monitor
monitor.start(interval=1.0)
monitor.track(os.path.join(os.path.dirname(__file__), 'site.cf'))