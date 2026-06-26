# Course Library

ASP.NET Core MVC application for a small university course library.

The application uses Entity Framework Core with the Code First approach and SQL Server. It was created to practice `DbContext`, entities, relationships, migrations, and data seeding.

## Features

The application supports:

* displaying a list of books at `/Books`
* displaying book details with borrowing history at `/Books/Details/{id}`
* creating a new book at `/Books/Create`
* displaying a list of authors at `/Authors`
* creating a new borrowing at `/Borrowings/Create`

## Domain model

The application contains three main entities.

### Author

* `Id`
* `FirstName`
* `LastName`
* collection of `Books`

### Book

* `Id`
* `Title`
* `Isbn`
* `PublishedYear`
* `AuthorId`
* related `Author`
* collection of `Borrowings`

### Borrowing

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

The context is injected into the service layer through dependency injection.

## Business logic

Business rules are placed in the service layer, not directly in controllers.

The service files are:

```text
Services/ILibraryService.cs
Services/LibraryService.cs
```

The service checks whether an author exists before adding a book and whether a book exists before adding a borrowing.

This keeps controllers focused on handling HTTP requests and returning views.

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

The last rule is also checked in the service layer before saving a borrowing to the database.

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

The seed data uses fixed `Id` values, which is required when using `HasData`.

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

The password is a local development password only. It is not a real production password.

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

## How to create or restore the database

To create or restore the database from the existing migration, run:

```bash
dotnet ef database update
```

This applies the migration from the `Migrations` folder and creates the database schema with seed data.

## How to recreate the database

To recreate the local database from scratch:

```bash
dotnet ef database drop
dotnet ef database update
```

Confirm the drop operation when prompted.

## Migration command used in this project

The command used to create the initial migration was:

```bash
dotnet ef migrations add InitialCreate
```

The migration was then applied with:

```bash
dotnet ef database update
```

The initial migration is included in the repository:

```text
Migrations/20260626212437_InitialCreate.cs
```

## Creating a new migration after model changes

If the model changes in the future, create a new migration with:

```bash
dotnet ef migrations add MigrationName
```

Then apply it with:

```bash
dotnet ef database update
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

## Questions for README

### What does ORM mean and what problem does EF Core solve?

ORM means Object-Relational Mapping.

It solves the problem of manually translating between database tables and C# objects. Without an ORM, the developer usually has to write more SQL and manually map query results to objects.

EF Core allows the application to work with database data through C# classes such as `Author`, `Book`, and `Borrowing`. It maps these classes to SQL Server tables and can translate LINQ queries into SQL queries.

### What is the role of DbContext?

`DbContext` is the main EF Core class responsible for database access.

It represents a session with the database and is used for querying, adding, updating, and deleting entities.

In this project, the custom context is:

```text
Data/LibraryDbContext.cs
```

It exposes `DbSet` properties for `Author`, `Book`, and `Borrowing`.

### How is DbSet different from a normal C# list?

A normal C# list stores objects only in memory.

A `DbSet<TEntity>` represents a database table for a specific entity type. It allows EF Core to build database queries and execute them against the database.

For example:

```csharp
public DbSet<Book> Books => Set<Book>();
```

represents the `Books` table.

Unlike a normal list, a `DbSet` can translate LINQ operations into SQL and load data from the database.

### Why should DbContext be Scoped in a web application?

In a web application, a scoped `DbContext` means one context instance is created for one HTTP request.

This is appropriate because it keeps database work consistent during the request and avoids sharing the same context instance between unrelated users or requests.

A singleton `DbContext` would be unsafe because it could be shared across many requests. A transient context could make it harder to keep one consistent unit of work during a request.

### What does an EF Core migration do?

An EF Core migration describes database schema changes generated from the C# model.

It allows the database schema to be created or updated in a controlled way.

In this project, the initial migration creates the `Authors`, `Books`, and `Borrowings` tables, relationships, indexes, and seed data.

### Why should seeding be idempotent?

Seeding should be idempotent because running the application or database setup more than once should not create duplicate data.

For example, the same authors and books should not be inserted again every time the application starts.

In this project, seed data is configured with `HasData` and fixed IDs. EF Core applies this seed data through migrations.

### When is Code First a good choice, and when should Database First be considered?

Code First is a good choice when the application is being created from scratch and the C# domain model should be the source of truth.

It is also useful when the developer wants to manage database changes with EF Core migrations.

Database First should be considered when the database already exists, especially in older systems or systems where the database schema is controlled outside the application.

This project uses Code First because the C# entity classes were created first and the SQL Server database was generated from the EF Core migration.
