using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using softvago_API.Models;
using System.Text;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
                            lastName = reader.GetString(reader.GetOrdinal("lastname")),
                            email = reader.GetString(reader.GetOrdinal("email")),
                            registrationDate = reader.GetDateTime(reader.GetOrdinal("registration_date")).ToString("yyyy-MM-dd"),
                            idRol = reader.GetInt32(reader.GetOrdinal("id_rol")),
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
                apiName = reader.GetString(reader.GetOrdinal("api_name")),
                baseUrl = reader.GetString(reader.GetOrdinal("base_url")),
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

        public async Task<List<Job>> GetJobs(JobSearchParameters searchParameters, bool full = false)
        {
            var conditions = new List<string>();
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrEmpty(searchParameters.Keywords))
            {
                conditions.Add("title ILIKE @Title");
                parameters.Add(new NpgsqlParameter("@Title", $"%{searchParameters.Keywords}%"));
            }

            if (!string.IsNullOrEmpty(searchParameters.Location))
            {
                conditions.Add("location ILIKE @Location");
                parameters.Add(new NpgsqlParameter("@Location", $"%{searchParameters.Location}%"));
            }

            if (searchParameters.MinSalary.HasValue)
            {
                conditions.Add("wage >= @MinWage");
                parameters.Add(new NpgsqlParameter("@MinWage", searchParameters.MinSalary.Value));
            }

            if (searchParameters.MaxSalary.HasValue)
            {
                conditions.Add("wage <= @MaxWage");
                parameters.Add(new NpgsqlParameter("@MaxWage", searchParameters.MaxSalary.Value));
            }

            if (searchParameters.IdModality.HasValue)
            {
                conditions.Add("id_modality = @IdModality");
                parameters.Add(new NpgsqlParameter("@IdModality", searchParameters.IdModality.Value));
            }

            if (!full)
            {
                conditions.Add("enable = B'1'");
            }

            string whereClause = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : "";

            string sql = $"SELECT * FROM softvago_test.jobs {whereClause}";

            return await ExecuteQueryAsync(sql, reader => new Job
            {
                id = reader.GetInt32(reader.GetOrdinal("id")),
                title = reader.GetString(reader.GetOrdinal("title")),
                enterprise = reader.GetString(reader.GetOrdinal("enterprise")),
                urlRedirection = reader.GetString(reader.GetOrdinal("url_redirection")),
                shortDescription = reader.GetString(reader.GetOrdinal("short_description")),
                wage = reader.GetDouble(reader.GetOrdinal("wage")),
                location = reader.GetString(reader.GetOrdinal("location")),
                idModality = reader.GetInt32(reader.GetOrdinal("id_modality")),
                clicks = reader.GetInt32(reader.GetOrdinal("clicks")),
                enable = reader.GetBoolean(reader.GetOrdinal("enable"))
            }, parameters.ToArray());
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
            const string updateSql = "UPDATE softvago_test.jobs SET clicks = clicks + 1 WHERE id = @Id";

            var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", idJob));
            if (count == 0) return 0;

            return await ExecuteNonQueryAsync(updateSql, new NpgsqlParameter("@Id", idJob));
        }

        public async Task<int> InsertJobs(List<Job> jobs)
        {
            const string checkExistSql = "SELECT COUNT(1) FROM softvago_test.jobs WHERE short_description = @ShortDescription";
            const string insertSql = @"INSERT INTO softvago_test.jobs (title, enterprise, url_redirection, short_description, wage, id_modality, location, clicks, enable) VALUES (@Title, @Enterprise, @UrlRedirection, @ShortDescription, @Wage, @IdModality, @Location, @Clicks, @Enable)";

            int rowsAffected = 0;

            try
            {
                foreach (var job in jobs)
                {
                    var exists = await ExecuteNonQueryAsync(checkExistSql, new NpgsqlParameter("@ShortDescription", job.shortDescription)) > 0;

                    if (!exists)
                    {
                        var enterprise = string.IsNullOrEmpty(job.enterprise) ? "Anonimo" : job.enterprise;

                        rowsAffected += await ExecuteNonQueryAsync(insertSql,
                            new NpgsqlParameter("@Title", job.title),
                            new NpgsqlParameter("@Enterprise", enterprise),
                            new NpgsqlParameter("@UrlRedirection", job.urlRedirection),
                            new NpgsqlParameter("@ShortDescription", job.shortDescription),
                            new NpgsqlParameter("@Wage", job.wage),
                            new NpgsqlParameter("@IdModality", job.idModality),
                            new NpgsqlParameter("@Location", job.location),
                            new NpgsqlParameter("@Clicks", job.clicks),
                            new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit) { Value = job.enable }
                        );
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return rowsAffected;
        }

        //public async Task<List<Location>> GetLocations()
        //{
        //    const string sql = "SELECT * FROM softvago_test.locations";
        //    return await ExecuteQueryAsync(sql, reader => new Location
        //    {
        //        id = reader.GetInt32(reader.GetOrdinal("id")),
        //        city = reader.GetString(reader.GetOrdinal("city")),
        //        country = reader.GetString(reader.GetOrdinal("country")),
        //        enable = reader.GetBoolean(reader.GetOrdinal("enable"))
        //    });
        //}

        //public async Task<int> UpdateLocation(Location locationToUpdate)
        //{
        //    const string selectSql = "SELECT COUNT(*) FROM softvago_test.locations WHERE id = @Id";
        //    const string updateSql = "UPDATE softvago_test.locations SET city = @City, country = @Country, enable = @Enable WHERE id = @Id";

        //    var count = (int)await ExecuteNonQueryAsync(selectSql, new NpgsqlParameter("@Id", locationToUpdate.id));
        //    if (count == 0) return 0;

        //    return await ExecuteNonQueryAsync(updateSql,
        //        new NpgsqlParameter("@City", locationToUpdate.city),
        //        new NpgsqlParameter("@Country", locationToUpdate.country),
        //        new NpgsqlParameter("@Enable", NpgsqlTypes.NpgsqlDbType.Bit)
        //        {
        //            Value = locationToUpdate.enable
        //        },
        //        new NpgsqlParameter("@Id", locationToUpdate.id));
        //}

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
            const string updateSql = "UPDATE softvago_test.rol SET description = @Description, enable = @Enable WHERE id = @Id";

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
                lastName = reader.GetString(reader.GetOrdinal("lastname")),
                email = reader.GetString(reader.GetOrdinal("email")),
                registrationDate = reader.GetDateTime(reader.GetOrdinal("registration_date")).ToString("yyyy-MM-dd"),
                idRol = reader.GetInt32(reader.GetOrdinal("id_rol")),
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
            const string selectSql = "SELECT COUNT(*) FROM softvago_test.users WHERE id = @Id";
            const string updateSql = "UPDATE softvago_test.users SET name = @Name, lastname = @LastName, email = @Email, password = @Password, username = @username, id_rol = @IdRol, enable = @Enable WHERE id = @Id";

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
            const string insertSql = @"INSERT INTO softvago_test.users (name, lastname, email, password, username, registration_date, id_rol, enable) VALUES (@Name, @LastName, @Email, @Password, @Username, @RegistrationDate, @IdRol, @Enable)";

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