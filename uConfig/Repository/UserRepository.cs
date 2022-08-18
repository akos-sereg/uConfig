using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Threading.Tasks;
using uConfig.Model;
using uConfig.Services;

namespace uConfig.Repository
{
    public class UserRepository
    {
		private string demoUser = null;
		private string demoJwtToken = null;
		private string connectionString;

		public UserRepository(string connectionString)
        {
			this.demoUser = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Demo")["Username"];
			this.demoJwtToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Demo")["JwtToken"];
			this.connectionString = connectionString;
		}

        public string DemoUser { 
			get {
				return this.demoUser;
            }
		}

        public async Task<LoggedInUser> GetUserByApiKey(string apiKey)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at, email_verified FROM uconfy_user WHERE api_key = ?api_key AND email_verified = 1", connection);
				command.Parameters.Add("?api_key", MySqlDbType.VarChar).Value = apiKey;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
						EmailVerified = reader.GetInt32(reader.GetOrdinal("email_verified")) == 1
					};

					user.Role = user.Email.Equals("demouser@demo.de") ? "demo" : "user";
					user.Token = JwtTokenService.GenerateToken(user.UserID, user.Email, user.ApiKey);

					return user;
				}

				return null;
			}
		}

		public async Task<LoggedInUser> GetUserWithVerifiedEmail(string username, string password)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at, email_verified FROM uconfy_user WHERE email = ?username AND password = MD5(?password) AND email_verified = 1", connection);
				command.Parameters.Add("?username", MySqlDbType.VarChar).Value = username;
				command.Parameters.Add("?password", MySqlDbType.VarChar).Value = password;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
						EmailVerified = reader.GetInt32(reader.GetOrdinal("email_verified")) == 1
					};

					user.Token = JwtTokenService.GenerateToken(user.UserID, user.Email, user.ApiKey);

					return user;
				}

				return null;
			}
		}

		public async Task<LoggedInUser> GetDemoUser()
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at FROM uconfy_user WHERE email = ?username", connection);
				command.Parameters.Add("?username", MySqlDbType.VarChar).Value = this.demoUser;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
						EmailVerified = true
					};

					user.Token = this.demoJwtToken;
					return user;
				}

				return null;
			}
		}

		public async Task<Boolean> IsEmailRegistered(string email)
		{
			int userCount = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();

				// check for existing user
				using var command = new MySqlCommand("SELECT user_id FROM uconfy_user WHERE email = ?email", connection);
				command.Parameters.Add("?email", MySqlDbType.VarChar).Value = email.Trim().ToLower();

				using var reader = await command.ExecuteReaderAsync();

				while (await reader.ReadAsync())
				{
					userCount++;
				}

				await connection.CloseAsync();
			}

			return userCount > 0;
		}

		public async Task RegisterUserWithEmailNotVerified(string email, string password, int registrationCode)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();

				// insert
				var insertCommand = new MySqlCommand("INSERT INTO uconfy_user (user_id, email, password, api_key, registered_at, registration_code, email_verified) VALUES (DEFAULT, ?email, MD5(?password), ?api_key, UTC_TIMESTAMP(), ?registration_code, 0)", connection);
				insertCommand.Parameters.Add("?email", MySqlDbType.VarChar).Value = email.Trim().ToLower();
				insertCommand.Parameters.Add("?password", MySqlDbType.VarChar).Value = password;
				insertCommand.Parameters.Add("?api_key", MySqlDbType.VarChar).Value = Guid.NewGuid().ToString();
				insertCommand.Parameters.Add("?registration_code", MySqlDbType.Int16).Value = registrationCode;
				await insertCommand.ExecuteNonQueryAsync();
			}
		}

		public async Task<LoggedInUser> GetUserByEmail(string email)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();
				using var command = new MySqlCommand("SELECT user_id, email, password, api_key, registered_at, registration_code, email_verified FROM uconfy_user WHERE email = ?username", connection);
				command.Parameters.Add("?username", MySqlDbType.VarChar).Value = email;

				using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					LoggedInUser user = new LoggedInUser()
					{
						ApiKey = reader.GetString(reader.GetOrdinal("api_key")),
						Email = reader.GetString(reader.GetOrdinal("email")),
						UserID = reader.GetInt32(reader.GetOrdinal("user_id")),
						RegistrationCode = reader.GetInt32(reader.GetOrdinal("registration_code")),
						EmailVerified = reader.GetInt32(reader.GetOrdinal("email_verified")) == 1
					};

					user.Token = JwtTokenService.GenerateToken(user.UserID, user.Email, user.ApiKey);

					return user;
				}

				return null;
			}
		}

		public async Task EmailVerified(string email)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();

				// insert
				var insertCommand = new MySqlCommand("UPDATE uconfy_user SET email_verified = 1 WHERE email = ?email", connection);
				insertCommand.Parameters.Add("?email", MySqlDbType.VarChar).Value = email.Trim().ToLower();
				await insertCommand.ExecuteNonQueryAsync();
			}
		}

		public async Task DeleteUserByEmail(string email)
        {
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				await connection.OpenAsync();

				var insertCommand = new MySqlCommand("DELETE FROM uconfy_user WHERE email_verified != 1 AND email = ?email", connection);
				insertCommand.Parameters.Add("?email", MySqlDbType.VarChar).Value = email.Trim().ToLower();
				await insertCommand.ExecuteNonQueryAsync();
			}
		}
	}
}
