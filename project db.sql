-- ============================================================
-- BANK MANAGEMENT SYSTEM - COMPLETE SQL/PLSQL CODE
-- Based on EERD: Bank Branch, Employee, Customer,
-- Account, Bank_Transaction, Loan, Card
-- ============================================================

-- ============================================================
-- STEP 1: CREATE TABLES
-- ============================================================

CREATE TABLE Bank_Branch (
BranchID NUMBER PRIMARY KEY,
BranchName VARCHAR2(100) NOT NULL,
Street VARCHAR2(100),
City VARCHAR2(50),
State VARCHAR2(50),
Zip VARCHAR2(20),
PhoneNo VARCHAR2(20),
ManagerEmpID NUMBER
);

CREATE TABLE Employee (
EmpID NUMBER PRIMARY KEY,
SSN VARCHAR2(20) UNIQUE NOT NULL,
FName VARCHAR2(50) NOT NULL,
LName VARCHAR2(50) NOT NULL,
Position VARCHAR2(50),
Salary NUMBER(10,2),
PhoneNo VARCHAR2(20),
DateOfHire DATE,
Email VARCHAR2(100),
BranchID NUMBER,
CONSTRAINT fk_emp_branch FOREIGN KEY (BranchID) REFERENCES Bank_Branch(BranchID)
);

ALTER TABLE Bank_Branch ADD CONSTRAINT fk_branch_manager
FOREIGN KEY (ManagerEmpID) REFERENCES Employee(EmpID) DEFERRABLE INITIALLY DEFERRED;

CREATE TABLE Customer (
CusID NUMBER PRIMARY KEY,
SSN VARCHAR2(20) UNIQUE NOT NULL,
FName VARCHAR2(50) NOT NULL,
LName VARCHAR2(50) NOT NULL,
DOB DATE,
Email VARCHAR2(100),
PhoneNo VARCHAR2(20),
Address VARCHAR2(200)
);

CREATE TABLE Account (
AccNo NUMBER PRIMARY KEY,
Balance NUMBER(12,2) DEFAULT 0,
Rate NUMBER(5,4),
DateOpened DATE DEFAULT SYSDATE,
Status VARCHAR2(20) DEFAULT 'Active',
TypeCode VARCHAR2(20),
CusID NUMBER NOT NULL,
BranchID NUMBER NOT NULL,
CONSTRAINT fk_acc_customer FOREIGN KEY (CusID) REFERENCES Customer(CusID),
CONSTRAINT fk_acc_branch FOREIGN KEY (BranchID) REFERENCES Bank_Branch(BranchID),
CONSTRAINT chk_acc_type CHECK (TypeCode IN ('Checking','Saving','CD')),
CONSTRAINT chk_acc_status CHECK (Status IN ('Active','Inactive','Closed'))
);

CREATE TABLE Checking_Account (
AccNo NUMBER PRIMARY KEY,
OverdraftLimit NUMBER(10,2) DEFAULT 0,
CONSTRAINT fk_checking_acc FOREIGN KEY (AccNo) REFERENCES Account(AccNo)
);

CREATE TABLE Saving_Account (
AccNo NUMBER PRIMARY KEY,
MinBalRequirement NUMBER(10,2) DEFAULT 0,
CONSTRAINT fk_saving_acc FOREIGN KEY (AccNo) REFERENCES Account(AccNo)
);

CREATE TABLE Certificate_of_Deposit (
AccNo NUMBER PRIMARY KEY,
TermsOfDeposit VARCHAR2(100),
MaturityDate DATE,
CONSTRAINT fk_cd_acc FOREIGN KEY (AccNo) REFERENCES Account(AccNo)
);

CREATE TABLE Bank_Transaction (
TransID NUMBER PRIMARY KEY,
Amount NUMBER(12,2) NOT NULL,
Trans_DateTime TIMESTAMP DEFAULT SYSTIMESTAMP,
Description VARCHAR2(200),
TypeCode VARCHAR2(20),
AccNo NUMBER NOT NULL,
EmpID NUMBER,
CONSTRAINT fk_trans_acc FOREIGN KEY (AccNo) REFERENCES Account(AccNo),
CONSTRAINT fk_trans_emp FOREIGN KEY (EmpID) REFERENCES Employee(EmpID),
CONSTRAINT chk_trans_type CHECK (TypeCode IN ('Transfer','Deposit','Withdrawal'))
);

CREATE TABLE Transfer (
TransID NUMBER PRIMARY KEY,
FromAccNo NUMBER NOT NULL,
ToAccNo NUMBER NOT NULL,
CONSTRAINT fk_transfer_trans FOREIGN KEY (TransID) REFERENCES Bank_Transaction(TransID),
CONSTRAINT fk_transfer_from FOREIGN KEY (FromAccNo) REFERENCES Account(AccNo),
CONSTRAINT fk_transfer_to FOREIGN KEY (ToAccNo) REFERENCES Account(AccNo)
);

CREATE TABLE Deposit (
TransID NUMBER PRIMARY KEY,
Amount NUMBER(12,2) NOT NULL,
CONSTRAINT fk_deposit_trans FOREIGN KEY (TransID) REFERENCES Bank_Transaction(TransID)
);

