﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SP.Web.UserEmails.Inky
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
    using SP.DataAccess;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    public partial class CandidateReadingMessage : SP.Web.UserEmails.EmailBase
    {
#line hidden
        #line 5 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
           
    public Course Course { get; set; }

        #line default
        #line hidden
        
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("\r\n");

            
            #line 9 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
   
    Layout = new SP.Web.UserEmails.LayoutTemplate
    {
        Title = "Candidate Reading"
    };
    var afterCourse = Course.StartFacultyUtc < DateTime.UtcNow;

            
            #line default
            #line hidden
WriteLiteral("\r\n<row>\r\n    <columns large = \"12\" >\r\n        <callout");

WriteLiteral(" class=\"success\"");

WriteLiteral(">\r\n            <p>\r\n                Additional reading for the\r\n");

WriteLiteral("                ");

            
            #line 21 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
           Write(Course.CourseFormat.Description);

            
            #line default
            #line hidden
WriteLiteral("\r\n                (");

            
            #line 22 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
            Write(Course.CourseFormat.CourseType.Description);

            
            #line default
            #line hidden
WriteLiteral(")\r\n");

            
            #line 23 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                
            
            #line default
            #line hidden
            
            #line 23 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                 if (!Course.CourseFormat.CourseType.Description.EndsWith("course", StringComparison.OrdinalIgnoreCase))
                {

            
            #line default
            #line hidden
WriteLiteral("                    ");

WriteLiteral("Course\r\n");

            
            #line 26 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("                you \r\n");

            
            #line 28 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                
            
            #line default
            #line hidden
            
            #line 28 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                 if (afterCourse) {

            
            #line default
            #line hidden
WriteLiteral("                    ");

WriteLiteral("attended\r\n");

            
            #line 30 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                } else {

            
            #line default
            #line hidden
WriteLiteral("                    ");

WriteLiteral("are scheduled to attend\r\n");

            
            #line 32 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("                on ");

            
            #line 33 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
              Write(Course.StartFacultyLocal.ToString(afterCourse ? "D":"f",FormatProvider));

            
            #line default
            #line hidden
WriteLiteral(" is attached to this email.\r\n            </p>\r\n        </callout>\r\n    </columns>" +
"\r\n</row>\r\n<row>\r\n    <columns large = \"12\" >\r\n");

            
            #line 40 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
        
            
            #line default
            #line hidden
            
            #line 40 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
         if (afterCourse)
        {

            
            #line default
            #line hidden
WriteLiteral("            <p>\r\n                Thank you for attending, and we hope you have ga" +
"ined something useful for future practice.\r\n            </p>\r\n");

            
            #line 45 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <p>\r\n            Please take the time to read through the attachments\r\n");

            
            #line 48 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
            
            
            #line default
            #line hidden
            
            #line 48 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
             if (afterCourse)
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("while the course is still fresh in your mind\r\n");

            
            #line 51 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
            }
            else
            {

            
            #line default
            #line hidden
WriteLiteral("                ");

WriteLiteral("prior to attending the course\r\n");

            
            #line 55 "..\..\UserEmails\Inky\CandidateReadingMessage.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            .\r\n        </p>\r\n    </columns>\r\n</row>\r\n");

        }
    }
}
#pragma warning restore 1591
