# Portfolio Builder

A personal portfolio website with a public project showcase and a protected admin dashboard for managing content.

**Stack:** ASP.NET Core Razor Pages (.NET 10), Dapper, SQL Server

## Features

**Public site**
- Portfolio sections (about, education, skills, research, coding)
- Projects loaded from the database
- Contact form that saves messages to SQL Server

**Admin dashboard** (`/Admin`)
- Login with Session state and an optional "remember me" cookie
- Projects: add, edit, delete
- Contact messages: inbox with unread count, read, delete
- Dashboard with project and unread message counts

## Security

- All database access uses parameterized Dapper queries (no string-built SQL)
- Passwords are stored as salted PBKDF2 hashes, never in plain text
- "Remember me" stores only a hash of a random token in the database; the cookie is HttpOnly
- Session cookie is HttpOnly and expires after 30 minutes idle
- Admin pages are protected by a folder-wide page filter
- Forms use anti-forgery tokens, and all output is HTML-encoded
- The first-admin setup page at `/Setup` only works while no admin exists

## Requirements

- .NET 10 SDK
- SQL Server Express (or any SQL Server instance)
- `sqlcmd`, or the VS Code SQL Server extension, to run the database scripts

## Setup

1. Clone the repo:

```bash
   git clone https://github.com/YOUR-USERNAME/portfolio_builder.git
   cd portfolio_builder
```

2. Create the database. Run the scripts in `database/` in this order:

```bash
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\01_create_database.sql
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\02_create_projects_table.sql
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\03_create_contactmessages_table.sql
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\04_create_adminusers_table.sql
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\05_seed_projects.sql 
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\06_add_remember_token.sql
```

   `05_seed_projects.sql` is optional. It inserts the author's sample projects.

3. Check the connection string in `src/PortfolioBuilder.Web/appsettings.json`. The default uses Windows authentication on `localhost\SQLEXPRESS`. Change the server name if yours differs.

4. Run the app:

```bash
   dotnet run --project src/PortfolioBuilder.Web
```

5. Open the URL printed in the terminal. On a fresh database, go to `/Setup` to create the first admin account, then sign in at `/Admin/Login`.

## Project structure

```
database/                     SQL scripts (run in order)
src/PortfolioBuilder.Web/
  Data/                       DbConnectionFactory and Dapper repositories
  Models/                     Entity and form input classes
  Pages/                      Public pages
  Pages/Admin/                Protected admin pages
  Services/                   Password hashing, auth service, page guard
  wwwroot/                    CSS, JavaScript, images, CV
```

## Git workflow

- `main` holds stable releases
- `develop` is the integration branch
- Work happens on `feature/*`, `fix/*` and `chore/*` branches, merged into `develop` by pull request