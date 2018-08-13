Practical Neo4j for Python
==================

I used Eclipse 3.7 with JDK 7 for this project.  This project also requires Python 2.7 or greater.

I used virtualenv (http://docs.python-guide.org/en/latest/dev/virtualenvs/) to install python 2.7 as part of the project

After the project has been downloaed and unpacked, then you should:

1. Setup a Neo4j instance by going to www.graphstory.com/practicalneo4j
2. Copy the database connection information
3. Import the project into your IDE
4. Make sure to add a writable "logs" directory to under the {projectroot}/app folder
5. Update the database connection information in the {projectroot}/app/bottle/graphstory.py file with your connection information
6. You may need to add python packages as part of the installation, e.g. bottle, py2neo.  Your mileage may vary
7. Configure your web server. I used mod_wsgi with Apache http.

	```xml
	LoadModule wsgi_module  libexec/apache2/mod_wsgi.so
	WSGIPythonHome /path/to/your/practicalneo4j-python/virtual/env/bin/python
	```
8. Make sure the files are readable by the web server user, e.g. the apapter.wsgi files and your python files
9. The virtual host configuration for apache web server is below. 

```xml
<VirtualHost *:80>
	ServerName practicalneo4j-python
	DocumentRoot /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/public
	
	<Directory /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/public>
		Options None
		AllowOverride None
		Order allow,deny
		allow from all
	</Directory>
	
	
	WSGIDaemonProcess graphstory user=_www group=_www processes=1 threads=15
	WSGIProcessGroup graphstory
	WSGIApplicationGroup %{GLOBAL}
    WSGIScriptAlias / /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/adapter.wsgi

	Alias /css/ /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/public/css/
	Alias /fonts/ /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/public/fonts/
	Alias /img/ /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/public/img/
	Alias /js/ /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app/public/js/
	
	<Directory /Users/gregorymjordan/Sites/practicalneo4j/practicalneo4j-python/app>
			Order deny,allow
		    Allow from all
	</Directory>
	
</VirtualHost>
```