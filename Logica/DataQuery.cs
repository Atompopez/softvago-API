using Npgsql;
using softvago_API.Models;
using System.Data;
using System.Data.SqlClient;

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

        public async Task<List<Api>> GetApis()
        {
            var apis = new List<Api>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT * FROM softvago_test.apis
                                WHERE enable = B'1'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        //cmd.Parameters.AddWithValue("@documento", idConsulta);
                        using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var api = new Api
                                {
                                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                    apiName = reader.IsDBNull(reader.GetOrdinal("apiName")) ? string.Empty : reader.GetString(reader.GetOrdinal("apiName")),
                                    baseUrl = reader.IsDBNull(reader.GetOrdinal("baseUrl")) ? string.Empty : reader.GetString(reader.GetOrdinal("baseUrl")),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description")),
                                    enable = reader.IsDBNull(reader.GetOrdinal("enable")) ? false : reader.GetBoolean(reader.GetOrdinal("enable"))
                                };

                                apis.Add(api);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return apis;
        }

        public async Task<List<Job>> GetJobs()
        {
            var jobs = new List<Job>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT * FROM softvago_test.jobs
                                WHERE enable = B'1'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        //cmd.Parameters.AddWithValue("@documento", idConsulta);
                        using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var job = new Job
                                {
                                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                    title = reader.IsDBNull(reader.GetOrdinal("title")) ? string.Empty : reader.GetString(reader.GetOrdinal("title")),
                                    enterprise = reader.IsDBNull(reader.GetOrdinal("enterprise")) ? string.Empty : reader.GetString(reader.GetOrdinal("enterprise")),
                                    urlRedirection = reader.IsDBNull(reader.GetOrdinal("urlRedirection")) ? string.Empty : reader.GetString(reader.GetOrdinal("urlRedirection")),
                                    shortDescription = reader.IsDBNull(reader.GetOrdinal("shortDescription")) ? string.Empty : reader.GetString(reader.GetOrdinal("shortDescription")),
                                    wage = reader.IsDBNull(reader.GetOrdinal("wage")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("wage")),
                                    idLocation = reader.IsDBNull(reader.GetOrdinal("idLocation")) ? 0 : reader.GetInt32(reader.GetOrdinal("idLocation")),
                                    idModality = reader.IsDBNull(reader.GetOrdinal("idModality")) ? 0 : reader.GetInt32(reader.GetOrdinal("idModality")),
                                    enable = reader.IsDBNull(reader.GetOrdinal("enable")) ? false : reader.GetBoolean(reader.GetOrdinal("enable"))
                                };

                                jobs.Add(job);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return jobs;
        }

        public async Task<List<Location>> GetLocations()
        {
            var locations = new List<Location>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT * FROM softvago_test.locations
                                WHERE enable = B'1'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        //cmd.Parameters.AddWithValue("@documento", idConsulta);
                        using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var location = new Location
                                {
                                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                    city = reader.IsDBNull(reader.GetOrdinal("city")) ? string.Empty : reader.GetString(reader.GetOrdinal("city")),
                                    country = reader.IsDBNull(reader.GetOrdinal("country")) ? string.Empty : reader.GetString(reader.GetOrdinal("country")),
                                    enable = reader.IsDBNull(reader.GetOrdinal("enable")) ? false : reader.GetBoolean(reader.GetOrdinal("enable"))
                                };

                                locations.Add(location);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return locations;
        }

        public async Task<List<Modality>> GetModality()
        {
            var modalities = new List<Modality>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT * FROM softvago_test.modality
                                WHERE enable = B'1'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        //cmd.Parameters.AddWithValue("@documento", idConsulta);
                        using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var modality = new Modality
                                {
                                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description")),
                                    enable = reader.IsDBNull(reader.GetOrdinal("enable")) ? false : reader.GetBoolean(reader.GetOrdinal("enable"))
                                };

                                modalities.Add(modality);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return modalities;
        }

        public async Task<List<Rol>> GetRoles()
        {
            var Roles = new List<Rol>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT * FROM softvago_test.rol
                                WHERE enable = B'1'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        //cmd.Parameters.AddWithValue("@documento", idConsulta);
                        using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var rol = new Rol
                                {
                                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                                    description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString(reader.GetOrdinal("description")),
                                    enable = reader.IsDBNull(reader.GetOrdinal("enable")) ? false : reader.GetBoolean(reader.GetOrdinal("enable"))
                                };

                                Roles.Add(rol);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Roles;
        }

        public async Task<List<User>> GetUsers()
        {
            var Users = new List<User>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    string sql = @"SELECT * FROM softvago_test.users
                                WHERE enable = B'1'";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                    {
                        //cmd.Parameters.AddWithValue("@documento", idConsulta);
                        using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var user = new User
                                {
                                    id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                                    lastName = reader.IsDBNull(reader.GetOrdinal("lastName")) ? string.Empty : reader.GetString(reader.GetOrdinal("lastName")),
                                    email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                                    password = reader.IsDBNull(reader.GetOrdinal("password")) ? string.Empty : reader.GetString(reader.GetOrdinal("password")),
                                    registrationDate = reader.IsDBNull(reader.GetOrdinal("registrationDate")) ? string.Empty : reader.GetDateTime(reader.GetOrdinal("registrationDate")).ToString("yyyy-MM-dd"),
                                    idRol = reader.IsDBNull(reader.GetOrdinal("idRol")) ? 0 : reader.GetInt32(reader.GetOrdinal("idRol")),
                                    enable = reader.IsDBNull(reader.GetOrdinal("enable")) ? false : reader.GetBoolean(reader.GetOrdinal("enable"))
                                };

                                Users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Users;
        }
    }
}