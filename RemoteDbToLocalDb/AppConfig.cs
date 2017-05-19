using System.Configuration;

namespace RemoteDbToLocalDb
{
    public static class AppConfig
    {
        public static readonly string LocalConnectionString = ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        public static readonly string RemoteConnectionString = ConfigurationManager.ConnectionStrings["Remote"].ConnectionString;
        public static readonly string Database = ConfigurationManager.AppSettings["Database"];
    }
}
