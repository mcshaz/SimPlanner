﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 14.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace SP.Web.UserEmails
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public partial class ReverseConfirmation : ReverseConfirmationBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("\r\n<table class=\"callout\"><tr><th class=\"callout-inner primary\">\r\n\t\t\t<table class=" +
                    "\"row\"><tbody><tr>\r\n\t\t\t\t\t\t<th class=\"small-12 large-12 columns first last\">\r\n\t\t\t\t" +
                    "\t\t\t<table>\r\n\t\t\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t\t\t<th>\r\n\t\t\t\t\t\t\t\t\t\t<p> ");
            
            #line 13 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.FullName));
            
            #line default
            #line hidden
            this.Write(" \r\n\t\t\t\t\t\t\t\t\t\t\t<small>");
            
            #line 14 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.ProfessionalRole.Description));
            
            #line default
            #line hidden
            this.Write(" </small>\r\n\t\t\t\t\t\t\t\t\t\t\thad been confirmed as ");
            
            #line 15 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.IsConfirmed.Value?"attending":"unable to attend"));
            
            #line default
            #line hidden
            this.Write(" as a ");
            
            #line 15 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.IsFaculty?"faculty member":"participant"));
            
            #line default
            #line hidden
            this.Write(" in the \r\n\t\t\t\t\t\t\t\t\t\t\t");
            
            #line 16 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseName));
            
            #line default
            #line hidden
            this.Write(" on the ");
            
            #line 16 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(FormattedDate(CourseParticipant.Course.StartLocal)));
            
            #line default
            #line hidden
            this.Write(".</p>\r\n\t\t\t\t\t\t\t\t\t\t<p>However, he/she would like to <strong>change this response</s" +
                    "trong> to being ");
            
            #line 17 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.IsConfirmed.Value?"unable":"able"));
            
            #line default
            #line hidden
            this.Write(@" to attend.</p>
									</th>
									<th class=""expander""></th>
								</tr>
							</table>
						</th>
					</tr></tbody></table>
		</th><th class=""expander""></th></tr></table>
		<table class=""row""><tbody><tr>
			<th class=""small-12 large-12 columns first last"">
				<table>
					<tr>
						<th>
							<p class=""small""><em>Note:</em> you can also change the confirmation status for any or all participants by logging in <a href=""");
            
            #line 30 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetCourseRef()));
            
            #line default
            #line hidden
            this.Write(@""">here</a></p>
						</th>
					</tr>
				</table>
			</th>
		</tr></tbody></table>

		<table class=""row""><tbody><tr>
			<th class=""small-offset-1 large-offset-1 small-11 large-11 columns first last"">
				<table>
					<tr>
						<th>
							<h3>Contact details for ");
            
            #line 42 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.FullName));
            
            #line default
            #line hidden
            this.Write(":</h3>\r\n\t\t\t\t\t\t</th>\r\n\t\t\t\t\t</tr>\r\n\t\t\t\t</table>\r\n\t\t\t</th>\r\n\t\t</tr></tbody></table>\r" +
                    "\n\r\n\t\t");
            
            #line 49 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
 if(CourseParticipant.Participant.PhoneNumber != null) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t<table class=\"row\"><tbody><tr>\r\n\t\t\t\t<th class=\"small-offset-1 large-offset-1 s" +
                    "mall-11 large-11 columns first last\">\r\n\t\t\t\t\t<table>\r\n\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t<th>\r\n\t\t" +
                    "\t\t\t\t\t\t<h5>Phone:</h5>\r\n\t\t\t\t\t\t\t\t<p>");
            
            #line 56 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.PhoneNumber));
            
            #line default
            #line hidden
            this.Write("</p>\r\n\t\t\t\t\t\t\t</th>\r\n\t\t\t\t\t\t</tr>\r\n\t\t\t\t\t</table>\r\n\t\t\t\t</th>\r\n\t\t\t</tr></tbody></tabl" +
                    "e>\r\n\t\t");
            
            #line 62 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n\t\t<table class=\"row\"><tbody><tr>\r\n\t\t\t<th class=\"small-offset-1 large-offset-1 s" +
                    "mall-11 large-11 columns first last\">\r\n\t\t\t\t<table>\r\n\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t<th>\r\n\t\t\t\t\t" +
                    "\t\t<h5>Email:</h5>\r\n\t\t\t\t\t\t\t<p><a href=\"mailto:");
            
            #line 70 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.Email));
            
            #line default
            #line hidden
            this.Write("\">");
            
            #line 70 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.Email));
            
            #line default
            #line hidden
            this.Write("</a></p>\r\n\t\t\t\t\t\t</th>\r\n\t\t\t\t\t</tr>\r\n\t\t\t\t</table>\r\n\t\t\t</th>\r\n\t\t</tr></tbody></table" +
                    ">\r\n\r\n\t\t");
            
            #line 77 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
 if(CourseParticipant.Participant.AlternateEmail != null) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t<table class=\"row\"><tbody><tr>\r\n\t\t\t<th class=\"small-offset-1 large-offset-1 sm" +
                    "all-11 large-11 columns first last\">\r\n\t\t\t\t<table>\r\n\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t<th>\r\n\t\t\t\t\t\t" +
                    "\t<h5>Email:</h5>\r\n\t\t\t\t\t\t\t<p><a href=\"mailto:");
            
            #line 84 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.AlternateEmail));
            
            #line default
            #line hidden
            this.Write("\">");
            
            #line 84 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.Participant.AlternateEmail));
            
            #line default
            #line hidden
            this.Write("</a></p>\r\n\t\t\t\t\t\t</th>\r\n\t\t\t\t\t</tr>\r\n\t\t\t\t</table>\r\n\t\t\t</th>\r\n\t\t</tr></tbody></table" +
                    ">\r\n\t\t");
            
            #line 90 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\r\n\t<hr/>\r\n\t<table class=\"row\"><tbody><tr>\r\n\t\t\t\t<th class=\"small-12 large-6 column" +
                    "s first\">\r\n\t\t\t\t\t<table>\r\n\t\t\t\t\t\t<tr>\r\n\t\t\t\t\t\t\t<th>\r\n\t\t\t\t\t\t\t\t<center data-parsed=\"\"" +
                    ">\r\n\t\t\t\t\t\t\t\t\t<table class=\"button success float-center\"><tr><td><table><tr><td><a" +
                    " href=\"");
            
            #line 99 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetNotificationUrl(!CourseParticipant.IsConfirmed.Value)));
            
            #line default
            #line hidden
            this.Write("\">Accept Change <small>(");
            
            #line 99 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.IsConfirmed.Value?"not attending":"attending"));
            
            #line default
            #line hidden
            this.Write(@")</small></a></td></tr></table></td></tr></table> 
									<center align=""center"" class=""float-center"" data-parsed="""">
									</center></center></th>
						</tr>
					</table>
				</th>
				<th class=""small-12 large-6 columns last"">
					<table>
						<tr>
							<th>
								<center data-parsed="""">
									<table class=""button alert float-center""><tr><td><table><tr><td><a href=""");
            
            #line 110 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetNotificationUrl(CourseParticipant.IsConfirmed.Value)));
            
            #line default
            #line hidden
            this.Write("\">Decline Change <small>(");
            
            #line 110 "C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\UserEmails\ReverseConfirmation.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CourseParticipant.IsConfirmed.Value?"attending":"not attending"));
            
            #line default
            #line hidden
            this.Write(")</small></a></td></tr></table></td></tr></table>\r\n\t\t\t\t\t\t\t\t\t<center align=\"center" +
                    "\" class=\"float-center\" data-parsed=\"\">\t\t\r\n\t\t\t\t\t\t\t\t\t</center></center></th>\r\n\t\t\t\t" +
                    "\t\t</tr>\r\n\t\t\t\t\t</table>\r\n\t\t\t\t</th>\r\n\t\t\t</tr></tbody></table>\r\n\t\t");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public class ReverseConfirmationBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
