using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uConfig.Model;

namespace uConfig.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private string connectionString;

        public DeviceRepository(MySqlConnection connection)
        {
            this.connectionString = connection.ConnectionString;
        }

        public async Task CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var deleteCommand = new MySqlCommand("DELETE FROM device_config_item WHERE device_id = ?device_id", connection);
                deleteCommand.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId.ToString();
                await deleteCommand.ExecuteNonQueryAsync();

                for (int i=0; i!=deviceConfig.Items.Count; i++)
                {
                    deleteCommand = new MySqlCommand("INSERT INTO device_config_item (device_id, config_key, config_value) VALUES (?device_id, ?config_key, ?config_value)", connection);
                    deleteCommand.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId.ToString();
                    deleteCommand.Parameters.Add("?config_key", MySqlDbType.VarChar).Value = deviceConfig.Items[i].Key;
                    deleteCommand.Parameters.Add("?config_value", MySqlDbType.VarChar).Value = deviceConfig.Items[i].Value;
                    await deleteCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async void DeleteDevice(Guid deviceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var deleteCommand = new MySqlCommand("DELETE FROM device WHERE id = ?id", connection);
                deleteCommand.Parameters.Add("?id", MySqlDbType.VarChar).Value = deviceId.ToString();
                await deleteCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<Device> GetDeviceById(Guid deviceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT id, name, platform, user_id FROM device WHERE id = ?id", connection);
                command.Parameters.Add("?id", System.Data.DbType.Int32).Value = deviceId.ToString();

                using var reader = await command.ExecuteReaderAsync();
                List<Device> devices = new List<Device>();
                while (await reader.ReadAsync())
                {
                    return new Device()
                    {
                        DeviceID = Guid.Parse(reader.GetString(reader.GetOrdinal("id"))),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Platform = reader.GetString(reader.GetOrdinal("platform")),
                        UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
                    };
                }

                return null;
            }
        }

        public async Task<DeviceConfig> GetDeviceConfig(Guid deviceId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT config_key, config_value FROM device_config_item WHERE device_id = ?device_id", connection);
                command.Parameters.Add("?device_id", MySqlDbType.VarChar).Value = deviceId.ToString();

                using var reader = await command.ExecuteReaderAsync();
                DeviceConfig deviceConfig = new DeviceConfig();
                while (await reader.ReadAsync())
                {
                    deviceConfig.Items.Add(new DeviceConfigItem()
                    {
                        Key = reader.GetString(reader.GetOrdinal("config_key")),
                        Value = reader.GetString(reader.GetOrdinal("config_value")),
                    });
                }

                return deviceConfig;
            }
        }

        public async Task<List<Device>> GetDevices(int userId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using var command = new MySqlCommand("SELECT id, name, platform, user_id FROM device WHERE user_id = ?user_id", connection);
                command.Parameters.Add("?user_id", System.Data.DbType.Int32).Value = userId;

                using var reader = await command.ExecuteReaderAsync();
                List<Device> devices = new List<Device>();
                while (await reader.ReadAsync())
                {
                    devices.Add(new Device()
                    {
                        DeviceID = Guid.Parse(reader.GetString(reader.GetOrdinal("id"))),
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        Platform = reader.GetString(reader.GetOrdinal("platform")),
                        UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
                    });
                }

                return devices;
            }
        }

        public async Task<bool> IsDeviceAlreadyRegistered(Device device)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                List<Device> devices = await GetDevices(device.UserID);

                return devices.Any(d => d.Name.Equals(device.Name));
            }
        }

        public async void RegisterDevice(Device device)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                device.DeviceID = Guid.NewGuid();

                var insertCommand = new MySqlCommand("INSERT INTO device (id, name, platform, user_id) VALUES (?id, ?name, ?platform, ?user_id)", connection);
                insertCommand.Parameters.Add("?id", MySqlDbType.VarChar).Value = device.DeviceID.ToString();
                insertCommand.Parameters.Add("?name", MySqlDbType.VarChar).Value = device.Name;
                insertCommand.Parameters.Add("?platform", MySqlDbType.VarChar).Value = device.Platform;
                insertCommand.Parameters.Add("?user_id", MySqlDbType.Int32).Value = device.UserID;
                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async void UpdateDevice(Device device)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var updateCommand = new MySqlCommand("UPDATE device SET name = ?name, platform = ?platform WHERE id = ?id", connection);
                updateCommand.Parameters.Add("?id", MySqlDbType.VarChar).Value = device.DeviceID.ToString();
                updateCommand.Parameters.Add("?name", MySqlDbType.VarChar).Value = device.Name;
                updateCommand.Parameters.Add("?platform", MySqlDbType.VarChar).Value = device.Platform;
                await updateCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
