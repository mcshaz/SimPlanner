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
            string metadata = MedSimDtoRepository.GetMetadata();
            const string metadataPath = "~/app/metadata.js";

            // construct the filename and runtime file location
            string fileName = HostingEnvironment.MapPath(metadataPath)
                ?? @"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimManager\SM.Web" + metadataPath.Substring(1).Replace('/', '\\');

            // the same pre- and post-fix strings we used earlier
            const string prefix = "; Object.defineProperty(window, 'medsimMetadata', {enumerable: false, configurable: false, writable: false,"
                + "value: JSON.stringify(";

            const string postfix = ")});";

            // write to file
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(prefix + metadata + postfix);
            }
        }

    }
}
