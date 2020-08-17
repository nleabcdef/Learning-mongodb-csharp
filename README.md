# Learning MongoDB with Asp.net Core
    ### About Web Apis,
        - allows users to create, manage their passwords/login details and securly store them in MongoDB
        
# features used in Apis
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
    - cd  /src/PassphraseManagerSvc/
    ```ps
        > dotnet run
     ```
- browse Apis using  http://localhost:5000/

# references
    - MongoDB docs - https://docs.mongodb.com/manual/reference/
    - MongoDB University free cources
        - http://university.mongodb.com/course_completion/830dba9a-de04-4889-997c-6daf6887b334

# License : Apache License 2.0
 - read more here http://www.apache.org/licenses/ 

