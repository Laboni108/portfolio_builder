# Portfolio Builder
**Link** - https://laboni108.github.io/portfolio_builder/
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
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\07_create_skillcategories_table.sql
   sqlcmd -S localhost\SQLEXPRESS -E -No -i database\08_seed_skillcategories.sql
```

   `05_seed_projects.sql` is optional. It inserts the author's sample projects.

3. Check the connection string in `src/PortfolioBuilder.Web/appsettings.json`. The default uses Windows authentication on `localhost\SQLEXPRESS`. Change the server name if yours differs.

4. Run the app:

```bash
   dotnet run --project src/PortfolioBuilder.Web
```

5. Open the URL printed in the terminal. On a fresh database, go to `/Setup` to create the first admin account, then sign in at `/Admin/Login`.

## Run with Docker

The app can run in a container, with SQL Server in a second container on the same network.

1. Create a network and start SQL Server (choose your own strong `sa` password):

```bash
   docker network create portfolio-net
   docker run -d --name portfolio-db --network portfolio-net \
     -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<sa-password>" -e "MSSQL_PID=Express" \
     -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

2. Run the scripts in `database/` against it, in order (`01` to `06`, with `05` optional), for example:

```bash
   sqlcmd -S "tcp:localhost,1433" -U sa -P "<sa-password>" -No -i database/01_create_database.sql
```

3. Create a limited login for the app. It should not connect as `sa`:

```sql
   CREATE LOGIN portfolio_app WITH PASSWORD = '<app-password>', CHECK_POLICY = ON;
   USE PortfolioDb;
   CREATE USER portfolio_app FOR LOGIN portfolio_app;
   ALTER ROLE db_datareader ADD MEMBER portfolio_app;
   ALTER ROLE db_datawriter ADD MEMBER portfolio_app;
```

4. Build and run the app:

```bash
   docker build -t portfolio-builder .
   docker run -d --name portfolio-web --network portfolio-net -p 8080:8080 \
     -e "ConnectionStrings__PortfolioDb=Server=portfolio-db,1433;Database=PortfolioDb;User Id=portfolio_app;Password=<app-password>;TrustServerCertificate=True;" \
     -e "DataProtection__KeysPath=/keys" \
     -v portfolio-keys:/keys \
     portfolio-builder
```

5. Open `http://localhost:8080`, go to `/Setup` to create the first admin, and sign in at `/Admin/Login`.

**Configuration** is read from environment variables, which override `appsettings.json`:

| Variable | Purpose |
|---|---|
| `ConnectionStrings__PortfolioDb` | Database connection string |
| `DataProtection__KeysPath` | Folder for form-protection keys (mount a volume here so keys survive restarts) |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | Set to `true` in the image, so the app trusts a reverse proxy's HTTPS header. Only expose the container through a trusted proxy. |

Never commit real passwords or connection strings. 

## Publishing a static snapshot (GitHub Pages)

The full app needs a server and SQL Server, so the public link is a **snapshot**: the home page exported as static files and hosted on GitHub Pages at `https://laboni108.github.io/portfolio_builder/`. The admin dashboard is not part of the snapshot, and the contact form in the snapshot posts to Formspree.

**One-time setup**

1. Clone the repo a second time, next to this one, and create an empty `gh-pages` branch:

   ```powershell
   cd ..
   git clone https://github.com/Laboni108/portfolio_builder.git portfolio-pages
   cd portfolio-pages
   git checkout --orphan gh-pages
   git rm -rf .
   New-Item .nojekyll -ItemType File | Out-Null
   git add .nojekyll
   git commit -m "chore: start gh-pages"
   git push origin gh-pages
   ```

2. On GitHub, go to Settings > Pages and deploy from the `gh-pages` branch, `/ (root)` folder.

**Refreshing the live site** after changing content in the dashboard (stop the app first):

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\publish-snapshot.ps1
```

The script starts the app, saves the home page, fixes its links for static hosting, copies the css, js, images and files folders, and pushes to `gh-pages`. It refuses to publish if the page links to a file that was not copied.

If the `portfolio-pages` folder is lost, restore it with `git clone --branch gh-pages https://github.com/Laboni108/portfolio_builder.git portfolio-pages`.

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