CREATE TABLE Withdrawal (
TransID NUMBER PRIMARY KEY,
Amount NUMBER(12,2) NOT NULL,
PIN VARCHAR2(10),
CONSTRAINT fk_withdrawal_trans FOREIGN KEY (TransID) REFERENCES Bank_Transaction(TransID)
);

CREATE TABLE Loan (
LoanID NUMBER PRIMARY KEY,
Amount NUMBER(12,2) NOT NULL,
CurrBal NUMBER(12,2),
Rate NUMBER(5,4),
Term NUMBER,
StartDate DATE,
EndDate DATE,
Status VARCHAR2(20) DEFAULT 'Active',
LoanType VARCHAR2(20),
CusID NUMBER NOT NULL,
BranchID NUMBER NOT NULL,
CONSTRAINT fk_loan_customer FOREIGN KEY (CusID) REFERENCES Customer(CusID),
CONSTRAINT fk_loan_branch FOREIGN KEY (BranchID) REFERENCES Bank_Branch(BranchID),
CONSTRAINT chk_loan_type CHECK (LoanType IN ('Mortgage','CarLoan','PersonalLoan')),
CONSTRAINT chk_loan_status CHECK (Status IN ('Active','Closed','Defaulted'))
);

CREATE TABLE Mortgage (
LoanID NUMBER PRIMARY KEY,
PropertyAddress VARCHAR2(200),
CONSTRAINT fk_mortgage_loan FOREIGN KEY (LoanID) REFERENCES Loan(LoanID)
);

CREATE TABLE Car_Loan (
LoanID NUMBER PRIMARY KEY,
VIN VARCHAR2(50),
VehicleMake VARCHAR2(50),
Model VARCHAR2(50),
CONSTRAINT fk_carloan_loan FOREIGN KEY (LoanID) REFERENCES Loan(LoanID)
);

CREATE TABLE Personal_Loan (
LoanID NUMBER PRIMARY KEY,
Purpose VARCHAR2(100),
CONSTRAINT fk_personalloan_loan FOREIGN KEY (LoanID) REFERENCES Loan(LoanID)
);

CREATE TABLE Card (
CardNo VARCHAR2(20) PRIMARY KEY,
CVV VARCHAR2(5) NOT NULL,
Type VARCHAR2(20),
ExpDate DATE,
Status VARCHAR2(20) DEFAULT 'Active',
CusID NUMBER NOT NULL,
AccNo NUMBER NOT NULL,
CONSTRAINT fk_card_customer FOREIGN KEY (CusID) REFERENCES Customer(CusID),
CONSTRAINT fk_card_acc FOREIGN KEY (AccNo) REFERENCES Account(AccNo),
CONSTRAINT chk_card_type CHECK (Type IN ('Credit','Debit')),
CONSTRAINT chk_card_status CHECK (Status IN ('Active','Blocked','Expired'))
);

CREATE TABLE Credit_Card (
CardNo VARCHAR2(20) PRIMARY KEY,
CreditLimit NUMBER(10,2),
MinPayment NUMBER(10,2),
APR NUMBER(5,4),
CONSTRAINT fk_creditcard_card FOREIGN KEY (CardNo) REFERENCES Card(CardNo)
);

CREATE TABLE Debit_Card (
CardNo VARCHAR2(20) PRIMARY KEY,
PIN VARCHAR2(10),
CONSTRAINT fk_debitcard_card FOREIGN KEY (CardNo) REFERENCES Card(CardNo)
);

-- ============================================================
-- STEP 2: INSERT SAMPLE DATA
-- ============================================================

-- Insert Bank Branches (without manager first, add later)
INSERT INTO Bank_Branch (BranchID, BranchName, Street, City, State, Zip, PhoneNo)
VALUES (1, 'Downtown Branch', '123 Main St', 'New York', 'NY', '10001', '212-555-0100');
INSERT INTO Bank_Branch (BranchID, BranchName, Street, City, State, Zip, PhoneNo)
VALUES (2, 'Uptown Branch', '456 Oak Ave', 'New York', 'NY', '10025', '212-555-0200');
INSERT INTO Bank_Branch (BranchID, BranchName, Street, City, State, Zip, PhoneNo)
VALUES (3, 'Brooklyn Branch', '789 Pine Rd', 'Brooklyn', 'NY', '11201', '718-555-0300');

-- Insert Employees
INSERT INTO Employee (EmpID, SSN, FName, LName, Position, Salary, PhoneNo, DateOfHire, Email, BranchID)
VALUES (1, '111-22-3333', 'Alice', 'Johnson', 'Manager', 90000, '212-555-1001', DATE '2015-03-10', 'alice.j@bank.com', 1);
INSERT INTO Employee (EmpID, SSN, FName, LName, Position, Salary, PhoneNo, DateOfHire, Email, BranchID)
VALUES (2, '222-33-4444', 'Bob', 'Smith', 'Teller', 45000, '212-555-1002', DATE '2018-06-15', 'bob.s@bank.com', 1);
INSERT INTO Employee (EmpID, SSN, FName, LName, Position, Salary, PhoneNo, DateOfHire, Email, BranchID)
VALUES (3, '333-44-5555', 'Carol', 'Davis', 'Manager', 92000, '212-555-1003', DATE '2014-01-20', 'carol.d@bank.com', 2);
INSERT INTO Employee (EmpID, SSN, FName, LName, Position, Salary, PhoneNo, DateOfHire, Email, BranchID)
VALUES (4, '444-55-6666', 'David', 'Wilson', 'Loan Officer', 70000, '718-555-1004', DATE '2017-09-01', 'david.w@bank.com', 3);
INSERT INTO Employee (EmpID, SSN, FName, LName, Position, Salary, PhoneNo, DateOfHire, Email, BranchID)
VALUES (5, '555-66-7777', 'Emma', 'Brown', 'Teller', 44000, '212-555-1005', DATE '2020-02-14', 'emma.b@bank.com', 2);

