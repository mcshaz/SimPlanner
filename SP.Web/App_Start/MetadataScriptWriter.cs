using SP.DataAccess;
using SP.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace SP.Web.App_Start
{
    public static class MetadataScriptWriter
    {

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(bool force = false)
        {
            const string metadataPath = "~/app";

            MetaDataStrings metadata = null;

            var metadataFiles = new Dictionary<string, Func<string>> {
                ["breezeMetadata"] = () => (metadata ?? (metadata = MedSimDtoMetadata.GetAllMetadata(pretty: true))).Breeze, 
                ["breezeValidators"] = () => (metadata ?? (metadata = MedSimDtoMetadata.GetAllMetadata(pretty: true))).RequiredNavProperties, 
                ["breezeEnums"] = () => MedSimDtoMetadata.GetEnums()
            };
            // construct the filename and runtime file location
            string dirName = HostingEnvironment.MapPath(metadataPath)
                ?? @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web" + metadataPath.Substring(1).Replace('/', '\\');
            var migrationId = "//" + GetMigrationId();
            foreach (var f in metadataFiles)
            {
                var fn = Path.Combine(dirName, f.Key + ".ts");
                if (force || File.ReadLines(fn).First() != migrationId)
                {
                    using (var writer = new StreamWriter(fn))
                    {
                        writer.WriteLine(migrationId);
                        writer.WriteLine($"export default { f.Value() } }}");
                    }
                }
            }
        }

        static string GetMigrationId()
        {

            using (var db = new MedSimDbContext())
            {
                string query = "select top 1 MigrationId from [dbo].[__MigrationHistory] where [ContextKey] = '" + typeof(MedSimDbContext).FullName + "' order by LEFT(MigrationId, 15) desc";
                return db.Database.SqlQuery<string>(query).FirstOrDefault();
            }
        }
    }
}
