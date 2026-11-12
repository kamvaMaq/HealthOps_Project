HealthOps_Project

This project is a functional ASP.NET Core MVC skeleton with:
- Identity (seeded admin: admin@example.com / Admin@123)
- EF Core DbContext and models
- SignalR chat hub and client
- EmailService (SMTP config in appsettings.json)
- Basic CRUD for Patients and Admissions (with transactional bed allocation)

To run:
1. Extract ZIP.
2. Update appsettings.json ConnectionStrings if needed.
3. (Optional) Create EF migrations locally:
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
4. Run with `dotnet run`.

Notes:
- Email sending requires SMTP configured in appsettings.json under "Smtp".
- The project uses db.Database.Migrate() at startup; if you want to control migrations, create them locally as above.
