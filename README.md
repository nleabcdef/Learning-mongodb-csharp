# Learning MongoDB with Asp.net Core
  ## About Web Apis,
    - allows users to create, manage their passwords/login details and securly store them in MongoDB
        
# Features used in Apis
  - CRUD Operations
  - Aggregate

# Configuration

by default the Web api connects to In-Memory repository(database) can be configured to use MongoDB

-  add the below app setting in /src/PassphraseManagerSvc/appsettings.json
    -  to enable or disable MongoDB    
		```json
            "PreferredRepo": 1, // 1 - MongoDB, 2 - InMemory
	        "Mongo" : {
              "DbConnection" : "mongodb+srv://<<username>>:<<password>>@<<servername>>/<<database>>?retryWrites=true&w=majority"
            }
		 ```
	-  to enable or disable to load/create test records(randomly generated), while app/api startup
		```json
		    "LoadTestData" : true
		 ```
# Quick start: 
-    Open /src/PassphraseManagerSvc/PassphraseManagerSvc.csproj poject file in vscode and run it or
-    to Run, using .Net CLI
        
```bash
    > cd  /src/PassphraseManagerSvc/
    > dotnet run
```

     
- browse Apis using  http://localhost:5000/

# References
    - MongoDB docs - https://docs.mongodb.com/manual/reference/
    - MongoDB University free courses
        - https://university.mongodb.com/course_completion/830dba9a-de04-4889-997c-6daf6887b334
    - MongoDB for .NET Developers
        - https://university.mongodb.com/course_completion/70f8656d-b786-485a-adf6-9d64b0c8716f

# License : Apache License 2.0
 - read more here http://www.apache.org/licenses/ 

