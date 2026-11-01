using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace OnlineQuizASPNet.Models
{
    public static class DbHelper
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["QuizDbConn"]?.ConnectionString 
            ?? "Server=localhost;Database=QLThiTracNghiem;Integrated Security=True;";

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
