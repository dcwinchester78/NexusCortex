# Nexus Cortex Setup Guide

Follow these steps to run the application on your local machine using Visual Studio 2022.

## 1. Database Setup

This project uses **SQL Server**. Specifically, it defaults to using SQL Server LocalDB, which is typically installed automatically with Visual Studio.

1. Open **Visual Studio 2022**.
2. Open the **SQL Server Object Explorer** view (`View > SQL Server Object Explorer`).
3. Connect to your local instance: `(localdb)\MSSQLLocalDB`.
4. Right-click the `Databases` folder and select **Add New Database**. Name it: **`NexusCortex`**
5. Right-click the new `NexusCortex` database and select **New Query**.
6. Open the `src/schema.sql` file located in this repository, copy all of its contents, paste it into the query window, and execute it to create the `Nodes` and `Relationships` tables.

## 2. Connection Strings

Your connection strings are stored in standard ASP.NET Core configuration files:
*   `src/NexusCortex.Api/appsettings.json`
*   `src/NexusCortex.Api/appsettings.Development.json`

By default, `appsettings.json` is configured to look for the LocalDB instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NexusCortex;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
*(If you are using a full SQL Server instance like SQL Express, simply update this connection string).*

## 3. Running the Application

This solution contains an API backend and a Blazor WebAssembly frontend. You need to run them simultaneously.

1. Open `src/NexusCortex.sln` in Visual Studio 2022.
2. In the **Solution Explorer**, right-click the Solution (`Solution 'NexusCortex'`) and select **Configure Startup Projects...**
3. Select **Multiple startup projects**.
4. Set the Action for both **`NexusCortex.Api`** and **`NexusCortex.Web`** to **Start**.
5. Click **OK**.
6. Press **F5** (or click the Start button) to launch the application. 

Visual Studio will start the API server in a console window and launch your browser pointing to the Blazor WebAssembly frontend.