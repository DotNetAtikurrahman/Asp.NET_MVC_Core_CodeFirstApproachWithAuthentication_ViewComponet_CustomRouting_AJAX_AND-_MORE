# ASP.NET Core MVC — Code-First with Role-Based Authentication

A full-featured **ASP.NET Core MVC** web application built on **.NET 10** that demonstrates a Code-First Entity Framework workflow, ASP.NET Core Identity with role-based authorization, customer & product management, file uploads, View Components, and both conventional and attribute routing.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Data Models](#data-models)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Setup](#database-setup)
  - [Running the App](#running-the-app)
- [Role-Based Authorization](#role-based-authorization)
- [Routing](#routing)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

This project serves as a practical reference implementation of an **ASP.NET Core MVC Code-First** application. It covers:

- Defining the database schema entirely in C# (Code-First)
- Extending ASP.NET Core Identity for custom user properties
- Protecting routes with role-based `[Authorize]` attributes
- Handling file uploads and persisting images to `wwwroot`
- Using **View Components** for reusable, self-contained UI blocks
- Responding to AJAX-driven forms with Partial Views

---

## Features

| Area | Details |
|---|---|
| **Authentication** | Register / Login via ASP.NET Core Identity Razor Pages |
| **Role Management** | Create roles and assign them to users (`SuperAdmin`, `Admin`) |
| **Customer CRUD** | Create, list, edit, and delete customers with photo upload |
| **Product CRUD** | Full product management with details view |
| **Transaction Details** | Many-to-many link between customers and products |
| **Image Handling** | Upload and serve profile pictures from `wwwroot/Images` |
| **View Components** | `ProductMenuViewComponent` for a sidebar product menu |
| **Partial Views** | AJAX-friendly `_success` / `_error` feedback partials |
| **Custom Routing** | Both conventional and attribute-based custom routes |
| **Migrations** | EF Core migration history tracked in `Data/Migrations` |

---

## Tech Stack

- **Runtime:** .NET 10
- **Framework:** ASP.NET Core MVC
- **ORM:** Entity Framework Core 10 (Code-First)
- **Database:** Microsoft SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Auth:** ASP.NET Core Identity (`IdentityDbContext`, Razor Pages UI)
- **Front-end:** Razor Views, Bootstrap (via LibMan), jQuery Unobtrusive AJAX
- **IDE:** Visual Studio 2022+ (`.slnx` solution format)

---

## Project Structure

```
ASP.NET_CORE_CodeFirst/
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/          # Scaffolded Login & Register pages
├── Controllers/
│   ├── CustomersController.cs    # Customer CRUD + image upload
│   ├── ProductsController.cs     # Product CRUD
│   ├── RoleController.cs         # Role creation & assignment
│   └── HomeController.cs
├── Data/
│   ├── ApplicationDbContext.cs   # EF Core DbContext
│   ├── ApplicationUser.cs        # Extended IdentityUser
│   └── Migrations/               # EF Core migration files
├── Models/
│   ├── DataModel.cs              # Product, Customer, TransactionDetail
│   ├── ErrorViewModel.cs
│   └── ViewModel/
│       └── ClientVM.cs           # Customer form ViewModel (w/ IFormFile)
├── ViewComponents/
│   └── ProductMenuViewComponent.cs
├── Views/
│   ├── Customers/                # Index, Create, Edit, Delete + partials
│   ├── Products/                 # Index, Create, Edit, Delete, Details
│   ├── Role/                     # Index, AssignRole
│   ├── Home/
│   └── Shared/
│       ├── Components/ProductMenu/Default.cshtml
│       ├── _Layout.cshtml
│       ├── _LoginPartial.cshtml
│       ├── _success.cshtml       # AJAX success partial
│       └── _error.cshtml         # AJAX error partial
├── wwwroot/
│   ├── css/site.css
│   └── Images/                   # Uploaded customer photos land here
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

---

## Data Models

### `Product`
| Column | Type | Notes |
|---|---|---|
| `ProductId` | `int` | Primary key |
| `ProductName` | `string?` | Display name |

### `Customer`
| Column | Type | Notes |
|---|---|---|
| `CustomerId` | `int` | Primary key |
| `CustomerName` | `string?` | Required |
| `Picture` | `string?` | Relative path to uploaded image |
| `Address` | `string?` | |
| `Phone` | `string?` | |
| `PurchaseDate` | `DateTime` | Stored as `date`, required |
| `TotalBill` | `double` | |
| `IsPaid` | `bool` | |

### `TransactionDetail` *(join table)*
| Column | Type | Notes |
|---|---|---|
| `TransactionDetailId` | `int` | Primary key |
| `CustomerId` | `int` | FK → Customer |
| `ProductId` | `int` | FK → Product |

### `ApplicationUser` *(extends `IdentityUser`)*
Custom fields can be added to `ApplicationUser.cs` and will be picked up by Identity automatically.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Developer, or Express edition works)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) v17.10+ **or** VS Code with the C# Dev Kit extension
- EF Core CLI tools:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/<your-username>/<repo-name>.git
cd <repo-name>

# 2. Restore NuGet packages
dotnet restore
```

### Database Setup

1. Open `appsettings.json` and update the connection string to point to your SQL Server instance:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=YourDbName;Trusted_Connection=True;TrustServerCertificate=True"
   }
   ```

2. Apply all existing migrations to create the database schema:

   ```bash
   dotnet ef database update
   ```

   > To add a new migration after changing a model:
   > ```bash
   > dotnet ef migrations add <MigrationName>
   > dotnet ef database update
   > ```

### Running the App

```bash
dotnet run --project ASP.NET_CORE_CodeFirst
```

Or press **F5** in Visual Studio. The app will launch at `https://localhost:{port}` as defined in `Properties/launchSettings.json`.

---

## Role-Based Authorization

The application uses ASP.NET Core Identity with two primary roles:

| Role | Permissions |
|---|---|
| `SuperAdmin` | Full access — create, **edit**, and **delete** customers; manage roles |
| `Admin` | Can create customers; read-only on edit/delete |
| *(Authenticated)* | Can view listings |
| *(Anonymous)* | Redirected to Login |

Roles are enforced via `[Authorize(Roles = "...")]` attributes on controller actions:

```csharp
[Authorize(Roles = "SuperAdmin,Admin")]
public IActionResult Create() { ... }

[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> Edit(int? id) { ... }

[Authorize(Roles = "SuperAdmin")]
public async Task<IActionResult> Delete(int? id) { ... }
```

To seed roles and assign them to users, navigate to `/Role` after registering your first account.

---

## Routing

The app uses both **conventional** and **attribute** routing.

### Conventional (default)
```
/{controller=Home}/{action=Index}/{id?}
```

### Custom Conventional Route
A dedicated route for the customer create form:
```
/add/newcustomer/mydatabase  →  Customers/Create
```
Configured in `Program.cs`:
```csharp
app.MapControllerRoute(
    name: "amercustomroute",
    pattern: "add/newcustomer/mydatabase",
    defaults: new { controller = "Customers", action = "Create" }
);
```

### Attribute Route (SuperAdmin Edit)
The edit action is locked behind a custom URL to emphasize restricted access:
```
[Route("ohbrotheronlysuperadmin/canedit")]
public async Task<IActionResult> Edit(int? id) { ... }
```

---



Please follow the existing code style and include XML doc comments on public members.

---

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.
