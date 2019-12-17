This is a music rating web application that was made for a university project and implemented using ASP.NET Core 3.0.
## How to build and run the web app
Make sure that the Entity Framework Core CLI tools have been installed beforehand with `dotnet tool install --global dotnet-ef`.

 1. Build the project `MusicRatingWebApp`. You may also build the `UnitTests` project, but that is not needed.
 2. While in the `MusicRatingWebApp` project directory, run `dotnet ef database update` in order to update (make) the database from the last migration.
 3. Run the project. You should see a website running on `https://localhost:44342/`.