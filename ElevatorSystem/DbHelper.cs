#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace ElevatorSystem
{
    // Stores the details of one elevator event for the operation log.
    public class LogEntry
    {
        public int FromFloor;
        public int ToFloor;
        public string Description;
        public DateTime Time;

        public LogEntry(int fromFloor, int toFloor, string description)
        {
            FromFloor = fromFloor;
            ToFloor = toFloor;
            Description = description;
            Time = DateTime.Now;
        }
    }

    // Keeps all MySQL database operations separate from the user interface.
    public static class DbHelper
    {
        private const string ShowSql =
            "SELECT id AS `Log ID`, event_time AS `Date & Time`, " +
            "from_floor AS `From Floor`, to_floor AS `To Floor`, " +
            "description AS `Event Description` " +
            "FROM elevator_log ORDER BY id DESC LIMIT 100";

        private static string GetConnectionString()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "dbconfig.txt");
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    "dbconfig.txt was not found beside ElevatorSystem.exe. Check that the file is in the project and copied to the output folder.");

            // Cleans the text before using it as the MySQL connection string.
            string raw = File.ReadAllText(path).Trim().TrimStart('\uFEFF');
            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("dbconfig.txt is empty.");

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(raw);
            builder.ConnectionTimeout = 5;
            return builder.ConnectionString;
        }

        private static string ServerAddress()
        {
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(GetConnectionString());
                return builder.Server + ":" + builder.Port;
            }
            catch
            {
                return "the server in dbconfig.txt";
            }
        }

        private static string Explain(Exception ex)
        {
            string message = ex.Message;
            if (ex.InnerException != null)
                message += " (" + ex.InnerException.Message + ")";
            return message;
        }

        public static DataTable LoadLogs()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(ShowSql, connection))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not load operation logs from MySQL at " + ServerAddress() + ": " + Explain(ex), ex);
            }
        }

        // Saves the log entries using parameters and keeps the whole batch in one transaction.
        public static void SaveLogs(List<LogEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        const string sql =
                            "INSERT INTO elevator_log (event_time, from_floor, to_floor, description) " +
                            "VALUES (@event_time, @from_floor, @to_floor, @description)";

                        using (MySqlCommand command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.Add("@event_time", MySqlDbType.DateTime);
                            command.Parameters.Add("@from_floor", MySqlDbType.Int32);
                            command.Parameters.Add("@to_floor", MySqlDbType.Int32);
                            command.Parameters.Add("@description", MySqlDbType.VarChar, 255);

                            foreach (LogEntry entry in entries)
                            {
                                command.Parameters["@event_time"].Value = entry.Time;
                                command.Parameters["@from_floor"].Value = entry.FromFloor;
                                command.Parameters["@to_floor"].Value = entry.ToFloor;
                                command.Parameters["@description"].Value =
                                    string.IsNullOrEmpty(entry.Description)
                                        ? "Elevator event"
                                        : (entry.Description.Length > 255 ? entry.Description.Substring(0, 255) : entry.Description);
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save operation logs to MySQL at " + ServerAddress() + ": " + Explain(ex), ex);
            }
        }
    }
}