using System;

namespace SP.Dto.Interfaces
{
    public interface IAssociateFileDto
    {
        /// <summary>
        /// If set to null, will serve the entire archive
        /// </summary>
        string FileName { get; }
        byte[] File { get; }
    }

    public interface IAssociateFileRequiredDto : IAssociateFileDto
    {
        DateTime FileModified { get; }
        long FileSize { get; }
    }

    public interface IAssociateFileOptionalDto : IAssociateFileDto
    {
        DateTime? FileModified { get; }
        long? FileSize { get; }
    }
}
