using System;
using System.IO;

namespace SureMeasure
{
    class Constants
    {
        public const string DatabaseFilename = "SMSRSQLite.db3";
        public const string OrdersFolderName = "Orders";

        private const string fileformat = "xml";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

        public static string OrdersPath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string path = Path.Combine(basePath, OrdersFolderName);
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string NewOrderPath
        {
            get
            {
                Random rand = new Random(DateTime.Now.Second);
                return Path.Combine(OrdersPath, $"{rand.Next().ToString()}.{fileformat}");
            }
        }


    }
}
