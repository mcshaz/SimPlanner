using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SP.Dto.Utilities
{
    public static class MailAttachmentExtensions
    {
        public static Attachment Clone(this Attachment master)
        {
            var newStream = new MemoryStream((int)master.ContentStream.Length);
            master.ContentStream.Position = 0;
            master.ContentStream.CopyTo(newStream);
            return new Attachment(newStream, master.ContentType) {
                ContentId = master.ContentId,
                Name = master.Name,
                NameEncoding = master.NameEncoding,
                TransferEncoding = master.TransferEncoding
            };

        }
    }
}
