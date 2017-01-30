using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Loki.MSSQL
{
    public static class MSSQLHelper
    {
        public static async Task<int> ExecuteNonQueryAsync(string connectionString, CommandType cmdType, string cmdText,
       params SqlParameter[] commandParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(cmdText, connection))
                {
                    command.CommandType = cmdType;
                    command.Parameters.AddRange(commandParameters);
                    connection.Open();

                    return await command.ExecuteNonQueryAsync();
                }
            }

        }

        public static async Task<object> ExecuteScalarAsync(string connectionString, CommandType cmdType, string cmdText,
            params SqlParameter[] commandParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(cmdText, connection))
                {
                    command.CommandType = cmdType;
                    command.Parameters.AddRange(commandParameters);
                    connection.Open();

                    return await command.ExecuteScalarAsync();
                }
            }
        }
    }
}