-- Update Branch Managers
UPDATE Bank_Branch SET ManagerEmpID = 1 WHERE BranchID = 1;
UPDATE Bank_Branch SET ManagerEmpID = 3 WHERE BranchID = 2;
UPDATE Bank_Branch SET ManagerEmpID = 4 WHERE BranchID = 3;
COMMIT;

-- Insert Customers
INSERT INTO Customer (CusID, SSN, FName, LName, DOB, Email, PhoneNo, Address)
VALUES (1, '101-20-3040', 'John', 'Doe', DATE '1985-07-15', 'john.doe@email.com', '212-800-1001', '100 Elm St, NY');
INSERT INTO Customer (CusID, SSN, FName, LName, DOB, Email, PhoneNo, Address)
VALUES (2, '202-30-4050', 'Jane', 'Roe', DATE '1990-11-22', 'jane.roe@email.com', '212-800-1002', '200 Maple Ave, NY');
INSERT INTO Customer (CusID, SSN, FName, LName, DOB, Email, PhoneNo, Address)
VALUES (3, '303-40-5060', 'Mike', 'Adams', DATE '1978-04-30', 'mike.a@email.com', '718-800-1003', '300 Cedar Rd, Brooklyn');
INSERT INTO Customer (CusID, SSN, FName, LName, DOB, Email, PhoneNo, Address)
VALUES (4, '404-50-6070', 'Sara', 'Lee', DATE '1995-01-05', 'sara.l@email.com', '212-800-1004', '400 Birch Blvd, NY');
INSERT INTO Customer (CusID, SSN, FName, LName, DOB, Email, PhoneNo, Address)
VALUES (5, '505-60-7080', 'Tom', 'King', DATE '1988-09-18', 'tom.k@email.com', '718-800-1005', '500 Walnut St, Brooklyn');

-- Insert Accounts
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (1001, 5000, 0.0150, DATE '2020-01-10', 'Active', 'Checking', 1, 1);
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (1002, 12000, 0.0300, DATE '2019-06-20', 'Active', 'Saving', 1, 1);
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (1003, 8500, 0.0200, DATE '2021-03-15', 'Active', 'Checking', 2, 1);
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (1004, 25000, 0.0450, DATE '2018-11-01', 'Active', 'CD', 3, 3);
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (1005, 3200, 0.0250, DATE '2022-07-07', 'Active', 'Saving', 4, 2);
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (1006, 1500, 0.0150, DATE '2023-01-01', 'Active', 'Checking', 5, 3);

-- Insert Account Subtypes
INSERT INTO Checking_Account VALUES (1001, 500);
INSERT INTO Checking_Account VALUES (1003, 300);
INSERT INTO Checking_Account VALUES (1006, 200);
INSERT INTO Saving_Account VALUES (1002, 500);
INSERT INTO Saving_Account VALUES (1005, 200);
INSERT INTO Certificate_of_Deposit VALUES (1004, '12 Months Fixed', DATE '2019-11-01');

-- Insert Transactions
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (5001, 1000, SYSTIMESTAMP, 'Cash Deposit', 'Deposit', 1001, 2);
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (5002, 200, SYSTIMESTAMP, 'ATM Withdrawal', 'Withdrawal', 1001, NULL);
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (5003, 500, SYSTIMESTAMP, 'Transfer to Saving', 'Transfer', 1001, 2);
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (5004, 3000, SYSTIMESTAMP, 'Salary Deposit', 'Deposit', 1003, 5);
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (5005, 150, SYSTIMESTAMP, 'Bill Payment', 'Withdrawal', 1005, NULL);
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (5006, 750, SYSTIMESTAMP, 'Transfer to Checking', 'Transfer', 1002, 2);

-- Insert Bank_Transaction Subtypes
INSERT INTO Deposit VALUES (5001, 1000);
INSERT INTO Deposit VALUES (5004, 3000);
INSERT INTO Withdrawal VALUES (5002, 200, '7890');
INSERT INTO Withdrawal VALUES (5005, 150, '4321');
INSERT INTO Transfer VALUES (5003, 1001, 1002);
INSERT INTO Transfer VALUES (5006, 1002, 1001);

