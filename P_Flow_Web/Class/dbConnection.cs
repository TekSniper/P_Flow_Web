using Npgsql;

namespace P_Flow_Web.Class
{
    public class dbConnection
    {
        private string _connectionString { get; set; } 
        private NpgsqlConnection _connection { get; set; }
        IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        public dbConnection()
        {
            this._connectionString = GetConfiguration().GetSection("ConnectionStrings").GetSection("CR€@tIv€").Value!;
            this._connection = new NpgsqlConnection(this._connectionString);
        }

        public NpgsqlConnection GetConnection()
        {
            return this._connection;
        }
    }
}
