# CalculatorApp

A full-stack **.NET Blazor** application that allows users to perform arithmetic calculations and store results using **PostgreSQL**. The app consists of a **Blazor UI**, a **.NET Web API**, and **Entity Framework Core** for database operations.

## Features

- Perform basic arithmetic operations (**Add, Subtract, Multiply, Divide**)
- Store a computed number for later use
- Reset stored numbers
- PostgreSQL database integration with **EF Core**
- Full API support with **Swagger UI**
- Source-controlled with **GitHub**

## Technologies Used

- **Frontend**: Blazor WebAssembly
- **Backend**: .NET 8 Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Source Control**: Git & GitHub

## Getting Started

### Prerequisites

- [.NET SDK 7+](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [pgAdmin](https://www.pgadmin.org/) (Optional for database management)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository:**

   ```sh
   git clone https://github.com/abunawO/CalculatorApp.git
   cd CalculatorApp
   ```

2. **Set up PostgreSQL database:**

   - Create a new database called `CalculatorDB`
   - Update the connection string in `appsettings.json` in the `CalculatorAPI` project:
     ```json
     "ConnectionStrings": {
       "PostgresConnection": "Host=localhost;Port=5432;Database=CalculatorDB;Username=postgres;Password=somepassword"
     }
     ```

3. **Apply database migrations:**

   ```sh
   cd CalculatorAPI
   dotnet ef database update
   ```

4. **Run the backend API:**

   ```sh
   dotnet run
   ```

5. **Run the frontend (Blazor UI):**
   ```sh
   cd CalculatorUI
   dotnet run
   ```

### API Endpoints

| Method   | Endpoint                              | Description                     |
| -------- | ------------------------------------- | ------------------------------- |
| `POST`   | `/api/calculator/calculate`           | Perform arithmetic calculations |
| `POST`   | `/api/calculator/store-number`        | Store a computed number         |
| `GET`    | `/api/calculator/stored-number`       | Retrieve the stored number      |
| `DELETE` | `/api/calculator/reset-stored-number` | Reset the stored number         |

### Using Swagger UI

To test API endpoints, navigate to:

```
http://localhost:5101/swagger
```

### Viewing Data in PostgreSQL

To check stored numbers manually:

```sql
SELECT * FROM "StoredNumbers";
```
