# 📚 Libro — Online Library Management System

Libro is a full-featured library catalog and lending platform built with **ASP.NET Core MVC**. It lets librarians manage a book catalog and lending operations, while members browse, borrow, reserve, and review books — all through a role-based, responsive web application.

---

## ✨ Features

### Catalog & Content
- Full CRUD for **Books**, **Authors**, and **Categories** (many-to-many book–category relationship)
- Extended author profiles: biography, date of birth, nationality, website link, photo
- Search and pagination on the book catalog
- Cover images and author photos via URL, with live preview on the edit forms

### Membership & Identity
- Custom-built authentication with **ASP.NET Core Identity** (no scaffolded UI — hand-built Login/Register/Logout)
- Two roles with distinct permissions: **Librarian** and **Member**
- Login lockout after repeated failed attempts (brute-force protection)
- Member account suspension by librarians
- Editable member/librarian profile with photo

### Borrowing & Lending
- Borrow flow with member-selected loan duration (1–7 days) and live cost calculation
- Automatic per-copy availability tracking (`TotalCopies` / `AvailableCopies`)
- Return flow with automatic **late fee calculation**
- Loan **renewal** (one-time extension, blocked once overdue)
- Fine payment tracking (`Unpaid` / `Paid`)
- A configurable cap on simultaneous active loans per member

### Waitlists & Personalization
- **Reservation queue** (FIFO) for books that are fully checked out, with a 48-hour claim window before the reservation expires and rolls over to the next member
- **Favorites / wishlist** for members
- **Recently viewed books**, tracked via session — no account required
- **Ratings & reviews**, restricted to members who have actually borrowed and returned the book

### Librarian Tools
- Dashboard with live stats: total books/members, overdue loans, unpaid fines, top-borrowed titles, newest members
- CSV export of the full loan history
- Dedicated views for active loans, unpaid fines, and member management

### Engineering
- **Repository pattern** across all data access — controllers depend on interfaces, not `DbContext`, directly
- Fully **async/await** data access with Entity Framework Core
- **FluentValidation** for all forms (including cross-field and uniqueness rules, e.g. duplicate ISBN)
- **Serilog** structured logging to console and rolling daily files
- **xUnit** test suite covering core business logic (fine calculation, availability rules)
- TempData-based flash messaging, consistent across every write operation

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET) |
| Data access | Entity Framework Core + SQL Server |
| Auth | ASP.NET Core Identity (custom UI) |
| Validation | FluentValidation |
| Logging | Serilog |
| Testing | xUnit |
| Frontend | Razor Views, Bootstrap, custom CSS design system, vanilla JS |

---

## 🏗️ Architecture

```
Controllers  →  Repositories (interfaces)  →  EF Core DbContext  →  SQL Server
     ↓
  Services (FineCalculator, etc.)
     ↓
  ViewModels  →  Razor Views
```

- **Controllers** stay thin: they orchestrate repositories and services, they don't talk to `DbContext` directly.
- **Repositories** (`IBookRepository`, `ILoanRepository`, `IMemberRepository`, …) encapsulate all EF Core queries behind interfaces, registered via dependency injection.
- **Services** hold pure business logic (e.g. `FineCalculator`) that's easy to unit test in isolation.
- **ViewModels** shape data specifically for each view instead of leaking domain models into forms that don't need them.

---

## 🗄️ Data Model (core entities)

`Book` · `Author` · `Category` · `Member` · `Librarian` · `Loan` · `Reservation` · `Favorite` · `Review`

Key relationships:
- `Author` 1—* `Book`
- `Book` *—* `Category`
- `Member` 1—* `Loan` *—1 `Book` (the loan itself carries borrow/due/return dates and fine)
- `Member` 1—* `Reservation`, `Favorite`, `Review`

---

## 🚀 Getting Started

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Apply migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the app:
   ```bash
   dotnet run
   ```
5. Register an account and choose a role (Librarian or Member) to explore the corresponding dashboard.

---

## 🧪 Running Tests

```bash
dotnet test
```

---

## 📌 Notes

This project was built incrementally as a learning exercise in ASP.NET Core MVC — starting from core CRUD and Identity, and layering in relational modeling (one-to-many and many-to-many), business rules, repository-based architecture, and a custom visual identity.
