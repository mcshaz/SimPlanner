using System;
using System.ComponentModel.DataAnnotations;

namespace SM.Metadata.CustomValidators
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PersonFullNameAttribute : ValidationAttribute
    {
        public PersonFullNameAttribute() : base(defaultErrorMessage){}
        public const int MinNames=2;
        public const int MaxNames = 5;
        public const int MinNameLength = 2;
        const string defaultErrorMessage =
            "must be between 2 and 5 words, with a first and last name of at least 2 letter each";
        public override bool IsValid(object value)
        {
            string stringVal = (string)value;
            string[] nameComponents = stringVal.Trim().Split(' ');
            return (nameComponents.Length >= MinNames &&
                nameComponents.Length <= MaxNames &&
                nameComponents[0].Length >= MinNameLength &&
                nameComponents[nameComponents.Length - 1].Length >= MinNameLength);
        }
    }
}
