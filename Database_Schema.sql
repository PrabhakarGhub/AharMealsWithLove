-- ================================================================
-- AHAR - MEALS WITH LOVE | Database Schema (SQLite)
-- Auto-created by the application on first run
-- ================================================================

CREATE TABLE IF NOT EXISTS uRegistration (
    ID       INTEGER PRIMARY KEY AUTOINCREMENT,
    Email    TEXT NOT NULL UNIQUE,
    Password TEXT,
    Name     TEXT,
    Gender   TEXT,
    DOB      TEXT,
    Phone    TEXT,
    Address  TEXT,
    Aadhar   TEXT,
    AreaCode TEXT,
    Image    TEXT
);

CREATE TABLE IF NOT EXISTS ureg (
    ID    INTEGER PRIMARY KEY AUTOINCREMENT,
    Email TEXT NOT NULL,
    OTP   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS vRegistration (
    ID        INTEGER PRIMARY KEY AUTOINCREMENT,
    Email     TEXT NOT NULL UNIQUE,
    Password  TEXT,
    Name      TEXT,
    Gender    TEXT,
    DOB       TEXT,
    Phone     TEXT,
    Address   TEXT,
    Aadhar    TEXT,
    AreaCode  TEXT,
    Image     TEXT,
    availstat TEXT DEFAULT 'unavailable'
);

CREATE TABLE IF NOT EXISTS vreg (
    ID    INTEGER PRIMARY KEY AUTOINCREMENT,
    Email TEXT NOT NULL,
    OTP   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS AdminLogin (
    ID       INTEGER PRIMARY KEY AUTOINCREMENT,
    admin_id TEXT NOT NULL,
    Password TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS hlog (
    ID      INTEGER PRIMARY KEY AUTOINCREMENT,
    hotelid TEXT,
    Name    TEXT,
    fssai   TEXT,
    Contact TEXT,
    Password TEXT,
    Email   TEXT
);

CREATE TABLE IF NOT EXISTS hregistration (
    ID       INTEGER PRIMARY KEY AUTOINCREMENT,
    Email    TEXT,
    Uname    TEXT,
    Name     TEXT,
    Gender   TEXT,
    DOB      TEXT,
    Phone    TEXT,
    Address  TEXT,
    Aadhar   TEXT,
    AreaCode TEXT
);

CREATE TABLE IF NOT EXISTS upick (
    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
    uname       TEXT,
    Email       TEXT,
    eventdetail TEXT,
    venue       TEXT,
    date        TEXT,
    category    TEXT,
    quantity    TEXT,
    cooktime    TEXT,
    exptime     TEXT,
    areacode    TEXT,
    status      TEXT DEFAULT 'Submitted'
);

CREATE TABLE IF NOT EXISTS hpick (
    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
    uname       TEXT,
    Email       TEXT,
    eventdetail TEXT,
    venue       TEXT,
    date        TEXT,
    category    TEXT,
    quantity    TEXT,
    cooktime    TEXT,
    exptime     TEXT,
    areacode    TEXT,
    status      TEXT DEFAULT 'Submitted'
);

CREATE TABLE IF NOT EXISTS Vehicle_details (
    ID         INTEGER PRIMARY KEY AUTOINCREMENT,
    Vehicle_no TEXT,
    owner_name TEXT,
    F_name     TEXT,
    Uid_no     TEXT,
    Contact    TEXT,
    Email      TEXT
);

CREATE TABLE IF NOT EXISTS aharspot (
    ID       INTEGER PRIMARY KEY AUTOINCREMENT,
    category TEXT,
    type     TEXT,
    address  TEXT,
    nopeople TEXT
);

CREATE TABLE IF NOT EXISTS feedback (
    ID         INTEGER PRIMARY KEY AUTOINCREMENT,
    Reason     TEXT,
    ComplainFor TEXT,
    Detail     TEXT,
    UID        INTEGER,
    Ticket     INTEGER,
    Status     TEXT DEFAULT 'Submitted'
);

CREATE TABLE IF NOT EXISTS Contact (
    ID      INTEGER PRIMARY KEY AUTOINCREMENT,
    Name    TEXT,
    Contact TEXT,
    Message TEXT,
    Date    TEXT
);

CREATE TABLE IF NOT EXISTS bdonat (
    ID      INTEGER PRIMARY KEY AUTOINCREMENT,
    name    TEXT,
    email   TEXT,
    mobile  TEXT,
    dob     TEXT,
    address TEXT,
    people  TEXT,
    amount  TEXT,
    date    TEXT
);

CREATE TABLE IF NOT EXISTS info (
    ID      INTEGER PRIMARY KEY AUTOINCREMENT,
    Subject TEXT,
    Details TEXT,
    Link    TEXT,
    Date    TEXT
);

CREATE TABLE IF NOT EXISTS Netbanking (
    ID   INTEGER PRIMARY KEY AUTOINCREMENT,
    Bank TEXT
);

-- Default admin seed
INSERT OR IGNORE INTO AdminLogin (admin_id, Password) VALUES ('admin', 'admin123');
