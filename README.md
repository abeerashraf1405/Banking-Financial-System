# 🏦 Banking & Financial System

A full-scale relational database system for a banking application built with **Oracle SQL and PL/SQL**, featuring a normalized 20-table schema covering branch management, customer accounts, transactions, loan processing, and card services — with stored procedures, triggers, and views to automate business logic and enforce data integrity. Paired with a **C# Windows Forms GUI** for end-to-end usability.

> **Course:** Database Systems (CL-2005) — FAST-NUCES, Chiniot-Faisalabad Campus  
> **Session:** Spring 2026  
> **Authors:** Abeer Ashraf (24F-0762) · Wania Khurram (24F-0751)

---

## 📁 Repository Structure

```
├── project db.sql              # Full DDL + DML + procedures + triggers
├── DB_Project_Report.docx      # Complete project report
├── dbProject_form_gui.docx     # GUI implementation documentation
├── dbProject_presentation.pptx # Project presentation slides
├── eerdProject.drawio          # Entity-Relationship diagram
└── sql_screenshot.docx         # Query execution screenshots
```

---

## 🗂️ Database Schema

The system is built around **8 core entity groups** mapped to **20 relational tables**:

| Entity Group | Tables | Purpose |
|---|---|---|
| Branch | `Bank_Branch` | Physical bank locations with contact info and manager references |
| Employee | `Employee` | Staff records linked to branches |
| Customer | `Customer` | Personal details of account holders |
| Account | `Account`, `Checking_Account`, `Saving_Account`, `Certificate_of_Deposit` | All account types with type-specific sub-tables |
| Transaction | `Bank_Transaction`, `Deposit`, `Withdrawal`, `Transfer` | All financial movements with type specialization |
| Loan | `Loan`, `Mortgage`, `Car_Loan`, `Personal_Loan` | Loan records with sub-type details |
| Card | `Card`, `Credit_Card`, `Debit_Card` | Payment cards with credit/debit specialization |
| Audit | `Loan_Audit_Log` | Automatic audit trail for loan status changes |

### Schema Dependency Tree

```
Bank_Branch ─┬─ Employee (BranchID)
             └─ Account (BranchID)

Customer ────┬─ Account (CusID)
             ├─ Loan (CusID)
             └─ Card (CusID)

Account ─────┬─ Checking_Account / Saving_Account / Certificate_of_Deposit
             ├─ Bank_Transaction
             └─ Card

Bank_Transaction ─┬─ Deposit
                  ├─ Withdrawal
                  └─ Transfer

Loan ─────────┬─ Mortgage / Car_Loan / Personal_Loan
Card ─────────┬─ Credit_Card / Debit_Card
```

---

## ⚙️ Key Features

### ✅ Constraints & Integrity
- `PRIMARY KEY` on all tables
- `FOREIGN KEY` with referential integrity across all relationships
- Deferrable FK on `Bank_Branch.ManagerEmpID` to resolve circular dependency with `Employee`
- `CHECK` constraints on `TypeCode`, `Status`, and `LoanType`
- `UNIQUE` on SSN fields; `NOT NULL` on all critical columns
- `DEFAULT` values for `Balance`, `Status`, and `DateOpened`

---

### 👁️ Views (5 Total)

| View | Purpose |
|---|---|
| `vw_Customer_Accounts` | Customer + account + branch summary per account |
| `vw_Employee_Branch` | Employees with their branch city and state |
| `vw_Active_Loans` | Active loans with customer and branch context |
| `vw_Transaction_History` | Full auditable transaction log with customer and employee info |
| `vw_Card_Holders` | Card details with cardholder name and linked account balance |

---

### 🔧 Stored Procedures (10 Total)

| Procedure | Operation | Key Rules |
|---|---|---|
| `sp_AddCustomer` | INSERT Customer | Duplicate SSN guard |
| `sp_AddAccount` | INSERT Account + sub-type | Routes by TypeCode to correct sub-table |
| `sp_Deposit` | INSERT Transaction + Deposit | Amount must be positive; updates balance |
| `sp_Withdrawal` | INSERT Transaction + Withdrawal | Checks balance + overdraft limit |
| `sp_Transfer` | INSERT Transaction + Transfer | Atomically debits sender, credits receiver |
| `sp_AddLoan` | INSERT Loan | Auto-sets StartDate and EndDate |
| `sp_UpdateAccountStatus` | UPDATE Account | Changes Active/Inactive/Closed |
| `sp_UpdateSalary` | UPDATE Employee | Salary must be positive |
| `sp_DeleteAccount` | DELETE Account | Requires zero balance before deletion |
| `sp_UpdateLoanBalance` | UPDATE Loan | Auto-closes loan when balance reaches 0 |

---

### ⚡ Triggers (6 Total)

| Trigger | Event | Purpose |
|---|---|---|
| `trg_Account_DateOpened` | INSERT on Account | Auto-sets `DateOpened = SYSDATE` if null |
| `trg_Prevent_Negative_Balance` | UPDATE on Account | Blocks balance below overdraft limit |
| `trg_Loan_Status_Change` | UPDATE on Loan | Writes old/new status to `Loan_Audit_Log` |
| `trg_Block_Account_Delete` | DELETE on Account | Blocks deletion if balance ≠ 0 or status is Active |
| `trg_Card_Expiry_Check` | INSERT/UPDATE on Card | Auto-sets `Status = Expired` when `ExpDate < SYSDATE` |
| `trg_Validate_Transaction` | INSERT on Bank_Transaction | Raises error if `Amount <= 0` |

---

## 🖥️ GUI Application (C# Windows Forms)

A Windows Forms app built in **C# (.NET)** connects to Oracle XE via `Oracle.ManagedDataAccess` and calls all stored procedures through a clean graphical interface.

| Form | Purpose |
|---|---|
| `LoginForm` | Employee authentication via email + SSN |
| `DashboardForm` | Central navigation hub to all modules |
| `CustomerForm` | Full CRUD on customer records |
| `AccountForm` | Add, search, and update accounts |
| `TransactionForm` | Process deposits, withdrawals, and transfers |
| `LoanForm` | Add loans and process payments |
| `EmployeeForm` | Search employees and update salaries |

---

## 🧪 Testing

All stored procedures were tested with valid and invalid inputs. All triggers were tested via direct DML. **16 test cases — 16 PASS.**

| Range | What Was Tested |
|---|---|
| T-01 to T-10 | Stored procedure tests (valid inputs, error conditions, edge cases) |
| T-11 to T-16 | Trigger tests (auto-set fields, balance guards, audit log, expiry check) |

---

## 🛠️ Tech Stack

- **Database:** Oracle SQL · PL/SQL (Oracle XE)
- **GUI:** C# · .NET · Windows Forms · Oracle.ManagedDataAccess
- **Diagramming:** draw.io (EERD)

---

## 🚀 Getting Started

1. Install **Oracle XE** and open SQL*Plus or SQL Developer.
2. Run `project db.sql` to create all tables, insert sample data, and set up procedures, views, and triggers.
3. Open the C# solution in Visual Studio.
4. Update the connection string in `DBHelper.cs` to match your Oracle credentials.
5. Build and run — log in with any employee's email and SSN.
