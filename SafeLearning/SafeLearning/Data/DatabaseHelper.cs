using System.IO;                // עבודה עם תיקיות ונתיבי קבצים
using Microsoft.Data.Sqlite;    // החבילה שמאפשרת לעבוד מול SQLite

namespace SafeLearning.Data
{
    // מחלקה סטטית: משתמשים בה ישירות, בלי ליצור ממנה אובייקט
    // אחראית על החיבור למסד הנתונים ועל יצירת הטבלאות
    public static class DatabaseHelper
    {
        // התיקייה של התוכנה בתוך AppData\Local של המשתמש
        private static readonly string DatabaseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SafeLearning");

        // הנתיב המלא לקובץ מסד הנתונים
        public static readonly string DatabasePath =
            Path.Combine(DatabaseFolder, "SafeLearning.db");

        // פרטי החיבור: איפה הקובץ, והפעלת בדיקת מפתחות זרים
        private static readonly string ConnectionString =
            $"Data Source={DatabasePath};Foreign Keys=True";

        // פותחת חיבור חדש למסד הנתונים ומחזירה אותו
        public static SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        // יוצרת את התיקייה ואת הטבלאות אם הן עדיין לא קיימות
        public static void Initialize()
        {
            // יוצרת את התיקייה. אם היא כבר קיימת, לא קורה כלום
            Directory.CreateDirectory(DatabaseFolder);

            // using סוגר את המשאב אוטומטית בסוף הפעולה
            using var connection = GetConnection();

            // טרנזקציה: או שכל הטבלאות נוצרות, או שאף אחת לא נוצרת
            using var transaction = connection.BeginTransaction();

            // הפקודה שתריץ את שאילתת ה-SQL
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            // IF NOT EXISTS: יוצר את הטבלה רק אם היא לא קיימת
            // PRIMARY KEY: מפתח ראשי | AUTOINCREMENT: מספר שעולה לבד
            // NOT NULL: שדה חובה | UNIQUE: ערך שלא יכול לחזור על עצמו
            // DEFAULT: ערך ברירת מחדל | FOREIGN KEY: מפתח זר לטבלה אחרת
            command.CommandText = @"
                -- טבלת מקצועות
                CREATE TABLE IF NOT EXISTS Subjects (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );

                -- טבלת מטלות: כל מטלה שייכת למקצוע אחד
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    SubjectId INTEGER NOT NULL,
                    DueDate TEXT NOT NULL,
                    Priority INTEGER NOT NULL DEFAULT 0,
                    IsDone INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
                );

                -- טבלת סשנים: שייך למקצוע, והשיוך למטלה אינו חובה
                CREATE TABLE IF NOT EXISTS Sessions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TaskId INTEGER,
                    SubjectId INTEGER NOT NULL,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT,
                    DurationMinutes INTEGER NOT NULL DEFAULT 0,
                    BlockedAttempts INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (TaskId) REFERENCES Tasks(Id),
                    FOREIGN KEY (SubjectId) REFERENCES Subjects(Id)
                );

                -- טבלת התוכנות החסומות
                CREATE TABLE IF NOT EXISTS BlockedApps (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    ProcessName TEXT NOT NULL UNIQUE
                );

                -- טבלת קישור: איזו תוכנה נסגרה, באיזה סשן, מתי וכמה פעמים
                -- UNIQUE: שורה אחת בלבד לכל צירוף של סשן ותוכנה
                CREATE TABLE IF NOT EXISTS SessionBlocks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SessionId INTEGER NOT NULL,
                    BlockedAppId INTEGER NOT NULL,
                    BlockedAt TEXT NOT NULL,
                    CloseCount INTEGER NOT NULL DEFAULT 1,
                    UNIQUE (SessionId, BlockedAppId),
                    FOREIGN KEY (SessionId) REFERENCES Sessions(Id),
                    FOREIGN KEY (BlockedAppId) REFERENCES BlockedApps(Id)
                );

                -- טבלת הגדרות: שם הגדרה והערך שלה
                CREATE TABLE IF NOT EXISTS Settings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Key TEXT NOT NULL UNIQUE,
                    Value TEXT NOT NULL
                );";

            // מריצה את השאילתה
            command.ExecuteNonQuery();

            // מאשרת את הטרנזקציה ושומרת את השינויים בקובץ
            transaction.Commit();
        }
    }
}