using Npgsql;
using softvago_API.Models;

namespace softvago_API.Logica
{
    public class DataQuery
    {
        private readonly string? _connectionString;
        private readonly Utils _utils = new Utils();

        public DataQuery()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = configuration.GetConnectionString("Office");
        }

        private async Task<List<T>> ExecuteQueryAsync<T>(string sql, Func<NpgsqlDataReader, T> readFunc, params NpgsqlParameter[] parameters)
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
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddRange(parameters);
                        if (sql.TrimStart().StartsWith("SELECT COUNT", StringComparison.OrdinalIgnoreCase))
                        {
                            var result = await cmd.ExecuteScalarAsync();
                            return (int)Convert.ToInt32(result);
                        }
                        else
                        {
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            return rowsAffected;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<dynamic> Authenticate(Login loginCredentials)
        {
            const string sql = "SELECT * FROM softvago_test.users WHERE username = @Username";

            var user = await ExecuteQueryAsync(sql, reader => new User
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            name = reader.GetString(reader.GetOrdinal("name")),
                            lastName = reader.GetString(reader.GetOrdinal("lastName")),
                            email = reader.GetString(reader.GetOrdinal("email")),
                            registrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate")).ToString("yyyy-MM-dd"),
                            idRol = reader.GetInt32(reader.GetOrdinal("idRol")),
                            enable = reader.GetBoolean(reader.GetOrdinal("enable")),
                            login = new Login
                            {
                                username = reader.GetString(reader.GetOrdinal("username")),
                                password = reader.GetString(reader.GetOrdinal("password"))
                            }
                        },
                        new NpgsqlParameter("@Username", loginCredentials.username));

            if (user is not null && user.Count > 0)
            {
                string hashedInputPassword = _utils.HashGenerator(loginCredentials.password);

                if (hashedInputPassword == user[0].login.password)
                {
                    user[0].login = null;
                    return new
                    {
                        success = true,
                        data = user[0]
                    };
                }
            }

            return false;
        }

        public async Task<List<Api>> GetApis()
        {
            const string sql = "SELECT * FROM softvago_test.apis";
            return await ExecuteQueryAsync(sql, reader => new Api
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                apiName = reader.GetString(reader.GetOrdinal("apiName")),
                baseUrl = reader.GetString(reader.GetOrdinal("baseUrl")),
                description = reader.GetString(reader.GetOrdinal("description")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            });
        }

        public async Task<int> UpdateApi(Api apiToUpdate)
        {
            try
            {
                const string selectSql = "SELECT COUNT(*) FROM softvago_test.apis WHERE id = @Id";
                const string updateSql = "UPDATE softvago_test.apis SET description = @Description, enable = @Enable WHERE id = @Id";

                var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", apiToUpdate.id));
                if (count == 0) return 0;

                return await ExecuteNonQueryAsync(updateSql,
                    new NpgsqlParameter("@Description", apiToUpdate.description),
                    new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                    {
                        Value = apiToUpdate.enable
                    },
                    new NpgsqlParameter("@Id", apiToUpdate.id));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<Job>> GetJobs()
        {
            const string sql = "SELECT * FROM softvago_test.jobs";
            return await ExecuteQueryAsync(sql, reader => new Job
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
            });
        }

        public async Task<int> UpdateJob(Job jobToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.jobs WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.jobs SET enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", jobToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                 new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                 {
                     Value = jobToUpdate.enable
                 },
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
            return await ExecuteQueryAsync(sql, reader => new Location
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                city = reader.GetString(reader.GetOrdinal("city")),
                country = reader.GetString(reader.GetOrdinal("country")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            });
        }

        public async Task<int> UpdateLocation(Location locationToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.locations WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.locations SET city = @City, country = @Country, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", locationToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@City", locationToUpdate.city),
                new NpgsqlParameter("@Country", locationToUpdate.country),
                new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                {
                    Value = locationToUpdate.enable
                },
                new NpgsqlParameter("@Id", locationToUpdate.id));
        }

        public async Task<List<Modality>> GetModality()
        {
            const string sql = "SELECT * FROM softvago_test.modality";
            return await ExecuteQueryAsync(sql, reader => new Modality
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                description = reader.GetString(reader.GetOrdinal("description")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            });
        }

        public async Task<int> UpdateModality(Modality modalityToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.modality WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.modality SET description = @Description, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", modalityToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Description", modalityToUpdate.description),
                new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                {
                    Value = modalityToUpdate.enable
                },
                new NpgsqlParameter("@Id", modalityToUpdate.id));
        }

        public async Task<List<Rol>> GetRoles()
        {
            const string sql = "SELECT * FROM softvago_test.rol";
            return await ExecuteQueryAsync(sql, reader => new Rol
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                description = reader.GetString(reader.GetOrdinal("description")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            });
        }

        public async Task<int> UpdateRol(Rol rolToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.rol WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.rol SET name = @Name, description = @Description, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", rolToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Name", rolToUpdate.name),
                new NpgsqlParameter("@Description", rolToUpdate.description),
                new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                {
                    Value = rolToUpdate.enable
                },
                new NpgsqlParameter("@Id", rolToUpdate.id));
        }

        public async Task<List<User>> GetUsers()
        {
            const string sql = "SELECT * FROM softvago_test.users";
            return await ExecuteQueryAsync(sql, reader => new User
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                name = reader.GetString(reader.GetOrdinal("name")),
                lastName = reader.GetString(reader.GetOrdinal("lastName")),
                email = reader.GetString(reader.GetOrdinal("email")),
                registrationDate = reader.GetDateTime(reader.GetOrdinal("registrationDate")).ToString("yyyy-MM-dd"),
                idRol = reader.GetInt32(reader.GetOrdinal("idRol")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable")),
                login = new Login
                {
                    username = reader.GetString(reader.GetOrdinal("username")),
                    password = reader.GetString(reader.GetOrdinal("password"))
                }
            });
        }

        public async Task<int> UpdateUser(User userToUpdate)
        {
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.user WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.apis SET name = @Name, lastName = @LastName, email = @Email, password = @Password, username = @username, idRol = @IdRol, enable = @Enable WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", userToUpdate.id));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql,
                new NpgsqlParameter("@Name", userToUpdate.name),
                new NpgsqlParameter("@LastName", userToUpdate.lastName),
                new NpgsqlParameter("@Email", userToUpdate.email),
                new NpgsqlParameter("@Password", userToUpdate.login.password),
                new NpgsqlParameter("@username", userToUpdate.login.username),
                new NpgsqlParameter("@IdRol", userToUpdate.idRol),
                new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                {
                    Value = userToUpdate.enable
                },
                new NpgsqlParameter("@Id", userToUpdate.id));
        }

        public async Task<int> InsertUser(User newUser)
        {
            const string insertSql = @"INSERT INTO softvago_test.users (name, lastName, email, password, username, registrationDate, idRol, enable) 
                               VALUES (@Name, @LastName, @Email, @Password, @Username, @RegistrationDate, @IdRol, @Enable)";

            return await ExecuteNonQueryAsync(insertSql,
                new NpgsqlParameter("@Name", newUser.name),
                new NpgsqlParameter("@LastName", newUser.lastName),
                new NpgsqlParameter("@Email", newUser.email),
                new NpgsqlParameter("@Password", newUser.login.password),
                new NpgsqlParameter("@Username", newUser.login.username),
                new NpgsqlParameter("@RegistrationDate", DateTime.UtcNow),
                new NpgsqlParameter("@IdRol", newUser.idRol),
                new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
                {
                    Value = newUser.enable
                }
            );
        }
    }
}