-- Insert Loans
INSERT INTO Loan (LoanID, Amount, CurrBal, Rate, Term, StartDate, EndDate, Status, LoanType, CusID, BranchID)
VALUES (9001, 250000, 240000, 0.0425, 360, DATE '2020-05-01', DATE '2050-05-01', 'Active', 'Mortgage', 1, 1);
INSERT INTO Loan (LoanID, Amount, CurrBal, Rate, Term, StartDate, EndDate, Status, LoanType, CusID, BranchID)
VALUES (9002, 20000, 15000, 0.0599, 60, DATE '2022-03-01', DATE '2027-03-01', 'Active', 'CarLoan', 2, 1);
INSERT INTO Loan (LoanID, Amount, CurrBal, Rate, Term, StartDate, EndDate, Status, LoanType, CusID, BranchID)
VALUES (9003, 5000, 3000, 0.0899, 24, DATE '2023-01-15', DATE '2025-01-15', 'Active', 'PersonalLoan', 3, 3);
INSERT INTO Loan (LoanID, Amount, CurrBal, Rate, Term, StartDate, EndDate, Status, LoanType, CusID, BranchID)
VALUES (9004, 180000, 170000, 0.0400, 240, DATE '2021-08-10', DATE '2041-08-10', 'Active', 'Mortgage', 4, 2);

-- Insert Loan Subtypes
INSERT INTO Mortgage VALUES (9001, '100 Elm St, New York, NY 10001');
INSERT INTO Mortgage VALUES (9004, '400 Birch Blvd, New York, NY 10007');
INSERT INTO Car_Loan VALUES (9002, 'VIN1234567890', 'Toyota', 'Camry');
INSERT INTO Personal_Loan VALUES (9003, 'Home Renovation');

-- Insert Cards
INSERT INTO Card (CardNo, CVV, Type, ExpDate, Status, CusID, AccNo)
VALUES ('4111111111111001', '123', 'Credit', DATE '2028-12-31', 'Active', 1, 1001);
INSERT INTO Card (CardNo, CVV, Type, ExpDate, Status, CusID, AccNo)
VALUES ('4111111111111002', '456', 'Debit', DATE '2027-06-30', 'Active', 1, 1002);
INSERT INTO Card (CardNo, CVV, Type, ExpDate, Status, CusID, AccNo)
VALUES ('4111111111111003', '789', 'Credit', DATE '2026-03-31', 'Active', 2, 1003);
INSERT INTO Card (CardNo, CVV, Type, ExpDate, Status, CusID, AccNo)
VALUES ('4111111111111004', '321', 'Debit', DATE '2027-09-30', 'Active', 3, 1004);
INSERT INTO Card (CardNo, CVV, Type, ExpDate, Status, CusID, AccNo)
VALUES ('4111111111111005', '654', 'Credit', DATE '2029-01-31', 'Active', 4, 1005);

-- Insert Card Subtypes
INSERT INTO Credit_Card VALUES ('4111111111111001', 10000, 50, 0.1999);
INSERT INTO Credit_Card VALUES ('4111111111111003', 5000, 25, 0.2199);
INSERT INTO Credit_Card VALUES ('4111111111111005', 7500, 35, 0.1799);
INSERT INTO Debit_Card VALUES ('4111111111111002', '1234');
INSERT INTO Debit_Card VALUES ('4111111111111004', '5678');

COMMIT;

-- ============================================================
-- STEP 3: QUERIES (SELECT, JOINS, SUBQUERIES)
-- ============================================================

-- Basic Selects
SELECT * FROM Bank_Branch;
SELECT * FROM Employee;
SELECT * FROM Customer;
SELECT * FROM Account;
SELECT * FROM Bank_Transaction;
SELECT * FROM Loan;
SELECT * FROM Card;

-- JOIN: Employees with their Branch Name
SELECT e.EmpID, e.FName, e.LName, e.Position, b.BranchName
FROM Employee e
JOIN Bank_Branch b ON e.BranchID = b.BranchID;

-- JOIN: Customers with their Accounts
SELECT c.CusID, c.FName, c.LName, a.AccNo, a.TypeCode, a.Balance, a.Status
FROM Customer c
JOIN Account a ON c.CusID = a.CusID;

-- JOIN: Accounts with Branch Info
SELECT a.AccNo, a.TypeCode, a.Balance, b.BranchName, b.City
FROM Account a
JOIN Bank_Branch b ON a.BranchID = b.BranchID;

-- JOIN: Transactions with Account and Customer
SELECT t.TransID, t.TypeCode, t.Amount, t.Trans_DateTime, c.FName, c.LName, a.AccNo
FROM Bank_Transaction t
JOIN Account a ON t.AccNo = a.AccNo
JOIN Customer c ON a.CusID = c.CusID
ORDER BY t.Trans_DateTime DESC;

-- JOIN: Loans with Customer and Branch
SELECT l.LoanID, l.LoanType, l.Amount, l.CurrBal, l.Status,
c.FName, c.LName, b.BranchName
FROM Loan l
JOIN Customer c ON l.CusID = c.CusID
JOIN Bank_Branch b ON l.BranchID = b.BranchID;

-- JOIN: Cards with Customer and Account
SELECT ca.CardNo, ca.Type, ca.ExpDate, ca.Status,
c.FName, c.LName, a.AccNo
FROM Card ca
JOIN Customer c ON ca.CusID = c.CusID
JOIN Account a ON ca.AccNo = a.AccNo;

