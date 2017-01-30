using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Loki.MSSQL
{
    public class MSSQLLokiLockHandler : LokiLockHandler
    {
        private readonly string _connectionString;

        public MSSQLLokiLockHandler(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override bool Lock(string serviceKey, int expiryFromSeconds)
        {
            bool isLocked = false;
            try
            {
                string selectQuery = "SELECT CreationDate FROM LokiLockings WHERE ServiceKey=@ServiceKey";

                var isAnyLocked = Task.Run(async () => await MSSQLHelper.ExecuteScalarAsync(_connectionString, CommandType.Text, selectQuery, new SqlParameter("ServiceKey", serviceKey))).Result;

                if (isAnyLocked != null)
                {
                    DateTime creationDate = Convert.ToDateTime(isAnyLocked);

                    if (creationDate.AddSeconds(expiryFromSeconds) < DateTime.Now)
                    {
                        string updateQuery = "UPDATE LokiLockings SET CreationDate=@CreationDate WHERE ServiceKey=@ServiceKey";

                        int isUpdated = Task.Run(async () => await MSSQLHelper.ExecuteNonQueryAsync(_connectionString, CommandType.Text, updateQuery, new SqlParameter("ServiceKey", serviceKey))).Result;

                        if (isUpdated > 0)
                        {
                            isLocked = true;
                        }
                    }
                }
                else
                {
                    string insertQuery = "INSERT INTO LokiLockings (ServiceKey, CreationDate) VALUES(@ServiceKey, @CreationDate)";

                    int isInserted = Task.Run(async () => await MSSQLHelper.ExecuteNonQueryAsync(_connectionString, CommandType.Text, insertQuery, new SqlParameter("ServiceKey", serviceKey), new SqlParameter("CreationDate", DateTime.Now))).Result;

                    if (isInserted > 0)
                    {
                        isLocked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isLocked = false;
            }

            return isLocked;
        }

        public override void Release(string serviceKey)
        {
            string deleteQuery = "DELETE FROM LokiLockings WHERE ServiceKey=@ServiceKey";

            Task.Run(async () => await MSSQLHelper.ExecuteNonQueryAsync(_connectionString, CommandType.Text, deleteQuery, new SqlParameter("ServiceKey", serviceKey)));
        }
    }
}