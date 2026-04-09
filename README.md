# MiMealOrders

**ASP.NET WebForms school meal ordering system — built for public school district nutrition departments.**

> **Status:** Archived legacy system, preserved as a portfolio piece. Originally built at MiChoice Technology Systems (2005–2011) as part of a full product line deployed to real school districts.

---

## What It Is

MiMealOrders is a web-based meal ordering and management platform for K-12 school districts. It serves two user populations — **parents/students** (ordering meals, managing accounts) and **cafeteria administrators** (managing menus, vendors, campuses, and order data).

The system was built as part of a broader product line that included a Touch Screen POS application, a Central Office Data Management system, and a Free & Reduced meal application — all deployed to production school district environments.

---

## Architecture

The application follows a classic **3-tier ASP.NET WebForms** architecture:

```
┌─────────────────────────────────────────────┐
│           Presentation Layer                │
│  ASP.NET WebForms (.aspx + code-behind)     │
│  Master page for shared layout/navigation   │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│           Business / Data Access            │
│  DataLAyer.dll — separate class library     │
│  DataConnector wraps SqlClient calls        │
│  Parameterized queries via DataSelect()     │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│           Data Layer                        │
│  Microsoft SQL Server                       │
│  MiMealOrdering database                   │
│  Tables: AdminLoginInfo, UserLoginInfo,     │
│          Districts, Menus, MenuItems,       │
│          Vendors, Campuses, Orders          │
└─────────────────────────────────────────────┘
```

**Role-based access** is enforced at login — admin credentials route to `AdminHome.aspx`, user credentials to `UserMain.aspx`. District context is passed through `Session["DistrictID"]` across the request lifecycle.

---

## Modules

| Page | Role | Description |
|------|------|-------------|
| `Default.aspx` | All | Login page — routes to admin or user home based on role |
| `AdminHome.aspx` | Admin | Admin dashboard — entry point for all management functions |
| `AdminRegistration.aspx` | Admin | Register new administrator accounts |
| `UserRegistration.aspx` | User | Parent/student self-registration |
| `UserMain.aspx` | User | User home — view menus, place orders |
| `Main.aspx` | User | Main menu browsing and ordering interface |
| `MenuCreation.aspx` | Admin | Create and schedule menus by campus |
| `MenuAdminView.aspx` | Admin | View and manage existing menus |
| `MenuItemCreation.aspx` | Admin | Add individual menu items (name, price, description) |
| `VendorCreation.aspx` | Admin | Register food vendors supplying the menus |
| `CampusCreation.aspx` | Admin | Add school campuses (district → campus hierarchy) |
| `Master.Master` | All | Shared master page — navigation, header, session context |

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET WebForms, .NET Framework 4.0 |
| Language | C# |
| Data Access | Custom `DataLAyer` class library (SqlClient wrapper) |
| Database | Microsoft SQL Server |
| Auth | Session-based, role-checked at login |
| UI | HTML, CSS, server-side controls |
| Hosting | IIS (Windows Server) |
| Build | Visual Studio 2012, MSBuild, WebDeploy |

---

## Key Design Decisions

**Separate DataLayer DLL** — Data access is isolated into a `DataLAyer` class library (compiled separately), allowing the data connector to be reused across multiple apps in the MiChoice product suite (POS, Central Office, Free/Reduced). The `DataConnector` class wraps `SqlDataAdapter` and `DataTable` operations, reducing boilerplate in each page's code-behind.

**District-scoped session** — All queries are filtered by `Session["DistrictID"]`, which is set at login and scoped to the authenticated session. This allows the same application instance to serve multiple school districts without data bleed between accounts.

**Master page for shared layout** — `Master.Master` provides consistent navigation, session validation, and header branding across all pages. Code-behind pages reference it via `MasterType` directives for strongly-typed master page access.

**WebForms code-behind pattern** — Each `.aspx` page has a `.aspx.cs` code-behind and a `.aspx.designer.cs` auto-generated control reference file. This separation of markup and logic was the standard ASP.NET pattern of the era, predating MVC.

---

## Historical Context

This system was developed as part of Scott Murphy's role as CTO / Sr. Software Engineer at **MiChoice Technology Systems** (Houston, TX — 2005 to 2011), where he built the company's entire software product line for public school district nutrition departments.

The MiChoice suite was deployed to real school districts for production use. MiMealOrders was one component of a larger platform that included:
- Touch Screen POS Application (VB.NET, MS SQL)
- Central Office Data Management System
- Free & Reduced Meal Application Processing
- Parent Portal for meal history and payments
- Time Clock and Inventory Management

This repository represents the meal ordering web portal component — preserved as a demonstration of production-grade enterprise ASP.NET WebForms development.

---

## Running Locally

> This is a legacy archived project. These instructions are provided for reference.

**Prerequisites:**
- Visual Studio 2012+ (or compatible)
- .NET Framework 4.0
- SQL Server (LocalDB or Express)
- IIS or IIS Express

**Setup:**
1. Clone the repo and open `MiMealOrders.sln` in Visual Studio
2. Create a SQL Server database named `MiMealOrdering`
3. Run the schema scripts to create the required tables (not included — schema reconstructible from code-behind query patterns)
4. Update the connection string in `Default.aspx.cs` and any other code-behind files referencing `Server=localhost;Database=MiMealOrdering;Trusted_Connection=True;`
5. Build and run via IIS Express

---

## Author

**Scott Roy Murphy** — [scottroymurphy.com](https://scottroymurphy.com) | [LinkedIn](https://www.linkedin.com/in/scottroymurphy/)

Senior AI Engineer & Prompt Architect with 28+ years of enterprise software development, from ASP.NET WebForms and WPF to production LLM orchestration, RAG pipelines, and agentic AI systems.

---

*Part of the [srmbsrg portfolio](https://github.com/srmbsrg/portfolio) — archived legacy systems demonstrating full-stack enterprise development history.*
