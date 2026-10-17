# VehicleCategoryManager
Vehicle Classification Management web application for CreditWorks technical test


# Setup
## Required Software:
- .NET 10.0 SDK (for running and building the application)
- .NET ef tools (for applying database migrations)
  - If not installed run `dotnet tool install --global dotnet-ef` in the terminal.
- SQL Server (express used in development, 2019 and newer supported)
  - This can be installed locally or run in a Docker Container, ensure the connection string used is correct when running the application.

## Optional Software:
- Docker Desktop (for running application in a container)

## Database Requirements:
- SQL Server 2019 or newer
- Dotnet EF Tools (for applying database migrations)


## Configuration Steps:
1. Create your SQL connection string, the following is an example requiring :
	`Server=SERVER;Database=VehicleManagementSystem;User Id=USER;Password=PASSWORD;Encrypt=True;TrustServerCertificate=True`
	- To use this connection string, Replace SERVER, USER, and PASSWORD with your appropriate server/credentials.
	- The user should have appropriate permissons to create the database if it does not exist.
	- The database name defaults to VehicleManagementSystem, this can be changed by replacing the value in the connection string.
	- If you plan to run the application through docker using a local db, use `host.docker.internal` as SERVER, this will route to your `localhost`
2. keep this connection string handy, it will be used to run the application.

#### If planning to run through Visual Studio in debug mode:
Create a file called `appsettings.Development.json` in the folder `VehicleCategoryManager/VehicleCategoryManager` and copy the following into it replacing CONNECTIONSTRING with the connection string you just created:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "CONNECTIONSTRING"
  }
}
```


## Database Creation/Migration Instructions:
1. Open a terminal and navigate to the project root directory. *(The folder containing the .sln file)*
2. Run the following command.
	```bash
	dotnet ef database update --project .\VehicleCategoryManager.Persistance\ --startup-project .\VehicleCategoryManager\VehicleCategoryManager\
	```

## How to Build:
Build steps are different by how you plan to run the application:

### Docker (Easy
Run the following command from the root directory. *(The folder containing the .sln file)*
  ```bash
  docker build --build-arg "ConnectionStrings__DefaultConnection=CONNECTIONSTRING" -t vehicle-category-manager .
  ```

### Visual Studio
1. Open the solution in Visual Studio.
2. In the toolbar click Build->Rebuild Solution.

### Command Line
Run the following command from the root directory. *(the folder containing the .sln file)*
  ```bash
  dotnet build
  ```
If planning to run in release mode, instead use the following command:
  ```bash
  dotnet build -c Release
  ```

## How to Run:
First ensure the configuration steps and database migration instructions have been completed successfully.
Then follow one of the following methods to run the application:

### Docker: (Easy)
1. Ensure Docker Desktop is installed and running.
2. Open a terminal and navigate to the project root directory. *(The folder containing the .sln file)*
3. Run the following command to build and run the application in a Docker container:
   ```bash
   docker run -d --name vehicle-category-manager -p 7076:7076 --add-host=host.docker.internal:host-gateway -e "ConnectionStrings__DefaultConnection=CONNECTIONSTRING" vehicle-category-manager
   ```
4. The application will be accessible at `http://localhost:7076` in your web browser.

This will run a production build that automatically applies any pending migrations to the database.

### Visual Studio:
1. Open the solution in Visual Studio.
2. Ensure the VehicleCategoryManager project is set as the startup project.
  - This should be bold in the Solution Explorer, if not right click on the project and select "Set as Startup Project".
3. Set the correct launch profile (http or https) in the toolbar. *(http is prefered in this instance)*
4. Set the correct configuration (Debug or Release) in the toolbar.
5. Press F5 or click the "Start" button to run the application.
6. The application will be accessible at `http://localhost:7076` or `http://localhost:7075` in your web browser.

### Command Line:
1. Open a terminal and navigate to the project root directory. (The folder containing the .sln file)
2. Run the following command with your connection string from **Configuration Steps** to run the application:
   ```bash
   dotnet run --project VehicleCategoryManager --launch-profile http -- --ConnectionStrings:DefaultConnection="CONNECTIONSTRING"
   ```
   - For Release mode, use the following instead:
   ```bash
   dotnet run --project VehicleCategoryManager -c Release --launch-profile http -- --ConnectionStrings:DefaultConnection="CONNECTIONSTRING"
   ```
3. The application will be accessible at `http://localhost:7076` in your web browser.

**Note:** *changing launch profile to https will make the application accessable from https://localhost:7075 (This may cause issues with untrusted dev ssl certificates)*


## How to Execute Tests:

### In Visual Studio:
Option 1:
  - Open the solution in Visual Studio.
  - Open the Test Explorer (Test > Test Explorer).
  - Click "Run All" to execute all tests.
Option 2:
  - Open the solution in Visual Studio.
  - Right-click on the test project in the Solution Explorer.
  - Select "Run Tests" to execute all tests.

### In Command Line:
- Open a terminal and navigate to the root directory. *(The folder containing the .sln file)*
- Run the following command to execute all tests:
  ```bash
  dotnet test
  ```


# Design Notes:
## Solution Architecture
I have opted to develop a Blazor WebAssembly Application using global interactivity to have a SPA (Single Page Application) structure with an Entity Framework Core Database set to use SQL Server.
Testing uses an in-memory SQLLite database, this prevents the requirement of a second db while still maintaining relational data integrity.

