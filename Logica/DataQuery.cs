using Npgsql;
using softvago_API.Models;
using Microsoft.Extensions.Configuration;

namespace softvago_API.Logica
{
    public class DataQuery
    {
        private readonly string? _connectionString;

        public DataQuery()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = configuration.GetConnectionString("Office");
        }

        private async Task<T> ExecuteQueryAsync<T>(string sql, Func<NpgsqlDataReader, T> readFunc, params NpgsqlParameter[] parameters)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var results = new List<T>();
                        while (await reader.ReadAsync())
                        {
                            results.Add(readFunc(reader));
                        }
                        return results;
                    }
                }
            }
        }

        private async Task<int> ExecuteNonQueryAsync(string sql, params NpgsqlParameter[] parameters)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters);
                    return await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<Api>> GetApis()
        {
            const string sql = "SELECT * FROM softvago_test.apis";
            return await ExecuteQueryAsync(reader => new Api
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                apiName = reader.GetString(reader.GetOrdinal("apiName")),
                baseUrl = reader.GetString(reader.GetOrdinal("baseUrl")),
                description = reader.GetString(reader.GetOrdinal("description")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, sql);
        }

        public async Task<int> UpdateApi(Api apiToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.apis WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.apis SET description = @Description, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", apiToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Description", apiToUpdate.description),
                new NpgsqlParameter("@Enable", apiToUpdate.enable),
                new NpgsqlParameter("@Id", apiToUpdate.id));
        }

        public async Task<List<Job>> GetJobs()
        {
            const string sql = "SELECT * FROM softvago_test.jobs";
            return await ExecuteQueryAsync(reader => new Job
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                title = reader.GetString(reader.GetOrdinal("title")),
                enterprise = reader.GetString(reader.GetOrdinal("enterprise")),
                urlRedirection = reader.GetString(reader.GetOrdinal("urlRedirection")),
                shortDescription = reader.GetString(reader.GetOrdinal("shortDescription")),
                wage = reader.GetDouble(reader.GetOrdinal("wage")),
                idLocation = reader.GetInt32(reader.GetOrdinal("idLocation")),
                idModality = reader.GetInt32(reader.GetOrdinal("idModality")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, sql);
        }

        public async Task<int> UpdateJob(Job jobToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.jobs WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.jobs SET enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", jobToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Enable", jobToUpdate.enable),
                new NpgsqlParameter("@Id", jobToUpdate.id));
        }

        public async Task<int> AddClickJob(int idJob)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.jobs WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.jobs SET click = click + 1 WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", idJob));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql, new NpgsqlParameter("@Id", idJob));
        }

        public async Task<List<Location>> GetLocations()
        {
            const string sql = "SELECT * FROM softvago_test.locations";
            return await ExecuteQueryAsync(reader => new Location
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                city = reader.GetString(reader.GetOrdinal("city")),
                country = reader.GetString(reader.GetOrdinal("country")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, sql);
        }

        public async Task<List<Modality>> GetModality()
        {
            const string sql = "SELECT * FROM softvago_test.modality";
            return await ExecuteQueryAsync(reader => new Modality
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                description = reader.GetString(reader.GetOrdinal("description")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, sql);
        }

        public async Task<int> UpdateModality(Modality modalityToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.modality WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.modality SET name = @Name, description = @Description, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", modalityToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Name", modalityToUpdate.name),
                new NpgsqlParameter("@Description", modalityToUpdate.description),
                new NpgsqlParameter("@Enable", modalityToUpdate.enable),
                new NpgsqlParameter("@Id", modalityToUpdate.id));
        }

        public async Task<List<Rol>> GetRoles()
        {
            const string sql = "SELECT * FROM softvago_test.rol WHERE enable = B'1'";
            return await ExecuteQueryAsync(reader => new Rol
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                description = reader.GetString(reader.GetOrdinal("description")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, sql);
        }

        public async Task<List<User>> GetUsers()
        {
            const string sql = "SELECT * FROM softvago_test.users WHERE enable = B'1'";
            return await ExecuteQueryAsync(reader => new User
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                lastName = reader.GetString(reader.GetOrdinal("lastName")),
                email = reader.GetString(reader.GetOrdinal("email")),
                password = reader.GetString(reader.GetOrdinal("password")),
                registrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate")).ToString("yyyy-MM-dd"),
                idRol = reader.GetInt32(reader.GetOrdinal("idRol")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, sql);
        }

        public async Task<int> UpdateUser(User userToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.user WHERE id = @Id AND enable = B'1'";
            const string updateSql = "UPDATE softvago_test.apis SET name = @Name, lastName = @LastName, email = @Email, password = @Password, idRol = @IdRol, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", userToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Name", userToUpdate.name),
                new NpgsqlParameter("@LastName", userToUpdate.lastName),
                new NpgsqlParameter("@Email", userToUpdate.email),
                new NpgsqlParameter("@Password", userToUpdate.password),
                new NpgsqlParameter("@IdRol", userToUpdate.idRol),
                new NpgsqlParameter("@Enable", userToUpdate.enable),
                new NpgsqlParameter("@Id", userToUpdate.id));
        }
    }
}