using Microsoft.Data.Sqlite;

namespace AharMealsWithLove.Data
{
    public static class SqliteHelper
    {
        private static readonly string DbPath;

        static SqliteHelper()
        {
            DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ahar.db");
        }

        private static SqliteConnection OpenDb()
        {
            var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            using var walCommand = connection.CreateCommand();
            walCommand.CommandText = "PRAGMA journal_mode=WAL;";
            walCommand.ExecuteNonQuery();

            using var foreignKeysCommand = connection.CreateCommand();
            foreignKeysCommand.CommandText = "PRAGMA foreign_keys=ON;";
            foreignKeysCommand.ExecuteNonQuery();

            return connection;
        }

        public static void ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            using var db = OpenDb();
            using var command = CreateCommand(db, sql, parameters);
            command.ExecuteNonQuery();
        }

        public static long ExecuteInsert(string sql, Dictionary<string, object>? parameters = null)
        {
            using var db = OpenDb();
            using var command = CreateCommand(db, sql, parameters);
            command.ExecuteNonQuery();

            using var idCommand = db.CreateCommand();
            idCommand.CommandText = "SELECT last_insert_rowid();";
            return (long)(idCommand.ExecuteScalar() ?? 0L);
        }

        public static List<Dictionary<string, string>> ExecuteQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            var results = new List<Dictionary<string, string>>();

            using var db = OpenDb();
            using var command = CreateCommand(db, sql, parameters);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var row = new Dictionary<string, string>();
                for (var i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? "" : reader.GetValue(i)?.ToString() ?? "";

                results.Add(row);
            }

            return results;
        }

        public static Dictionary<string, string>? ExecuteQuerySingle(string sql, Dictionary<string, object>? parameters = null)
        {
            var rows = ExecuteQuery(sql, parameters);
            return rows.Count > 0 ? rows[0] : null;
        }

        private static SqliteCommand CreateCommand(SqliteConnection db, string sql, Dictionary<string, object>? parameters)
        {
            var command = db.CreateCommand();
            command.CommandText = sql;

            if (parameters == null)
                return command;

            var index = 0;
            foreach (var value in parameters.Values)
            {
                var parameterName = $"$p{index++}";
                var placeholderIndex = command.CommandText.IndexOf('?');
                if (placeholderIndex >= 0)
                    command.CommandText = command.CommandText.Remove(placeholderIndex, 1).Insert(placeholderIndex, parameterName);

                command.Parameters.AddWithValue(parameterName, value ?? DBNull.Value);
            }

            return command;
        }

        public static void InitializeDatabase()
        {
            string[] tables = {
                @"CREATE TABLE IF NOT EXISTS uRegistration (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL UNIQUE,
                    Password TEXT,
                    Name TEXT,
                    Gender TEXT,
                    DOB TEXT,
                    Phone TEXT,
                    Address TEXT,
                    Aadhar TEXT,
                    AreaCode TEXT,
                    Image TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS ureg (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL,
                    OTP TEXT NOT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS vRegistration (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL UNIQUE,
                    Password TEXT,
                    Name TEXT,
                    Gender TEXT,
                    DOB TEXT,
                    Phone TEXT,
                    Address TEXT,
                    Aadhar TEXT,
                    AreaCode TEXT,
                    Image TEXT,
                    availstat TEXT DEFAULT 'unavailable'
                )",
                @"CREATE TABLE IF NOT EXISTS vreg (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL,
                    OTP TEXT NOT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS AdminLogin (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    admin_id TEXT NOT NULL,
                    Password TEXT NOT NULL
                )",
                @"CREATE TABLE IF NOT EXISTS hlog (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    hotelid TEXT,
                    Name TEXT,
                    fssai TEXT,
                    Contact TEXT,
                    Password TEXT,
                    Email TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS hregistration (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT,
                    Uname TEXT,
                    Name TEXT,
                    Gender TEXT,
                    DOB TEXT,
                    Phone TEXT,
                    Address TEXT,
                    Aadhar TEXT,
                    AreaCode TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS upick (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    uname TEXT,
                    Email TEXT,
                    eventdetail TEXT,
                    venue TEXT,
                    date TEXT,
                    category TEXT,
                    quantity TEXT,
                    cooktime TEXT,
                    exptime TEXT,
                    areacode TEXT,
                    status TEXT DEFAULT 'Submitted'
                )",
                @"CREATE TABLE IF NOT EXISTS hpick (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    uname TEXT,
                    Email TEXT,
                    eventdetail TEXT,
                    venue TEXT,
                    date TEXT,
                    category TEXT,
                    quantity TEXT,
                    cooktime TEXT,
                    exptime TEXT,
                    areacode TEXT,
                    status TEXT DEFAULT 'Submitted'
                )",
                @"CREATE TABLE IF NOT EXISTS Vehicle_details (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Vehicle_no TEXT,
                    owner_name TEXT,
                    F_name TEXT,
                    Uid_no TEXT,
                    Contact TEXT,
                    Email TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS aharspot (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    category TEXT,
                    type TEXT,
                    address TEXT,
                    nopeople TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS feedback (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Reason TEXT,
                    ComplainFor TEXT,
                    Detail TEXT,
                    UID INTEGER,
                    Ticket INTEGER,
                    Status TEXT DEFAULT 'Submitted'
                )",
                @"CREATE TABLE IF NOT EXISTS Contact (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    Contact TEXT,
                    Message TEXT,
                    Date TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS bdonat (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    email TEXT,
                    mobile TEXT,
                    dob TEXT,
                    address TEXT,
                    people TEXT,
                    amount TEXT,
                    date TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS info (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Subject TEXT,
                    Details TEXT,
                    Link TEXT,
                    Date TEXT
                )",
                @"CREATE TABLE IF NOT EXISTS Netbanking (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Bank TEXT
                )"
            };

            foreach (var sql in tables)
                ExecuteNonQuery(sql);

            var admin = ExecuteQuerySingle("SELECT ID FROM AdminLogin WHERE admin_id='admin'");
            if (admin == null)
                ExecuteNonQuery("INSERT INTO AdminLogin (admin_id, Password) VALUES ('admin', 'admin123')");

            var banks = ExecuteQuery("SELECT ID FROM Netbanking");
            if (banks.Count == 0)
            {
                string[] bankList = { "SBI", "HDFC", "ICICI", "Axis Bank", "PNB", "Bank of Baroda", "Canara Bank", "Union Bank" };
                foreach (var bank in bankList)
                    ExecuteNonQuery("INSERT INTO Netbanking (Bank) VALUES (?)", new() { ["bank"] = bank });
            }
        }
    }
}
