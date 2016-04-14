using SM.Dto;
using System.IO;
using System.Web.Hosting;

namespace SM.Web.App_Start
{
    public static class MetadataScriptWriter
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write()
        {
            //const string category = "MetadataScriptWriter";
            // get the metadata the same way we get it for the controller
            var metadata = MedSimDtoMetadata.GetAllMetadata();
            const string metadataPath = "~/app/metadata.js";

            // construct the filename and runtime file location
            string fileName = HostingEnvironment.MapPath(metadataPath)
                ?? @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimManager\SM.Web" + metadataPath.Substring(1).Replace('/', '\\');

            // the same pre- and post-fix strings we used earlier

            // write to file
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(
                    "(function(){" +
                    "	window.medsimMetadata = {\r\n" +
                    "		getBreezeMetadata: getBreezeMetadata,\r\n" +
                    "		getBreezeValidators: getBreezeValidators,\r\n" +
                    "       getEnums: getEnums\r\n" +
                    "	}\r\n" +
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

    }
}
