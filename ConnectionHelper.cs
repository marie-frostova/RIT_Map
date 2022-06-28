using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MapApp
{
    public static class ConnectionHelper
    {
        public static int Trials = 3;

        public readonly static string ConnectionString;

        static ConnectionHelper()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
        
        public static void EstablishConnection(ref SqlConnection Connection)
        {
            for (int i = 0; i < Trials; i++)
            {
                if (Connection == null || Connection.State != ConnectionState.Open)
                {
                    try
                    {
                        Connection = new SqlConnection(ConnectionString);
                        Connection.Open();
                        return;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    throw new Exception("Failed to connect to the database");
                }
            }
        }
    }
}
