using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Cstieg.Sales.Test
{
    public static class LocalDb
    {
        public const string DB_DIRECTORY = "data";

        public static SqlConnection GetLocalDb(string dbName, bool deleteIfExists = false)
        {
            try
            {
                string outputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DB_DIRECTORY);
                string mdfFileName = dbName + ".mdf";
                string dbFileName = Path.Combine(outputFolder, mdfFileName);
                string logFileName = Path.Combine(outputFolder, String.Format("{0}_log.ldf", dbName));
                // Create data directory if it doesn't already exist
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // if the file exists, and we want to delete old data, remove it here and create a new database
                if (File.Exists(dbFileName) && deleteIfExists)
                {
                    if (File.Exists(logFileName))
                    {
                        File.Delete(logFileName);
                    }
                    File.Delete(dbFileName);
                    CreateDatabase(dbName, dbFileName);
                }

                // if the database does not already exist, create it
                else if (!File.Exists(dbFileName))
                {
                    CreateDatabase(dbName, dbFileName);
                }

                // Open newly created, or old database
                string connectionString = String.Format(@"Data Source=(LocalDb)\MSSQLLocalDB;AttachDBFileName={1};Initial Catalog={0};Integrated Security=True;", dbName, dbFileName);
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool CreateDatabase(string dbName, string dbFileName)
        {
            try
            {
                string connectionString = String.Format(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();

                    DetachDatabase(dbName);

                    cmd.CommandText = String.Format("CREATE DATABASE {0} ON (Name = N'{0}', FILENAME = '{1}')", dbName, dbFileName);
                    cmd.ExecuteNonQuery();
                }

                return (File.Exists(dbFileName));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool DetachDatabase(string dbName)
        {
            try
            {
                string connectionString = String.Format(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = String.Format("exec sp_detach_db'{0}'", dbName);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
