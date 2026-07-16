# 🩸 Blood Bank Network Management System

A robust and secure **ASP.NET Core MVC** web application designed to coordinate, manage, and track blood donor registries, blood stocks, donation histories, and urgent request pipelines across multiple blood bank locations. 

This project provides a centralized dashboard and secure operations panel for **Admin** and **Employee** roles to manage resources efficiently and save lives.

---

## 🚀 Key Features

*   **🔐 Advanced Cookie Authentication & Authorization**
    *   Role-based access control (RBAC) supporting `Admin` and `Employee` tiers.
    *   Global authentication locks configured in the MVC pipeline so that no internal views are exposed to anonymous guests.
*   **⏱️ Auto-Session Timeout & Tab-Close Detection**
    *   Custom global filter (`SessionTimeoutAttribute`) checking user activity on every controller hit.
    *   Idle session timeout securely managed and verified on the server side.
*   **📊 Interactive Management Dashboard**
    *   Aggregated key performance indicators (KPIs) showing total donors, active blood banks, total requests, and live inventory.
*   **👥 Donor Directory**
    *   Detailed profiling of donors including name, blood group, city location, contact details, total donation count, and last donation tracking.
*   **🏥 Blood Bank Registry**
    *   Directory of collaborating blood banks, operating hours, cities, and contact details.
*   **📦 Real-time Blood Stock Inventory**
    *   Inventories tracked by blood bank branch and blood group to ensure critical supplies are updated instantly upon donation or request fulfillment.
*   **📋 Blood Requests & Donation Records**
    *   Urgency-based request pipelines (Normal vs. High) mapping target hospitals, requested volume, and real-time status.
    *   Donation log connecting donors to specific blood bank locations with automated stock updates.

---

## 🛠️ Technology Stack

*   **Framework:** [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (ASP.NET Core MVC)
*   **ORM:** [Entity Framework Core 8.0.11](https://learn.microsoft.com/en-us/ef/core/)
*   **Database:** SQL Server / Azure SQL
*   **Frontend UI:** Razor Views (HTML5, Bootstrap, CSS3)
*   **Security & State:** ASP.NET Core Cookie Authentication & Distributed Memory Cache Sessions

---

## 📂 Project Structure

```text
BloodBankNetwork/
│
├── Controllers/                 # MVC Controllers handling requests and logic
│   ├── AccountController.cs     # Auth: Login, Register, Logout
│   ├── DashboardController.cs   # Metrics and administration portal
│   ├── DonorsController.cs      # Donor management
│   ├── BloodBanksController.cs  # Blood Bank locations registry
│   ├── BloodStocksController.cs # Live inventory tracking
│   ├── BloodRequestsController.cs# Patient/Hospital request management
│   ├── DonationRecordsController.cs # Historical donation entries
│   └── SessionTimeoutAttribute.cs# Custom filter for global tab/idle timeout check
│
├── Data/
│   └── ApplicationDbContext.cs  # Entity Framework Database Context with custom mappings
│
├── Models/                      # Schema representation and entity models
│   ├── User.cs                  # Registered User (Admin / Employee)
│   ├── Donor.cs                 # Donor profile information
│   ├── BloodBank.cs             # Blood Bank branch information
│   ├── BloodStock.cs            # Branch-specific stock tracker
│   ├── BloodRequest.cs          # Blood requests
│   └── DonationRecord.cs        # Record connecting donor, bank, and units
│
├── Views/                       # Razor layouts and view pages
├── Migrations/                  # EF Core database migrations
├── wwwroot/                     # Static files (CSS, JS, Images)
├── Program.cs                   # Application entry point, middleware pipeline, & services configuration
└── appsettings.json             # DB connection strings & configuration keys
```

---

## 💾 Database Schema

The database model is mapped using Fluent API to ensure exact naming conventions. Below is the Entity-Relationship logic:

*   **`User`**: Controls access. Fields include: `UserID`, `FullName`, `Email`, `Password`, `Role` (Admin/Employee).
*   **`Donor`**: Tracks individual donors. Fields include: `DonorID`, `Name`, `BloodGroup`, `Phone`, `City`, `LastDonationDate`, `TotalDonations`.
*   **`BloodBank`**: Registered branches. Fields include: `BankID`, `BankName`, `Address`, `City`, `ContactNo`, `OperatingHours`.
*   **`BloodStock`**: Inventory records. Fields include: `StockID`, `BankID` (FK to `BloodBank`), `BloodGroup`, `Units`, `LastUpdated`.
*   **`DonationRecord`**: Bridges donations. Fields include: `DonationID`, `DonorID` (FK to `Donor`), `BankID` (FK to `BloodBank`), `UnitsDonated`, `DonationDate`.
*   **`BloodRequest`**: Urgency tracking. Fields include: `RequestID`, `HospitalName`, `BloodGroup`, `UnitsNeeded`, `RequestDate`, `Urgency` (High/Normal), `Status`.

---

## ⚙️ Configuration & Setup

### 1. Prerequisites
Ensure you have the following installed on your machine:
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or Developer Edition)
*   [.NET EF Core Tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) (for running migrations):
    ```bash
    dotnet tool install --global dotnet-ef
    ```

### 2. Configure Database Connection
Open `appsettings.json` and configure your SQL Server connection string under `ConnectionStrings:DefaultConnection`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=BloodBankNetworkDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
```

### 3. Initialize the Database
Generate/update the database tables by executing EF migrations:
```bash
dotnet ef database update
```

### 4. Build and Run
Start the application from your command line:
```bash
dotnet run
```
Or use the watch command for hot-reloading:
```bash
dotnet watch run
```
Once started, navigate to `http://localhost:3000` (or the port specified in your console logs) in your web browser.

---

## 👨‍💻 Developed By

*   **GitHub Profile:** [MSaqibsohail](https://github.com/MSaqibsohail)
