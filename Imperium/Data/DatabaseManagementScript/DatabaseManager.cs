using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Imperium.Data.DatabaseManagementScript
{
    class DatabaseManager
    {

        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Open();
            return conn;
        }

    }
}