-- SUBQUERY: Customers who have more than one account
SELECT CusID, FName, LName
FROM Customer
WHERE CusID IN (
SELECT CusID FROM Account
GROUP BY CusID
HAVING COUNT(*) > 1
);

-- SUBQUERY: Accounts with balance above average
SELECT AccNo, TypeCode, Balance, CusID
FROM Account
WHERE Balance > (SELECT AVG(Balance) FROM Account);

-- SUBQUERY: Employees earning above branch average salary
SELECT EmpID, FName, LName, Salary, BranchID
FROM Employee e
WHERE Salary > (
SELECT AVG(Salary) FROM Employee WHERE BranchID = e.BranchID
);

-- SUBQUERY: Customers who have active loans
SELECT CusID, FName, LName
FROM Customer
WHERE CusID IN (
SELECT CusID FROM Loan WHERE Status = 'Active'
);

-- SUBQUERY: Branch with highest total deposits
SELECT BranchID, BranchName
FROM Bank_Branch
WHERE BranchID = (
SELECT BranchID FROM (
SELECT BranchID FROM Account
GROUP BY BranchID
ORDER BY SUM(Balance) DESC
) WHERE ROWNUM = 1
);

-- ============================================================
-- STEP 4: VIEWS
-- ============================================================

-- View: Customer Account Summary
CREATE OR REPLACE VIEW vw_Customer_Accounts AS
SELECT c.CusID, c.FName || ' ' || c.LName AS CustomerName,
c.Email, c.PhoneNo,
a.AccNo, a.TypeCode, a.Balance, a.Status AS AccStatus,
b.BranchName
FROM Customer c
JOIN Account a ON c.CusID = a.CusID
JOIN Bank_Branch b ON a.BranchID = b.BranchID;

-- View: Employee Branch Details
CREATE OR REPLACE VIEW vw_Employee_Branch AS
SELECT e.EmpID, e.FName || ' ' || e.LName AS EmployeeName,
e.Position, e.Salary, e.Email,
b.BranchName, b.City, b.State
FROM Employee e
JOIN Bank_Branch b ON e.BranchID = b.BranchID;

-- View: Active Loans Summary
CREATE OR REPLACE VIEW vw_Active_Loans AS
SELECT l.LoanID, l.LoanType, l.Amount, l.CurrBal,
l.Rate, l.StartDate, l.EndDate,
c.FName || ' ' || c.LName AS CustomerName,
b.BranchName
FROM Loan l
JOIN Customer c ON l.CusID = c.CusID
JOIN Bank_Branch b ON l.BranchID = b.BranchID
WHERE l.Status = 'Active';

-- View: Bank_Transaction History with Customer Info
CREATE OR REPLACE VIEW vw_Transaction_History AS
SELECT t.TransID, t.TypeCode, t.Amount, t.Trans_DateTime,
t.Description, a.AccNo,
c.FName || ' ' || c.LName AS CustomerName,
e.FName || ' ' || e.LName AS ProcessedBy
FROM Bank_Transaction t
JOIN Account a ON t.AccNo = a.AccNo
JOIN Customer c ON a.CusID = c.CusID
LEFT JOIN Employee e ON t.EmpID = e.EmpID;

-- View: Card Holder Summary
CREATE OR REPLACE VIEW vw_Card_Holders AS
SELECT ca.CardNo, ca.Type, ca.ExpDate, ca.Status,
c.FName || ' ' || c.LName AS CardHolder,
a.AccNo, a.TypeCode, a.Balance
FROM Card ca
JOIN Customer c ON ca.CusID = c.CusID
JOIN Account a ON ca.AccNo = a.AccNo;

-- ============================================================
-- STEP 5: STORED PROCEDURES (INSERT / UPDATE / DELETE)
-- ============================================================

-- Procedure: Add New Customer
CREATE OR REPLACE PROCEDURE sp_AddCustomer(
p_CusID IN NUMBER,
p_SSN IN VARCHAR2,
p_FName IN VARCHAR2,
p_LName IN VARCHAR2,
p_DOB IN DATE,
p_Email IN VARCHAR2,
p_PhoneNo IN VARCHAR2,
p_Address IN VARCHAR2
) AS
BEGIN
INSERT INTO Customer (CusID, SSN, FName, LName, DOB, Email, PhoneNo, Address)
VALUES (p_CusID, p_SSN, p_FName, p_LName, p_DOB, p_Email, p_PhoneNo, p_Address);
COMMIT;
DBMS_OUTPUT.PUT_LINE('Customer added: ' || p_FName || ' ' || p_LName);
EXCEPTION
WHEN DUP_VAL_ON_INDEX THEN
DBMS_OUTPUT.PUT_LINE('Error: Customer ID or SSN already exists.');
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_AddCustomer;
/

