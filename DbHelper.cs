using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace dbProject
{
    public static class DbHelper
    {
        // Keep your existing working credentials here.
        private static string connStr =
            "User Id=ABEER_24F0762;Password=abeer1405;Data Source=localhost:1521/xe;";

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(connStr);
        }

        /// <summary>Runs a SELECT (or view query) and returns the results as a DataTable.</summary>
        public static DataTable ExecuteQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using (var adapter = new OracleDataAdapter(cmd))
                {
                    var table = new DataTable();
                    conn.Open();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        /// <summary>Runs an INSERT/UPDATE/DELETE and returns rows affected.</summary>
        public static int ExecuteNonQuery(string sql, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Calls a stored procedure by name with the given parameters.
        /// Throws the underlying OracleException on failure (RAISE_APPLICATION_ERROR
        /// inside the procedure surfaces here as ex.Message), so callers should catch it.
        /// </summary>
        public static void ExecuteProcedure(string procName, params OracleParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new OracleCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static OracleParameter P(string name, object value)
        {
            return new OracleParameter(name, value ?? DBNull.Value);
        }

        /// <summary>Suggests the next free numeric ID for tables with manually-assigned PKs.</summary>
        public static int NextId(string table, string idColumn)
        {
            var dt = ExecuteQuery($"SELECT NVL(MAX({idColumn}),0)+1 AS NXT FROM {table}");
            return Convert.ToInt32(dt.Rows[0]["NXT"]);
        }

        /// <summary>
        /// True if a row exists matching keyColumn = keyValue. Used to confirm whether a stored
        /// procedure call actually succeeded, since these procedures swallow their own exceptions
        /// (WHEN OTHERS THEN ROLLBACK; DBMS_OUTPUT...) instead of raising them back to the caller.
        /// </summary>
        public static bool RowExists(string table, string keyColumn, object keyValue)
        {
            var dt = ExecuteQuery($"SELECT 1 AS X FROM {table} WHERE {keyColumn} = :k", P("k", keyValue));
            return dt.Rows.Count > 0;
        }
    }
}
