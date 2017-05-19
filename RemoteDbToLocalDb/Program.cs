using System;
using System.Data.SqlLocalDb;
using System.IO;
using Microsoft.SqlServer.Dac;

namespace RemoteDbToLocalDb
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Execute();
                Console.WriteLine("Success!!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error...");
                Console.WriteLine(ex.Message, ex);
            }
            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
        }

        private static void Execute()
        {
            var localInfo = SqlLocalDbApi.GetInstanceInfo(AppConfig.Database);
            if (localInfo.Exists)
            {
                SqlLocalDbApi.StopInstance(localInfo.Name);
                SqlLocalDbApi.DeleteInstance(localInfo.Name, true);
            }
            SqlLocalDbApi.CreateInstance(localInfo.Name);


            using (var memoryStream = new MemoryStream())
            {
                var remote = new DacServices(AppConfig.RemoteConnectionString);
                remote.ProgressChanged += (sender, args) => Console.WriteLine($"remote {args.Status} {args.Message}");
                remote.ExportBacpac(memoryStream, localInfo.Name, DacSchemaModelStorageType.Memory);

                using (var bacPackage = BacPackage.Load(memoryStream, DacSchemaModelStorageType.Memory))
                {
                    var local = new DacServices(AppConfig.LocalConnectionString);
                    local.ProgressChanged += (sender, args) => Console.WriteLine($"local {args.Status} {args.Message}");
                    local.ImportBacpac(bacPackage, localInfo.Name);
                }
            }
        }
    }
}