-- Procedure: Add New Account
CREATE OR REPLACE PROCEDURE sp_AddAccount(
p_AccNo IN NUMBER,
p_Balance IN NUMBER,
p_Rate IN NUMBER,
p_TypeCode IN VARCHAR2,
p_CusID IN NUMBER,
p_BranchID IN NUMBER
) AS
BEGIN
INSERT INTO Account (AccNo, Balance, Rate, DateOpened, Status, TypeCode, CusID, BranchID)
VALUES (p_AccNo, p_Balance, p_Rate, SYSDATE, 'Active', p_TypeCode, p_CusID, p_BranchID);
IF p_TypeCode = 'Checking' THEN
INSERT INTO Checking_Account VALUES (p_AccNo, 500);
ELSIF p_TypeCode = 'Saving' THEN
INSERT INTO Saving_Account VALUES (p_AccNo, 200);
ELSIF p_TypeCode = 'CD' THEN
INSERT INTO Certificate_of_Deposit VALUES (p_AccNo, '12 Months', ADD_MONTHS(SYSDATE, 12));
END IF;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Account created: ' || p_AccNo || ' Type: ' || p_TypeCode);
EXCEPTION
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_AddAccount;
/

-- Procedure: Process Deposit
CREATE OR REPLACE PROCEDURE sp_Deposit(
p_TransID IN NUMBER,
p_AccNo IN NUMBER,
p_Amount IN NUMBER,
p_EmpID IN NUMBER,
p_Description IN VARCHAR2
) AS
BEGIN
IF p_Amount <= 0 THEN
RAISE_APPLICATION_ERROR(-20001, 'Deposit amount must be positive.');
END IF;
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (p_TransID, p_Amount, SYSTIMESTAMP, p_Description, 'Deposit', p_AccNo, p_EmpID);
INSERT INTO Deposit VALUES (p_TransID, p_Amount);
UPDATE Account SET Balance = Balance + p_Amount WHERE AccNo = p_AccNo;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Deposit of ' || p_Amount || ' to Account ' || p_AccNo || ' successful.');
EXCEPTION
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_Deposit;
/

-- Procedure: Process Withdrawal
CREATE OR REPLACE PROCEDURE sp_Withdrawal(
p_TransID IN NUMBER,
p_AccNo IN NUMBER,
p_Amount IN NUMBER,
p_PIN IN VARCHAR2
) AS
v_Balance NUMBER;
v_Overdraft NUMBER := 0;
v_TypeCode VARCHAR2(20);
BEGIN
SELECT Balance, TypeCode INTO v_Balance, v_TypeCode
FROM Account WHERE AccNo = p_AccNo;
IF v_TypeCode = 'Checking' THEN
SELECT OverdraftLimit INTO v_Overdraft
FROM Checking_Account WHERE AccNo = p_AccNo;
END IF;
IF p_Amount <= 0 THEN
RAISE_APPLICATION_ERROR(-20002, 'Withdrawal amount must be positive.');
END IF;
IF (v_Balance + v_Overdraft) < p_Amount THEN
RAISE_APPLICATION_ERROR(-20003, 'Insufficient funds including overdraft limit.');
END IF;
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (p_TransID, p_Amount, SYSTIMESTAMP, 'ATM/Teller Withdrawal', 'Withdrawal', p_AccNo, NULL);
INSERT INTO Withdrawal VALUES (p_TransID, p_Amount, p_PIN);
UPDATE Account SET Balance = Balance - p_Amount WHERE AccNo = p_AccNo;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Withdrawal of ' || p_Amount || ' from Account ' || p_AccNo || ' successful.');
EXCEPTION
WHEN NO_DATA_FOUND THEN
DBMS_OUTPUT.PUT_LINE('Error: Account not found.');
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_Withdrawal;
/

-- Procedure: Process Transfer
CREATE OR REPLACE PROCEDURE sp_Transfer(
p_TransID IN NUMBER,
p_FromAccNo IN NUMBER,
p_ToAccNo IN NUMBER,
p_Amount IN NUMBER,
p_EmpID IN NUMBER
) AS
v_Balance NUMBER;
BEGIN
SELECT Balance INTO v_Balance FROM Account WHERE AccNo = p_FromAccNo;
IF p_Amount <= 0 THEN
RAISE_APPLICATION_ERROR(-20004, 'Transfer amount must be positive.');
END IF;
IF v_Balance < p_Amount THEN
RAISE_APPLICATION_ERROR(-20005, 'Insufficient balance for transfer.');
END IF;
INSERT INTO Bank_Transaction (TransID, Amount, Trans_DateTime, Description, TypeCode, AccNo, EmpID)
VALUES (p_TransID, p_Amount, SYSTIMESTAMP, 'Fund Transfer', 'Transfer', p_FromAccNo, p_EmpID);
INSERT INTO Transfer VALUES (p_TransID, p_FromAccNo, p_ToAccNo);
UPDATE Account SET Balance = Balance - p_Amount WHERE AccNo = p_FromAccNo;
UPDATE Account SET Balance = Balance + p_Amount WHERE AccNo = p_ToAccNo;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Transfer of ' || p_Amount || ' from ' || p_FromAccNo || ' to ' || p_ToAccNo || ' done.');
EXCEPTION
WHEN NO_DATA_FOUND THEN
DBMS_OUTPUT.PUT_LINE('Error: Account not found.');
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_Transfer;
/

