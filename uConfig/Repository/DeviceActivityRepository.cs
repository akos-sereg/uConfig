using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace uConfig.Repository
{
    public class DeviceActivityRepository : IDeviceActivityRepository
    {
        private string connectionString;
        private static Dictionary<string, DateTime> _deviceLastSeen = new Dictionary<string, DateTime>();
        private static Dictionary<string, List<string>> _deviceLogs = new Dictionary<string, List<string>>();

        public DeviceActivityRepository(MySqlConnection connection)
        {
            connectionString = connection.ConnectionString;
        }

        public async Task RegisterDeviceCheckin(string deviceId, string endpoint)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var insertCommand = new MySqlCommand("INSERT INTO device_activity (device_id, endpoint, is_success, inserted_at) VALUES (?device_id, ?endpoint, ?is_success, UTC_TIMESTAMP())", connection);
                insertCommand.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId.ToString();
                insertCommand.Parameters.Add("?endpoint", MySqlDbType.VarChar).Value = endpoint;
                insertCommand.Parameters.Add("?is_success", MySqlDbType.Int32).Value = 1;
                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<DateTime?> GetLastSeen(string deviceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DateTime? result = null;
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT inserted_at FROM device_activity WHERE device_id = ?device_id ORDER BY inserted_at DESC LIMIT 0,1", connection);
                command.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId;

                using var reader = await command.ExecuteReaderAsync();
                List<string> logs = new List<string>();
                while (await reader.ReadAsync())
                {
                    result = reader.GetDateTime(reader.GetOrdinal("inserted_at"));
                }

                return result;
            }
        }

        public async Task AppendLogs(string deviceId, string messages)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string[] lines = messages.Split("\n");
                for (int i = 0; i != lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i]))
                    {
                        continue;
                    }

                    var insertCommand = new MySqlCommand("INSERT INTO device_log (device_id, log_message, inserted_at) VALUES (?device_id, ?log_message, UTC_TIMESTAMP())", connection);
                    insertCommand.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId.ToString();
                    insertCommand.Parameters.Add("?log_message", MySqlDbType.VarChar).Value = lines[i];
                    await insertCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<string>> GetLogs(string deviceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT log_message, inserted_at FROM device_log WHERE device_id = ?device_id ORDER BY inserted_at DESC LIMIT 0,150", connection);
                command.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId;

                using var reader = await command.ExecuteReaderAsync();
                List<string> logs = new List<string>();
                while (await reader.ReadAsync())
                {
                    logs.Add("[" + reader.GetDateTime(reader.GetOrdinal("inserted_at")) + "] " + reader.GetString(reader.GetOrdinal("log_message")));
                }
                logs.Reverse();
                return logs;
            }
        }
    }
}
