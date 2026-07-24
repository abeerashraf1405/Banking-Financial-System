# 🏦 Prime Bank - Banking & Financial Management System

A full-featured desktop banking management system built with **C# Windows Forms** and an **Oracle SQL/PL-SQL** backend. It manages customers, accounts, transactions, loans, cards, employees, and branches through a clean, sidebar-navigated dashboard UI.

## Overview

Prime Bank is a console-free, GUI-driven banking application that models a real-world bank's core operations. It's backed by a normalized Oracle database with **18 tables**, **5 views**, and **9 stored procedures** that enforce business logic (balance checks, cascading inserts, audit logging) directly at the database layer, with the C# frontend calling into them.

## ✨ Features

- 📊 **Dashboard** : live stats (total customers, active balance, active loans, active cards) plus the 10 most recent transactions
- 👤 **Customers** : add and browse customer records
- 💳 **Accounts** : open Checking/Saving/CD accounts, change account status (Active/Inactive/Closed)
- 🔁 **Transactions** : Deposit, Withdraw, and Transfer funds between accounts
- 💰 **Loans** : issue Mortgage/Car/Personal loans with type-specific detail fields, record loan payments against the current balance
- 🪪 **Cards** : issue Credit or Debit cards with type-specific fields (credit limit & minimum payment, or PIN)
- 🧑‍💼 **Employees** : view employee/branch listing, update salaries
- 🏢 **Branches** : browse bank branch listing
- 🎨 Consistent navy-and-gold themed UI shared across every screen via a central `UiTheme` helper
- 🧩 Reusable dialog-building helpers so every "Add" form follows the same layout pattern

## 🗄️ Database Design

The schema is modeled with an Enhanced Entity-Relationship Diagram (`EERD.drawio`, open with [diagrams.net](https://app.diagrams.net/)) and implemented in `project_db.sql`.

### Tables

| Domain | Tables |
|---|---|
| **Organization** | `Bank_Branch`, `Employee` |
| **Customers & Accounts** | `Customer`, `Account`, `Checking_Account`, `Saving_Account`, `Certificate_of_Deposit` |
| **Transactions** | `Bank_Transaction`, `Transfer`, `Deposit`, `Withdrawal` |
| **Loans** | `Loan`, `Mortgage`, `Car_Loan`, `Personal_Loan`, `Loan_Audit_Log` |
| **Cards** | `Card`, `Credit_Card`, `Debit_Card` |

Account, loan, and card types use a **supertype/subtype (table-per-type)** design — e.g. every `Account` row has a matching detail row in exactly one of `Checking_Account`, `Saving_Account`, or `Certificate_of_Deposit`.

### Views

- `vw_Customer_Accounts` : accounts joined with owning customer
- `vw_Employee_Branch` : employees joined with their branch
- `vw_Active_Loans` : currently active loans
- `vw_Transaction_History` : unified transaction feed across deposits, withdrawals, and transfers
- `vw_Card_Holders` : cards joined with their holding customer

### Stored Procedures

| Procedure | Purpose |
|---|---|
| `sp_AddCustomer` | Insert a new customer |
| `sp_AddAccount` | Open a new account |
| `sp_Deposit` | Process a deposit and log the transaction |
| `sp_Withdrawal` | Process a withdrawal (with PIN check) |
| `sp_Transfer` | Move funds between two accounts |
| `sp_AddLoan` | Create a new loan |
| `sp_UpdateAccountStatus` | Change an account's status |
| `sp_UpdateSalary` | Update an employee's salary |
| `sp_DeleteAccount` | Remove an account |
| `sp_UpdateLoanBalance` | Apply a payment to a loan's current balance |

Procedures handle their own error control internally (`WHEN OTHERS THEN ROLLBACK`), so the C# layer verifies success by re-querying (`DbHelper.RowExists`) rather than relying solely on thrown exceptions.

## 🧰 Tech Stack

- **C# / .NET** : Windows Forms desktop application
- **Oracle Database** : data storage, business logic (PL/SQL procedures & views)
- **Oracle.ManagedDataAccess.Client** : ADO.NET driver for Oracle connectivity
- **draw.io / diagrams.net** : EERD schema diagram

## 📁 Project Structure

```
bank-management-system/
├── Program.cs                    # Application entry point
├── MainForm.cs                   # Main shell: sidebar nav, dashboard, list pages
├── DbHelper.cs                   # Centralized DB access (queries, procedures, connection)
├── UiTheme.cs                    # Shared colors, fonts, and styled control factories
├── DialogHelper.cs               # Reusable form-field layout helpers for dialogs
├── AddCustomerForm.cs            # Add Customer dialog
├── AddAccountForm.cs             # Open New Account dialog
├── AddLoanForm.cs                # Add New Loan dialog (type-specific fields)
├── AddCardForm.cs                # Issue New Card dialog (Credit/Debit specific fields)
├── DepositForm.cs                # Deposit dialog
├── WithdrawalForm.cs             # Withdrawal dialog
├── TransferForm.cs               # Transfer dialog
├── LoanPaymentForm.cs            # Record Loan Payment dialog
├── UpdateAccountStatusForm.cs    # Change Account Status dialog
├── UpdateSalaryForm.cs           # Update Employee Salary dialog
├── project_db.sql                # Full Oracle schema: tables, views, procedures
├── EERD.drawio                   # Enhanced ER Diagram (draw.io source file)
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- Windows with **.NET Framework / .NET Desktop Runtime** (Windows Forms support)
- **Oracle Database** (e.g. Oracle XE) running locally or accessible on the network
- **Oracle.ManagedDataAccess** NuGet package installed in the project
- Visual Studio (recommended) or another C# IDE with Windows Forms designer support

### 1. Set Up the Database

```sql
-- Connect to your Oracle instance as the target schema user, then run:
@project_db.sql
```

This creates all tables, views, and stored procedures.

### 2. Configure the Connection String

Update the connection string in `DbHelper.cs` with your own Oracle credentials:

```csharp
private static string connStr =
    "User Id=YOUR_USER;Password=YOUR_PASSWORD;Data Source=localhost:1521/xe;";
```

> ⚠️ **Security note:** the current code has credentials hardcoded directly in `DbHelper.cs`. Before pushing to GitHub, replace these with environment variables, a config file excluded via `.gitignore`, or a secrets manager and rotate the exposed password.

### 3. Build & Run

Open the solution in Visual Studio, restore the `Oracle.ManagedDataAccess` NuGet package, and run the project (`Program.cs` is the entry point). The app opens maximized with the sidebar dashboard.

## 🖥️ Usage

| Screen | What you can do |
|---|---|
| **Dashboard** | View live totals and the 10 most recent transactions |
| **Customers** | Add a new customer |
| **Accounts** | Open a new account, change an account's status |
| **Transactions** | Deposit, withdraw, or transfer funds |
| **Loans** | Add a loan, record a payment against a selected loan |
| **Cards** | Issue a new credit or debit card |
| **Employees** | Update a selected employee's salary |
| **Branches** | Browse all branches |


## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 👤 Author

**Abeer Ashraf**
Computer Science Undergraduate @ FAST-NUCES
