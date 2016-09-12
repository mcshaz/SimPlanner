using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SP.Dto
{
    public abstract class ResourceDto 
	{
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string ResourceFilename { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }
	}
    [MetadataType(typeof(ResourceMetadata))]
    public class ActivityTeachingResourceDto : ResourceDto
    {
        public Guid CourseActivityId { get; set; }
        public CourseActivityDto CourseActivity { get; set; }

        public virtual ICollection<ChosenTeachingResourceDto> ChosenTeachingResources { get; set; }
    }
    [MetadataType(typeof(ResourceMetadata))]
    public class ScenarioResourceDto : ResourceDto
    {
        public Guid ScenarioId { get; set; }
        public ScenarioDto Scenario { get; set; }
    }

    public static class ResourceDtoExtensions
    {
        private static string GetPath(string appDomainPath, ResourceDto resource)
        {
            var scenarioResource = resource as ScenarioResourceDto;
            if (scenarioResource != null)
            {
                return Path.Combine(appDomainPath, "App_Data", scenarioResource.ScenarioId.ToString(), scenarioResource.ResourceFilename);
            }
            var activityResource = (ActivityTeachingResourceDto)resource;
            return Path.Combine(appDomainPath, "App_Data", activityResource.CourseActivity.CourseTypeId.ToString(), activityResource.ResourceFilename);
        }

        public static FileInfo GetFile(string appDomainPath, ResourceDto resource)
        {
            var path = GetPath(appDomainPath, resource);
            return new FileInfo(path);
        }

        public static void UpdateFileDetails(this ResourceDto resource, string appDomainPath)
        {
            var fi = GetFile(appDomainPath, resource);
            resource.Modified = fi.LastWriteTimeUtc;
            resource.Created = fi.CreationTimeUtc;
            resource.Size = fi.Length / 1024;
        }

        public static void CreateFile(this ResourceDto resource, string appDomainPath, FileStream fs)
        {
            var path = GetPath(appDomainPath, resource);
            FileInfo fi = new FileInfo(path);
            fi.Directory.Create(); // If the directory already exists, this method does nothing.
            using (var outStream = File.Create(fi.FullName))
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.CopyTo(outStream);
            }
        }

    }
}


