BlogEngineTK - simple and fast blog engine, but optional functionality wasn't implemented.
Sorry but only russian version (comments, ui).

-------------------------------------------------------------------------------
Used technologies:
HTML, CSS, JavaScript, C#, ASP.NET MVC 4, Entity Framework, MS SQL Server, 
Ninject (Dependency Injection), Moq (Mocking) AJAX, JSON, Log4Net.

-------------------------------------------------------------------------------

Config to deployment:
In web.config:
1. debug = false;
2. set elFinder phys path and url;
3. connection string to db.

Set "<MvcBuildViews>true</MvcBuildViews>" in *.csproj file to compile views at building process 
and output errors in them;

Build all solution by "Release" config.

-------------------------------------------------------------------------------
