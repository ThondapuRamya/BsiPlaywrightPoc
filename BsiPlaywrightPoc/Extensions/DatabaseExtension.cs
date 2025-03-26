using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace BsiPlaywrightPoc.Extensions
{
    public static class DatabaseExtension
    {
        // Ensure you inject the logger or pass it as a parameter if needed
        public static DataTable? SqlExecute(this string connectionString, string sql, ILogger logger)
        {
            try
            {
                var results = new DataTable();

                using (var con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandTimeout = 90;

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                logger.LogWarning("No rows found for the executed SQL query.");
                                return null;
                            }

                            results.Load(reader); // Load all data into the DataTable
                        }
                    }
                }

                return results;
            }
            catch (SqlException sqlEx)
            {
                logger.LogError(sqlEx, "An SQL exception occurred while executing the query.");
                throw; // Re-throw the exception to be handled at a higher level
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while executing the query.");
                throw; // Re-throw the exception to be handled at a higher level
            }
        }
    }
}