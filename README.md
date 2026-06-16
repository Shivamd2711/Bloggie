# Bloggie

Bloggie is a lightweight ASP.NET Core web application designed for authoring and publishing blog posts. It features user authentication (via ASP.NET Core Identity), blog management, comments, tags, and basic email settings support. The project targets .NET 8 and includes EF Core migrations along with a Dockerfile for containerized deployment.

## Key Features
- Create, edit, and delete blog posts
- Tagging and basic blog search/filters
- Commenting on posts
- User authentication and role management (ASP.NET Core Identity)
- EF Core migrations for database schema management
- Dockerfile for containerized deployment

## Tech Stack
- .NET 8 (ASP.NET Core)
- ASP.NET Core MVC / Razor views
- Entity Framework Core (EF Core) Migrations
- ASP.NET Core Identity
- Bootstrap (front-end)

## Prerequisites
Before you begin, ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en/download)
- A SQL database server (SQL Server, or change to your preferred provider in configuration)
- (Optional) Docker for containerized deployment

## Local Setup
Follow these steps to set up the project locally:

1. **Clone the repository**

   git clone https://github.com/Shivamd2711/Bloggie.git
   cd Bloggie/Bloggie.Web

2. **Update configuration**

- Open `appsettings.json` (or `appsettings.Development.json`) and set a valid connection string for your database under `ConnectionStrings`. The example below uses SQL Server:

   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=BloggieDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }

- Configure mail settings (if you plan to use email features) under `MailSettings` to match `Bloggie.Web.Models.EmailModels.MailSettingModel` (mail, display name, password, host, port).

3. **Restore and build the project**

   dotnet restore
   dotnet build

4. **Apply EF Core migrations**

   From the `Bloggie.Web` project folder, run:

   dotnet ef database update

   If you prefer using Visual Studio, open the solution and use the Package Manager Console:

   Update-Database -Project Bloggie.Web

5. **Run the application**

   dotnet run --project Bloggie.Web

   Bydefault, the Dockerfile and some local settings expose the app on port 8080 when running in a container.

## Running with Docker
To build and run the container (ensure Docker is installed):

docker build -t bloggie:web ./Bloggie.Web
docker run -p 8080:8080 --env-file .env bloggie:web

Set environment variables or provide an `appsettings.json` inside the container to configure connection strings and mail settings.

## Configuration and Environment
- **`ASPNETCORE_ENVIRONMENT`**: Set to `Development` or `Production` based on your environment.
- **Connection Strings**: Configure your `DefaultConnection` in `appsettings.json`.
- **Mail Settings**: Configure `MailSettings` for SMTP usage.

## Database Migrations
Migrations are stored in the `Migrations` folder(s). Use the following commands to manage schema changes:

dotnet ef migrations add <Name>
dotnet ef database update

## Contributing
Contributions are welcome! Please open issues or pull requests on the repository. Ensure to follow the code style and formatting conventions used in the project.

## License
Refer to the repository for licensing information. Third-party libraries (such as Bootstrap and jQuery Validation) include their own licenses in `wwwroot/lib/*/LICENSE`.

## Notes
- The project includes ASP.NET Core Identity and an `AuthDb` migration; ensure your authentication database connection is configured before running authentication flows.
- If you encounter errors during migrations, verify that your connection string user has permissions to create databases, or create the database manually and run the migrations.

---