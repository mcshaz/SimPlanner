using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.DataAccess.Metadata.Attributes
{
    public class ValidFileNameAttribute : ValidationAttribute
    {
        public ValidFileNameAttribute()
        {
            RequireExtension = true;
            ErrorMessage = "Invalid Filename";
        }
        public override bool IsValid(object value)
        {
            //http://stackoverflow.com/questions/422090/in-c-sharp-check-that-filename-is-possibly-valid-not-that-it-exists
            var fileName = (string)value;
            if (string.IsNullOrEmpty(fileName)) { return true;  }
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1 ||
                (!AllowHidden && fileName[0] == '.') ||
                fileName[fileName.Length - 1]== '.')
            {
                return false;
            }
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(fileName);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }
            return fi != null 
                && (!RequireExtension || fi.Extension != string.Empty)
                && (ExtensionList==null || ExtensionList.Contains(fi.Extension));
        }
        private const string sepChar = ",";
        private IEnumerable<string> ExtensionList { get; set; }
        public bool AllowHidden { get; set; }
        public bool RequireExtension { get; set; }
        public string AllowedExtensions {
            get { return string.Join(sepChar, ExtensionList); } 
            set {
                if (string.IsNullOrEmpty(value))
                { ExtensionList = null; }
                else {
                    ExtensionList = value.Split(new char[] { sepChar[0] })
                        .Select(s => s[0] == '.' ? s : ('.' + s))
                        .ToList();
                }
        } }

        public override bool RequiresValidationContext => false;
    }
}
