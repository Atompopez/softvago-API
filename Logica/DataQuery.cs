using Npgsql;
using softvago_API.Models;
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
                                    title = reader.IsDBNull(reader.GetOrdinal("title")) ? string.Empty : reader.GetString(reader.GetOrdinal("title")),
                                    enterprise = reader.IsDBNull(reader.GetOrdinal("enterprise")) ? string.Empty : reader.GetString(reader.GetOrdinal("enterprise")),
                                    urlRedirection = reader.IsDBNull(reader.GetOrdinal("urlRedirection")) ? string.Empty : reader.GetString(reader.GetOrdinal("urlRedirection")),
                                    shortDescription = reader.IsDBNull(reader.GetOrdinal("shortDescription")) ? string.Empty : reader.GetString(reader.GetOrdinal("shortDescription")),
                                    wage = reader.IsDBNull(reader.GetOrdinal("wage")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("wage")),
                                    idLocation = reader.IsDBNull(reader.GetOrdinal("idLocation")) ? 0 : reader.GetInt32(reader.GetOrdinal("idLocation")),
                                    idModality = reader.IsDBNull(reader.GetOrdinal("idModality")) ? 0 : reader.GetInt32(reader.GetOrdinal("idModality")),
                                    clicks = reader.IsDBNull(reader.GetOrdinal("clicks")) ? 0 : reader.GetInt32(reader.GetOrdinal("clicks"))
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
    }
}