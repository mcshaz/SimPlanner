namespace SM.Web.UserEmails
{
    using System;
    using System.Linq;

    public partial class EmailTemplate
    {
        public string Body { get; set; }
        public string Title { get; set; }

        public object BodyTemplate
        {
            set
            {
                // Get the type and the TransformText method using reflection
                var type = value.GetType();
                var method = type.GetMethod("TransformText");

                // Reflection signature checks
                if (method == null) throw new ArgumentException("BodyTemplate needs to be a RunTimeTemplate with a TransformText method");
                //if (method.ReturnType != typeof(string) || method.GetParameters().Any()) throw new ArgumentException("Wrong TransformText signature on the BodyTemplate");

                // If everything is ok, assign the value
                Body = (string)method.Invoke(value, new object[0]);
            }
        }
    }
}
