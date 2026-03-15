# MerProject
# How TO RUN:
## Prerequisites : 
# .NET 8 SDK, SQL SERVER(SSMS), Angular, Visual studio 2022, Visual studio code

## HOW TO RUN
# Open MERPROJ backend in visual studio 2022, check connection string in appsettings.json, your database name could differentiate of the given one in the code
# If database does not exist, use code first migration, run Update-Database
# After the database is created, run the backend for seeding resources 
# Swagger should open and CRUD methods should return Ok after trying them out, 
# Run Visual studio code and open the  merproj-angular folder
# Run angular project with ng serve
# It should refer to localhost domain and open it
# Check if frontend is successfully speaking with backend


## Design choices
# Most design choices were explained in the task alone so i was not straying from them, its a classic backend frontend project
# For the backend i used ASP.NET Web API and for the frontend i used Angular
# At first i was going for the database first approach but realized half way through that it was a code first task so i went with migrations
# I used one of the SOLID principles such as dependency injection to give other classes crucial functionalities such as registering a service
# For the frontend we were tasked with using angular, in my opinion i wouldve liked a classic MVC project mixed with the api where we have the frontend displayed with razor views that use viewmodels because it wouldve had a cleaner look and everything would be bundled up in one application 
# Considering im new to angular it has suprised me, the way the generate components and everything is tidy in folders is pretty nice


## What i could improve on if i had more time
# I would make unit tests and integration tests
# I would make a better and simpler design with simpler UI/UX but i was in a hurry so for the frontend css part i was hurriedly using claude for design choices and help

## How long did it take
# Starting time : 3/9/2026
# End time: 3/15/2026
# 3/9/2026 : Planning and setting up took about 2 hours
# 3/10/2026: Backend creation and database creation with service layer functions, controllers, interface, and api CRUD methods took about a day
# 3/11/2026: More improvements to api methods, took about 3 hours
# 3/12/2026: Frontend creation, learning what angular is, how to run it, use it etc, 3 hours
# 3/13/2026: Angular creating components, routing , creating CORS in backend , 4 hours
# 3/14/2026: Optimization, final touches, pushing to git, 4 hours
# 3/15/2026: Video creation, final day.