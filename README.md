# Course Library

ASP.NET Core MVC application for a small university course library.
The application uses Entity Framework Core with the Code First approach and SQL Server.

## Features

The application supports:

* displaying a list of books at `/Books`
* displaying book details with borrowing history at `/Books/Details/{id}`
* creating a new book at `/Books/Create`
* displaying a list of authors at `/Authors`
* creating a new borrowing at `/Borrowings/Create`

## Domain model

The application contains three main entities:

* `Author`

  * `Id`
  * `FirstName`
  * `LastName`
  * collection of `Books`

* `Book`

  * `Id`
  * `Title`
  * `Isbn`
  * `PublishedYear`
  * `AuthorId`
  * related `Author`
  * collection of `Borrowings`

* `Borrowing`

  * `Id`
  * `BookId`
  * related `Book`
  * `BorrowerName`
  * `BorrowedAt`
  * `ReturnedAt`

## Relationships

The application uses the following relationships:

* one `Author` can have many `Books`
* one `Book` belongs to one `Author`
* one `Book` can have many `Borrowings`
* one `Borrowing` belongs to one `Book`

The relationship configuration is defined with Fluent API in:

* `Data/Configurations/AuthorConfiguration.cs`
* `Data/Configurations/BookConfiguration.cs`
* `Data/Configurations/BorrowingConfiguration.cs`

The configuration classes are loaded in `Data/LibraryDbContext.cs` with:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
```

## DbContext

The custom EF Core context is located in:

```text
Data/LibraryDbContext.cs
```

It contains the following `DbSet` properties:

```csharp
public DbSet<Author> Authors => Set<Author>();
public DbSet<Book> Books => Set<Book>();
public DbSet<Borrowing> Borrowings => Set<Borrowing>();
```

The context is registered in `Program.cs` using dependency injection:

```csharp
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Controllers do not create the context manually with `new`.
The context is injected through services.

## Business logic

Business rules are placed in the service layer, not directly in controllers.

The service files are:

```text
Services/ILibraryService.cs
Services/LibraryService.cs
```

The service checks, for example, whether an author exists before adding a book and whether a book exists before adding a borrowing.

## Validation

Form validation is implemented with ViewModels and data annotations.

The ViewModels are located in:

```text
ViewModels/BookCreateViewModel.cs
ViewModels/BorrowingCreateViewModel.cs
```

Implemented validation rules:

* book title is required
* ISBN is required
* published year must be greater than 1900
* author must be selected when creating a book
* borrower name is required
* book must be selected when creating a borrowing
* borrowing cannot be added for a book that does not exist

## Seed data

Seed data is configured with `HasData`.

The seed configuration is located in:

```text
Data/Configurations/AuthorConfiguration.cs
Data/Configurations/BookConfiguration.cs
```

The database is seeded with:

* 3 authors
* 5 books

## Connection string

The connection string is stored in:

```text
appsettings.json
```

Current local development connection string:

```json
"DefaultConnection": "Server=localhost,1433;Database=CourseLibraryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
```

The connection string is not hardcoded in controllers or services.

## Requirements

To run the project locally, use:

* .NET SDK
* SQL Server
* EF Core CLI tool `dotnet-ef`

For local development, SQL Server can be started with Docker:

```bash
docker run -e 'ACCEPT_EULA=Y' \
  -e 'MSSQL_SA_PASSWORD=YourStrong!Passw0rd' \
  -p 1433:1433 \
  --name course-library-sql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

If the container already exists, start it with:

```bash
docker start course-library-sql
```

## Installing EF Core CLI

If `dotnet ef` is not available, install it with:

```bash
dotnet tool install --global dotnet-ef --version 10.0.9
```

If needed, add .NET tools to the current terminal session:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

Check the installed version:

```bash
dotnet ef --version
```

## Creating the database

To create the database from the existing migration, run:

```bash
dotnet ef database update
```

This applies the migration from the `Migrations` folder and creates the database schema with seed data.

## Recreating the database

To recreate the local database from scratch:

```bash
dotnet ef database drop
dotnet ef database update
```

Confirm the drop operation when prompted.

## Creating a new migration

If the model changes, create a new migration with:

```bash
dotnet ef migrations add MigrationName
```

Then apply it with:

```bash
dotnet ef database update
```

The initial migration is already included in the repository:

```text
Migrations/20260626212437_InitialCreate.cs
```

## Running the application

Start SQL Server first:

```bash
docker start course-library-sql
```

Then run the application:

```bash
dotnet run
```

Or run it on a specific local URL:

```bash
dotnet run --urls "http://localhost:5288"
```

Open the application in the browser:

```text
http://localhost:5288
```

The default route opens the books list.

## Main routes

```text
/Books
/Books/Create
/Books/Details/{id}
/Authors
/Borrowings/Create
```

## Short lecture questions

### What is an ORM?

ORM means Object-Relational Mapping.
It allows the application to work with database data through C# classes instead of manually writing SQL for every operation.

In this project, Entity Framework Core maps the C# classes `Author`, `Book`, and `Borrowing` to SQL Server tables.

### What is DbContext?

`DbContext` is the main EF Core class responsible for database access.
It represents a session with the database and is used for querying, adding, updating, and deleting entities.

In this project, the custom context is:

```text
Data/LibraryDbContext.cs
```

### What is a DbSet?

A `DbSet<TEntity>` represents a database table for a specific entity type.

For example:

```csharp
public DbSet<Book> Books => Set<Book>();
```

represents the `Books` table.

### How is DbSet different from a normal list?

A normal C# list stores objects only in memory.
A `DbSet` represents database data and allows EF Core to translate LINQ queries into SQL queries executed against the database.

### Why should DbContext be registered as scoped?

In a web application, a scoped `DbContext` means one context instance is created for one HTTP request.
This is appropriate because it keeps database work consistent during the request and avoids sharing the same context between unrelated users or requests.

### What is a migration?

A migration is a file generated by EF Core that describes database schema changes.
It allows the database schema to be created or updated based on the C# model.

This project contains the initial migration in the `Migrations` folder.

### What does idempotent seeding mean?

Idempotent seeding means that running the seed operation multiple times should not create duplicate data.

In this project, seed data is configured with `HasData` and fixed IDs. EF Core applies this seed data through migrations.

### What is Code First?

Code First is an approach where the developer first creates C# entity classes and configuration, and EF Core generates the database schema from that model.

This project uses Code First because the database is created from the C# models and EF Core migration.

### Code First vs Database First

In Code First, the C# model is the source of truth and the database is generated from code.

In Database First, the database already exists first, and C# classes are generated or scaffolded from the database schema.

This project uses Code First.
