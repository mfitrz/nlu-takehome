# NLU Takehome — Building Violations API

A .NET 10 ASP.NET Core Web API that surfaces building violation and scofflaw data from the City of Chicago, backed by a PostgreSQL database hosted on Supabase.

---

## Project Structure

```
NLU Takehome/
├── NLUTakehome/                  # Main API project
│   ├── Endpoints/                # Minimal API route handlers
│   ├── Models/                   # Request, response, and DB row models
│   ├── Repositories/             # Data access layer (raw Npgsql, no ORM)
│   ├── Validators/               # Input validation helpers
│   └── appsettings.json          # Connection string configuration
├── NLUTakehome.Ingestion/        # Console app for CSV ingestion
│   ├── Parsers/                  # CSV → typed model parsers
│   ├── Repositories/             # Batch insert repositories
│   ├── Scripts/                  # SQL table creation scripts
│   │   └── create_tables.sql
│   └── datasets/                 # Source CSV files (bundled in output)
│       ├── Building_Code_Scofflaw_List_20250807.csv
│       └── Building_Violations_20250815.csv
└── NLUTakehome.Tests/            # xUnit test project
    ├── Endpoints/                # Integration tests (WebApplicationFactory)
    ├── Parsers/                  # Unit tests for CSV parsers
    └── Validators/               # Unit tests for validators
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A PostgreSQL database (the project uses Supabase)

---

## Configuration

Connection credentials are loaded from a `.env` file at the solution root. Copy the example and fill in your values:

```powershell
copy .env.example .env
```

`.env` contents:
```
ConnectionStrings__DefaultConnection=Host=<host>;Database=<database>;Username=<username>;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
```

---

## Database Setup

### 1. Create Tables

Run the SQL script against your PostgreSQL database:

```
NLUTakehome.Ingestion/Scripts/create_tables.sql
```

This creates three tables:

| Table        | Purpose                                          |
|--------------|--------------------------------------------------|
| `violations` | Building violations (2024-01-01 onward)          |
| `scofflaws`  | Addresses on the Building Code Scofflaw List     |
| `comments`   | User-submitted comments tied to addresses        |

All address columns are stored lowercase and trimmed to enable reliable JOINs across tables.

### 2. Run the Ingestion Script

The ingestion console app reads two CSVs and batch-inserts them into the database. CSV files are already included in the project under `NLUTakehome.Ingestion/datasets/`.

```powershell
cd NLUTakehome.Ingestion
dotnet run
```

The two datasets are ingested in parallel. Duplicate records are silently ignored (`ON CONFLICT DO NOTHING`), so re-running is safe.

---

## Running the API

```powershell
cd NLUTakehome
dotnet run
```

The API starts on:
- HTTP: `http://localhost:5174`
- HTTPS: `https://localhost:7205`

The OpenAPI spec is available at `/openapi/v1.json` in development.

---

## Running the Tests

```powershell
dotnet test NLUTakehome.Tests
```

All 29 tests run without a live database. Endpoint tests use `WebApplicationFactory` with stub repositories injected via `ConfigureTestServices`.

---

## Design Notes

- **No ORM.** All database access uses raw `Npgsql` commands with parameterized queries.
- **Address normalization.** All addresses are lowercased and trimmed at parse time (ingestion) and at request time (API), so `JOIN`s between `violations` and `scofflaws` are reliable.
- **Scofflaw flag.** The `GET /property/{address}` endpoint determines scofflaw status via a `LEFT JOIN` to the `scofflaws` table — no separate query needed.
- **Batch ingestion.** Records are inserted in batches of 1,000 using parameterized multi-row `INSERT ... ON CONFLICT DO NOTHING` statements for both datasets.
- **POST sanitization.** Comment inputs are validated (non-empty author and comment text) and trimmed before insertion. All values are passed as parameters — no string interpolation in SQL.
- **Indexes.** `violations(address)`, `violations(violation_date)`, `scofflaws(address)`, and `comments(address)` are indexed for query performance.

---

## API Endpoints

See [API_DOCS.md](API_DOCS.md) for full JSON request/response schema documentation.