-- Procedure: Add Loan
CREATE OR REPLACE PROCEDURE sp_AddLoan(
p_LoanID IN NUMBER,
p_Amount IN NUMBER,
p_Rate IN NUMBER,
p_Term IN NUMBER,
p_LoanType IN VARCHAR2,
p_CusID IN NUMBER,
p_BranchID IN NUMBER
) AS
BEGIN
INSERT INTO Loan (LoanID, Amount, CurrBal, Rate, Term, StartDate, EndDate, Status, LoanType, CusID, BranchID)
VALUES (p_LoanID, p_Amount, p_Amount, p_Rate, p_Term, SYSDATE, ADD_MONTHS(SYSDATE, p_Term), 'Active', p_LoanType, p_CusID, p_BranchID);
COMMIT;
DBMS_OUTPUT.PUT_LINE('Loan ' || p_LoanID || ' of type ' || p_LoanType || ' added for Customer ' || p_CusID);
EXCEPTION
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_AddLoan;
/

-- Procedure: Update Account Status
CREATE OR REPLACE PROCEDURE sp_UpdateAccountStatus(
p_AccNo IN NUMBER,
p_Status IN VARCHAR2
) AS
BEGIN
UPDATE Account SET Status = p_Status WHERE AccNo = p_AccNo;
IF SQL%ROWCOUNT = 0 THEN
RAISE_APPLICATION_ERROR(-20006, 'Account not found.');
END IF;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Account ' || p_AccNo || ' status updated to ' || p_Status);
EXCEPTION
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_UpdateAccountStatus;
/

-- Procedure: Update Employee Salary
CREATE OR REPLACE PROCEDURE sp_UpdateSalary(
p_EmpID IN NUMBER,
p_NewSalary IN NUMBER
) AS
BEGIN
IF p_NewSalary <= 0 THEN
RAISE_APPLICATION_ERROR(-20007, 'Salary must be a positive value.');
END IF;
UPDATE Employee SET Salary = p_NewSalary WHERE EmpID = p_EmpID;
IF SQL%ROWCOUNT = 0 THEN
RAISE_APPLICATION_ERROR(-20008, 'Employee not found.');
END IF;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Salary updated for Employee ' || p_EmpID);
EXCEPTION
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_UpdateSalary;
/

-- Procedure: Delete Account (if balance is zero and no active loans)
CREATE OR REPLACE PROCEDURE sp_DeleteAccount(
p_AccNo IN NUMBER
) AS
v_Balance NUMBER;
BEGIN
SELECT Balance INTO v_Balance FROM Account WHERE AccNo = p_AccNo;
IF v_Balance <> 0 THEN
RAISE_APPLICATION_ERROR(-20009, 'Cannot delete account with non-zero balance.');
END IF;
DELETE FROM Checking_Account WHERE AccNo = p_AccNo;
DELETE FROM Saving_Account WHERE AccNo = p_AccNo;
DELETE FROM Certificate_of_Deposit WHERE AccNo = p_AccNo;
DELETE FROM Account WHERE AccNo = p_AccNo;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Account ' || p_AccNo || ' deleted successfully.');
EXCEPTION
WHEN NO_DATA_FOUND THEN
DBMS_OUTPUT.PUT_LINE('Error: Account not found.');
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_DeleteAccount;
/

-- Procedure: Update Loan Balance (after payment)
CREATE OR REPLACE PROCEDURE sp_UpdateLoanBalance(
p_LoanID IN NUMBER,
p_PaymentAmount IN NUMBER
) AS
v_CurrBal NUMBER;
BEGIN
SELECT CurrBal INTO v_CurrBal FROM Loan WHERE LoanID = p_LoanID;
IF p_PaymentAmount <= 0 THEN
RAISE_APPLICATION_ERROR(-20010, 'Payment amount must be positive.');
END IF;
IF p_PaymentAmount > v_CurrBal THEN
RAISE_APPLICATION_ERROR(-20011, 'Payment exceeds remaining loan balance.');
END IF;
UPDATE Loan SET CurrBal = CurrBal - p_PaymentAmount WHERE LoanID = p_LoanID;
IF (v_CurrBal - p_PaymentAmount) = 0 THEN
UPDATE Loan SET Status = 'Closed' WHERE LoanID = p_LoanID;
END IF;
COMMIT;
DBMS_OUTPUT.PUT_LINE('Loan ' || p_LoanID || ' payment of ' || p_PaymentAmount || ' processed.');
EXCEPTION
WHEN NO_DATA_FOUND THEN
DBMS_OUTPUT.PUT_LINE('Error: Loan not found.');
WHEN OTHERS THEN
ROLLBACK;
DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
END sp_UpdateLoanBalance;
/

-- ============================================================
-- STEP 6: TRIGGERS
-- ============================================================

-- Trigger 1: Auto-set DateOpened on Account insert
CREATE OR REPLACE TRIGGER trg_Account_DateOpened
BEFORE INSERT ON Account
FOR EACH ROW
BEGIN
IF :NEW.DateOpened IS NULL THEN
:NEW.DateOpened := SYSDATE;
END IF;
END;
/

