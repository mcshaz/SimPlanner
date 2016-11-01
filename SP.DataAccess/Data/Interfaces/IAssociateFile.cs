using System;

namespace SP.DataAccess.Data.Interfaces
{

    public static class FileDefaults
    {
        public const long _maxFileSize = 250 * 1024;
        public const long _minFileSize = 1;
        public const string _errMsg = "File Size must be 1 byte to 250 KiB";
        public const string _zipExt = ".zip";
    }
    public interface IAssociateFile
    {
        Guid Id { get; }
        string FileName { get; set; }
        byte[] File { get; set; }
    }

    public interface IAssociateFileRequired : IAssociateFile
    {
        DateTime FileModified { get; }
        long FileSize { get; }
    }

    public interface IAssociateFileOptional : IAssociateFile
    {
        DateTime? FileModified { get; }
        long? FileSize { get; }
    }
}
