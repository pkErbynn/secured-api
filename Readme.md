# Overview
Buiding secured .NET 8 APIs using Custom JWT Authentication and Authorization using Microsoft Identity Manager


# SQL Server Setup
Installing SQL Server directly on a Mac (especially with an ARM-based M1/M2 chip) is not straightforward because Microsoft SQL Server is primarily designed to run on Windows and Linux, but not natively on macOS. However, you can run SQL Server on your Mac using Docker, although you may face compatibility issues with ARM architecture.

Here are some alternative steps you can take to install and use SQL Server on your Mac:

## 1. Use Docker to Install SQL Server
Even though there might be compatibility issues, you can try using Docker to run SQL Server on your Mac.

1. Run SQL Server in Docker:
Open your terminal and run the following command to pull the latest SQL Server Docker image:

```shell
docker pull mcr.microsoft.com/azure-sql-edge
```
Then, run the SQL Server container:

```shell
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=myStrong(!)Password' -p 1433:1433 --name sql1 -d mcr.microsoft.com/azure-sql-edge
```
PS: The mcr.microsoft.com/azure-sql-edge image is compatible with *ARM architecture*.

## 2. Use Azure Data Studio to Connect to SQL Server
Once you have SQL Server running in a Docker container, you can use Azure Data Studio to connect to it.

1. Open Azure Data Studio from extension in VSCode

2. Connect to SQL Server:
    - Open Azure Data Studio.
    - Click on "New Connection".
    - Enter the connection details:
        - Server: `localhost`
        - Authentication Type: `SQL Login`
        - Username: `sa`
        - Password: `myStrong(!)Password`
    - Click "Connect".

## 3. Update Your .NET Application Configuration
Update your appsettings.json file with the connection string to connect to the SQL Server instance running in Docker:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CustomDemoIdentityDb;User Id=sa;Password=myStrong(!)Password;TrustServerCertificate=True"
  },
}
```

## 4. Run Migrations and Update the Database
While SQL Server is running and the application is configured to connect to it, run the migrations and update the database:

```shell
dotnet ef migrations add InitialCreate
dotnet ef database update
```