I chose this for development as I am most comfortable with C# and Razor syntax, choosing the SPA structure enforces me to use a client side structure with API for demonstration of skills relating to API devlopment.


## Database Design:
The database consists of the following tables:
- VehicleWeightClass: This table contains the category information for each vehicle category, including the min and max weight values, the category name, and the icon associated with the category.
- Manufacturer: This table contains the manufacturer names, this is seeded however extendable.
- Vehicle: This table contains the vehicle information, including the vehicle owner's name, year of manufacture, weight which is used to determine the category of the vehicle, and a foreign key to the manufacturer table.
  - I have made the year use a short integer to save on storage space, as the year will always be a 4 digit number and will never exceed 9999. this creates the column in sql as a smallint which is 2 bytes instead of an int which is 4 bytes.
  - There is no foreign key for the Weight class, this is found when a vehicle is loaded to ensure it is always up to date.


## Category Calculation Approach:
I have opted to use an Inclusive Min / Exclusive Max approach to the category calculation approach.
I chose this approach to ensure a more readable and user friendly experience when entering weights so that each weight category has the option of being between two integer values instead of forcing a 2 point precision decimal to avoid failing validation on weight overlaps.


## Category Boundary Rules:
- No gaps or overlaps are allowed between categories, this is to ensure that all weights are accounted for and that no weight can be classified as more than one category.
- When adding a new category, the user must specify the min and max values for the category, and the application will alter the boundaries of the adjacent categories.
- If a new category is added that fully overlaps an existing category, validation will prevent the addition and display an error message.


## Important Assumptions:
- Two categories may not share an icon, this assumption is due to displaying the icons as a category identifier.
- On deletion of a category, the categories before and after must grow to take its place:
  - If the category is in the center of two categories, both the category before and the one after must grow to take its place.
  - If the category was the smallest, (0 inclusive) then the next category must grow to include 0.
  - If the category was the largest, (no upper limit) then the previous category must have its upper limit removed to accomidate.
  - Due to this assumption, the only validation required for weight class is to ensure an added record does not entirely overlap an existing category, as the application will handle the rest of the validation and adjustments to ensure no gaps or overlaps are present in the categories.
- A single owner may have multiple vehicles with the same weight, year and manufacturer, this accomodates fleets although there is no current way to differentiate between the vehicles.
- A vehicle cannot have a year before 1800, the first gasoline powered vehicle was created in 1885, 1800 is a cleaner value to remove excessively bad data and prevent user misinput. 


## Significant Design Decisions:
- I have decided to use a query to get the classification of a vehicle instead of storing it against the vehicle. This is to ensure that if a category is changed, the classification of a vehicle will be updated automatically without requiring any additional processing.
- A user may not delete the last remaining category, this is to ensure that there is always at least one category to classify vehicles against.
- Categories will automatically expand or shrink to remove any gaps left by a deleted/altered category, this is to ensure that there are no gaps in the category boundaries. These changes are previewed to the user before being applied to ensure that they are aware of the changes being made.
- For numeric weight inputs, steps are done in 100, this will make it easier when a user wants to use the buttons to alter weight.
- Sorting is handled on client side, this is a faster experience for the user. On larger datasets where paging is required, this would be moved to server side to keep paging accurate.

## Known Limitations:
- To save on required validation of image links or storing images within the database, I have provided additional category icons for any additional custom icons. 
  - This is also in place to prevent a user from using the same image with different image links or saving the same image twice with 2 different MD5 Hashes. 
   *(If storing images, I would use MD5 hash to prevent duplicates of an image, though metadata changing could get around this)*
  - Due to this, there is a hard limit on the number of categories a user may have at any time limited to the number of categories, this is due to the assumption two categories cannot share an icon.
- There is no way to differentiate a vehicle that has the same owner, weight, manufacturer and year, this could be solved by the inclusion of a number plate in the data structure.
- There is a hard limit of 8 weight classes, this is due to the number of icons I have set up for this project, adding more icons is done by expanding the list in the IconPicker and adding a new svg to the icons folder on the client.
- Due to no authentication, all visitors to the side can use CRUD operations on all records.


# Security Considerations:
- At the time of submission, all referenced packages had been checked, no packages were listed as vulnerable through NuGet.
- All database queries are parameterized through Entity Framework Core to prevent SQL Injection attacks.
- Dto's are used to prevent overposting attacks, and only the required fields are sent to the API for processing.
- Validation and automatic data correction happen server side to prevent scripting attacks.


# Additional Notes:
- I have opted not to use any agentic AI within the creation of this technical test to demonstrate my skills as a developer.
- An alternative to using Blazor WASM that I could use is Blazor Server: 
  - This would simplify the application and not require an API endpoint as the web page is rendered entirely on the server before being sent to the client. 
  - Server is also more secure against scripting and injection attacks as all code is stored and executed on the server, though more vulnerable to DoS attacks as it maintains an active connection with each client.
- I have decided to use SVGs as my icons, this is due to the low file size and high scalability, it also gives me the option to copy the file and change colours so that I may have more icons without a large amount of additional work.
- When performing smoke tests, I have added new tests where I have noticed issues.