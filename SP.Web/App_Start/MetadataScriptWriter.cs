using SP.DataAccess;
using SP.Dto;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace SP.Web.App_Start
{
    public static class MetadataScriptWriter
    {
        
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(bool force=false)
        {
            const string metadataPath = "~/app/metadata.js";
            // construct the filename and runtime file location
            string fileName = HostingEnvironment.MapPath(metadataPath)
                ?? @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web" + metadataPath.Substring(1).Replace('/', '\\');

            var migrationId = "//" + GetMigrationId();
            if (!force && File.ReadLines(fileName).First() == migrationId)
            {
                return;
            }

            //const string category = "MetadataScriptWriter";
            // get the metadata the same way we get it for the controller
            var metadata = MedSimDtoMetadata.GetAllMetadata(pretty:true);


            // the same pre- and post-fix strings we used earlier

            // write to file
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(
                    migrationId + "\r\n" +
                    "(function(){" +
                    "	window.medsimMetadata = {\r\n" +
                    "		getBreezeMetadata: getBreezeMetadata,\r\n" +
                    "		getBreezeValidators: getBreezeValidators,\r\n" +
                    "       getEnums: getEnums\r\n" +
                    "	};\r\n" +
                    "	function getBreezeMetadata(){\r\n" +
                    "		return JSON.stringify("+ metadata.Breeze + ");\r\n" +
                    "	}\r\n" +
                    "	function getBreezeValidators(){\r\n" +
                    "		return "+ metadata.RequiredNavProperties + ";\r\n" +
                    "	}\r\n" +
                    "	function getEnums(){\r\n" +
                    "		return " + MedSimDtoMetadata.GetEnums() + ";\r\n" +
                    "	}\r\n" +
                    "})();\r\n"
                    );
            }
        }
        static string GetMigrationId()
        {

            using (var db = new MedSimDbContext())
            {
                const string query = "select top 1 MigrationId from __MigrationHistory order by LEFT(MigrationId, 15) desc";
                var a = db.Courses.Any();
                return db.Database.SqlQuery<string>(query).FirstOrDefault();
            }
        }
    }
}
