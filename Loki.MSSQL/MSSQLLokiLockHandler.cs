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

        public override bool Lock(string tenantType, int expiryFromSeconds)
        {
            bool isLocked = false;
            try
            {
                string selectQuery = "SELECT CreationDate FROM LokiLockings WHERE TenantType=@TenantType";

                var isAnyLocked = Task.Run(async () => await MSSQLHelper.ExecuteScalarAsync(_connectionString, CommandType.Text, selectQuery, new SqlParameter("TenantType", tenantType))).Result;

                if (isAnyLocked != null)
                {
                    DateTime creationDate = Convert.ToDateTime(isAnyLocked);

                    if (creationDate.AddSeconds(expiryFromSeconds) < DateTime.Now)
                    {
                        string updateQuery = "UPDATE LokiLockings SET CreationDate=@CreationDate WHERE TenantType=@TenantType";

                        int isUpdated = Task.Run(async () => await MSSQLHelper.ExecuteNonQueryAsync(_connectionString, CommandType.Text, updateQuery, new SqlParameter("TenantType", tenantType))).Result;

                        if (isUpdated > 0)
                        {
                            isLocked = true;
                        }
                    }
                }
                else
                {
                    string insertQuery = "INSERT INTO LokiLockings (TenantType, CreationDate) VALUES(@TenantType, @CreationDate)";

                    int isInserted = Task.Run(async () => await MSSQLHelper.ExecuteNonQueryAsync(_connectionString, CommandType.Text, insertQuery, new SqlParameter("TenantType", tenantType), new SqlParameter("CreationDate", DateTime.Now))).Result;

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

        public override void Release(string tenantType)
        {
            string deleteQuery = "DELETE FROM LokiLockings WHERE TenantType=@TenantType";

            Task.Run(async () => await MSSQLHelper.ExecuteNonQueryAsync(_connectionString, CommandType.Text, deleteQuery, new SqlParameter("TenantType", tenantType)));
        }
    }
}