-- Trigger 2: Prevent negative balance on Accounts
CREATE OR REPLACE TRIGGER trg_Prevent_Negative_Balance
BEFORE UPDATE ON Account
FOR EACH ROW
DECLARE
v_Overdraft NUMBER := 0;
BEGIN
IF :NEW.TypeCode = 'Checking' THEN
BEGIN
SELECT OverdraftLimit INTO v_Overdraft
FROM Checking_Account WHERE AccNo = :NEW.AccNo;
EXCEPTION
WHEN NO_DATA_FOUND THEN v_Overdraft := 0;
END;
END IF;
IF :NEW.Balance < -v_Overdraft THEN
RAISE_APPLICATION_ERROR(-20020, 'Balance cannot go below overdraft limit.');
END IF;
END;
/

-- Trigger 3: Log when a loan status changes to 'Closed'
CREATE TABLE Loan_Audit_Log (
LogID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
LoanID NUMBER,
OldStatus VARCHAR2(20),
NewStatus VARCHAR2(20),
ChangedOn DATE DEFAULT SYSDATE
);

CREATE OR REPLACE TRIGGER trg_Loan_Status_Change
AFTER UPDATE OF Status ON Loan
FOR EACH ROW
WHEN (OLD.Status <> NEW.Status)
BEGIN
INSERT INTO Loan_Audit_Log (LoanID, OldStatus, NewStatus, ChangedOn)
VALUES (:OLD.LoanID, :OLD.Status, :NEW.Status, SYSDATE);
END;
/

-- Trigger 4: Prevent deletion of active accounts with balance
CREATE OR REPLACE TRIGGER trg_Block_Account_Delete
BEFORE DELETE ON Account
FOR EACH ROW
BEGIN
IF :OLD.Balance <> 0 THEN
RAISE_APPLICATION_ERROR(-20021, 'Cannot delete account with remaining balance of ' || :OLD.Balance);
END IF;
IF :OLD.Status = 'Active' THEN
RAISE_APPLICATION_ERROR(-20022, 'Deactivate account before deletion.');
END IF;
END;
/

-- Trigger 5: Auto-update card status to 'Expired' when ExpDate passes
CREATE OR REPLACE TRIGGER trg_Card_Expiry_Check
BEFORE INSERT OR UPDATE ON Card
FOR EACH ROW
BEGIN
IF :NEW.ExpDate < SYSDATE THEN
:NEW.Status := 'Expired';
END IF;
END;
/

-- Trigger 6: Validate transaction amount is positive
CREATE OR REPLACE TRIGGER trg_Validate_Transaction
BEFORE INSERT ON Bank_Transaction
FOR EACH ROW
BEGIN
IF :NEW.Amount <= 0 THEN
RAISE_APPLICATION_ERROR(-20023, 'Bank_Transaction amount must be greater than zero.');
END IF;
END;
/

-- ============================================================
-- STEP 7: TEST THE COMPLETE SYSTEM
-- ============================================================

-- Test: Add a new customer
BEGIN
sp_AddCustomer(6, '606-70-8090', 'Lily', 'Green', DATE '1993-05-12', 'lily.g@email.com', '212-900-1006', '600 Rose St, NY');
END;
/

-- Test: Add a new account
BEGIN
sp_AddAccount(1007, 1000, 0.015, 'Checking', 6, 1);
END;
/

-- Test: Deposit
BEGIN
sp_Deposit(5007, 1007, 500, 2, 'Initial Deposit');
END;
/

-- Test: Withdrawal
BEGIN
sp_Withdrawal(5008, 1007, 200, '9999');
END;
/

-- Test: Transfer
BEGIN
sp_Transfer(5009, 1007, 1001, 100, 2);
END;
/

-- Test: Add a loan
BEGIN
sp_AddLoan(9005, 10000, 0.07, 36, 'PersonalLoan', 6, 1);
END;
/

-- Test: Loan payment
BEGIN
sp_UpdateLoanBalance(9005, 500);
END;
/

-- Test: Update account status
BEGIN
sp_UpdateAccountStatus(1007, 'Inactive');
END;
/

-- Test: Update salary
BEGIN
sp_UpdateSalary(2, 48000);
END;
/

-- Verify via Views
SELECT * FROM vw_Customer_Accounts;
SELECT * FROM vw_Employee_Branch;
SELECT * FROM vw_Active_Loans;
SELECT * FROM vw_Transaction_History;
SELECT * FROM vw_Card_Holders;

-- Verify Triggers fired (audit log)
SELECT * FROM Loan_Audit_Log;

-- Final comprehensive query: Full customer profile
SELECT
c.CusID,
c.FName || ' ' || c.LName AS Name,
c.Email,
COUNT(DISTINCT a.AccNo) AS TotalAccounts,
SUM(a.Balance) AS TotalBalance,
COUNT(DISTINCT l.LoanID) AS TotalLoans,
COUNT(DISTINCT ca.CardNo) AS TotalCards
FROM Customer c
LEFT JOIN Account a ON c.CusID = a.CusID
LEFT JOIN Loan l ON c.CusID = l.CusID AND l.Status = 'Active'
LEFT JOIN Card ca ON c.CusID = ca.CusID AND ca.Status = 'Active'
GROUP BY c.CusID, c.FName, c.LName, c.Email
ORDER BY c.CusID;