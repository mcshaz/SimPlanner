using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Hosting;

namespace SP.Dto.Utilities
{
    public static class ResourceDtoExtensions
    {
        const string _zipExt = ".zip";
        //logic:
        //scenarios under App_Data/Scenarios/DepartmentGuid/ScenarioGuid.zip with all files pooled
        //TeachingResources under App_Data/TeachingResources/CourseTypeGuid/file.zip
        private static string _appDomainPath = HostingEnvironment.MapPath("~/App_Data");

        public static string ScenarioResourceToPath(Guid departmentId, Guid scenarioId)
        {
            return Path.Combine(_appDomainPath, "Scenarios", departmentId.ToString(), scenarioId.ToString() + _zipExt);
        }
        public static string TeachingResourceToPath(Guid courseTypeId, string resourceFileName)
        {
            return Path.Combine(_appDomainPath, "TeachingResources", courseTypeId.ToString(), resourceFileName + _zipExt);
        }

        public static IEnumerable<KeyValuePair<string, string>> GetFilePaths(this Course course)
        {
            return (from s in course.CourseSlotScenarios
                    let sr = s.Scenario.ScenarioResources.FirstOrDefault(ssr => ssr.FileName != null)
                    where sr != null
                    select new KeyValuePair<string, string>(s.Scenario.BriefDescription, ScenarioResourceToPath(sr.Scenario.DepartmentId, sr.ScenarioId)))
                   .Concat(from ctr in course.ChosenTeachingResources
                           let atr = ctr.ActivityTeachingResource
                           where atr.FileName != null
                           select new KeyValuePair<string, string>(atr.Description, TeachingResourceToPath(ctr.Course.CourseFormat.CourseTypeId, atr.FileName)));
        }

        public static void CreateFile(this ScenarioResourceDto resource, Guid departmentId)
        {
            string path = ScenarioResourceToPath(departmentId, resource.ScenarioId);
            CreateFile(resource, path);
        }

        public static void CreateFile(this ActivityTeachingResourceDto resource, Guid courseTypeId)
        {
            string path = TeachingResourceToPath(courseTypeId,resource.FileName);
            CreateFile(resource, path);
        }

        private static void CreateFile(ResourceDto resource, string path)
        {
            FileInfo fi = new FileInfo(path);
            fi.Directory.Create(); // If the directory already exists, this method does nothing.
            using (var stream = new MemoryStream(resource.File, false))
            {
                using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry entry = archive.Entries.FirstOrDefault(e => e.Name == resource.FileName)
                        ?? archive.CreateEntry(resource.FileName, CompressionLevel.Optimal);
                    using (var arcEntryStream = entry.Open())
                    {
                        stream.CopyTo(arcEntryStream);
                    }
                    entry.LastWriteTime = resource.FileModified.Value;
                }
            }
        }
        public static void DeleteFile(this ScenarioResource resource, Guid departmentId)
        {
            string path = ScenarioResourceToPath(departmentId, resource.ScenarioId);
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists) { return; }
            bool deleteFile = false;
            using (ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read))
            {
                ZipArchiveEntry entry = archive.Entries.FirstOrDefault(e => e.Name == resource.FileName);
                if (entry == null)
                {
                    return;
                }
                if (archive.Entries.Count == 1)
                {
                    deleteFile = true;
                }
                else
                {
                    entry.Delete();
                }
            }
            if (deleteFile)
            {
                fi.Delete();
            }
        }

        public static void DeleteFile(this ActivityTeachingResource resource, Guid courseTypeId)
        {
            string path = TeachingResourceToPath(courseTypeId, resource.FileName);
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists) { return; }
            fi.Delete();
        }
    }
}
