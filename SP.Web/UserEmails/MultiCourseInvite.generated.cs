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

namespace SP.Web.UserEmails
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\UserEmails\MultiCourseInvite.cshtml"
    using SP.DataAccess;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    public partial class MultiCourseInvite : SP.Web.UserEmails.EmailBase
    {
#line hidden
        #line 4 "..\..\UserEmails\MultiCourseInvite.cshtml"
           
    public Participant PersonRequesting { get; set; }
    public Participant Recipient { get; set; }
    public IEnumerable<Course> Courses { get; set; }

        #line default
        #line hidden
        
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 9 "..\..\UserEmails\MultiCourseInvite.cshtml"
  
    Layout = new SP.Web.UserEmails.LayoutTemplate
    {
        Title = "Upcoming Courses"
    };
    FormatProvider = Recipient.Department.Institution.Culture.CultureInfo;

            
            #line default
            #line hidden
WriteLiteral("\r\n<table");

WriteLiteral(" class=\"row\"");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; position: relat" +
"ive; text-align: left; vertical-align: top; width: 100%;\"");

WriteLiteral("><tbody><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral(">\r\n    <th");

WriteLiteral(" class=\"small-12 large-12 columns first last\"");

WriteLiteral(@" style=""Margin: 0 auto; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 16px; text-align: left; width: 564px;""");

WriteLiteral("><table");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; text-align: lef" +
"t; vertical-align: top; width: 100%;\"");

WriteLiteral("><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral("><th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n        <table");

WriteLiteral(" class=\"callout\"");

WriteLiteral(" style=\"Margin-bottom: 16px; border-collapse: collapse; border-spacing: 0; margin" +
"-bottom: 16px; padding: 0; text-align: left; vertical-align: top; width: 100%;\"");

WriteLiteral("><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral("><th");

WriteLiteral(" class=\"callout-inner primary\"");

WriteLiteral(" style=\"Margin: 0; background: #def0fc; border: 1px solid #444444; color: #0a0a0a" +
"; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: norma" +
"l; line-height: 1.3; margin: 0; padding: 10px; text-align: left; width: 100%;\"");

WriteLiteral(">\r\n            <p");

WriteLiteral(" style=\"Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, A" +
"rial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin" +
": 0; margin-bottom: 10px; padding: 0; text-align: left;\"");

WriteLiteral(">\r\n                <a");

WriteAttribute("href", Tuple.Create(" href=\"", 2206), Tuple.Create("\"", 2241)
            
            #line 20 "..\..\UserEmails\MultiCourseInvite.cshtml"
, Tuple.Create(Tuple.Create("", 2213), Tuple.Create<System.Object, System.Int32>(GetMailTo(PersonRequesting)
            
            #line default
            #line hidden
, 2213), false)
);

WriteLiteral(" style=\"Margin: 0; color: #2199e8; font-family: Helvetica, Arial, sans-serif; fon" +
"t-weight: normal; line-height: 1.3; margin: 0; padding: 0; text-align: left; tex" +
"t-decoration: none;\"");

WriteLiteral(">");

            
            #line 20 "..\..\UserEmails\MultiCourseInvite.cshtml"
                                                                                                                                                                                                                                       Write(PersonRequesting.FullName);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                <small");

WriteLiteral(" style=\"color: #cacaca; font-size: 80%;\"");

WriteLiteral(">(");

            
            #line 21 "..\..\UserEmails\MultiCourseInvite.cshtml"
                                                           Write(PersonRequesting.Department.Abbreviation);

            
            #line default
            #line hidden
WriteLiteral(" ");

            
            #line 21 "..\..\UserEmails\MultiCourseInvite.cshtml"
                                                                                                     Write(PersonRequesting.ProfessionalRole.Description);

            
            #line default
            #line hidden
WriteLiteral(")</small>\r\n                is looking for faculty to help run the courses as list" +
"ed below.\r\n            </p>\r\n        </th><th");

WriteLiteral(" class=\"expander\"");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0 !impo" +
"rtant; text-align: left; visibility: hidden; width: 0;\"");

WriteLiteral("></th></tr></table>\r\n    </th>\n<th");

WriteLiteral(" class=\"expander\"");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0 !impo" +
"rtant; text-align: left; visibility: hidden; width: 0;\"");

WriteLiteral("></th></tr></table></th>\r\n</tr></tbody></table>\r\n<table");

WriteLiteral(" class=\"row\"");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; position: relat" +
"ive; text-align: left; vertical-align: top; width: 100%;\"");

WriteLiteral("><tbody><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral(">\r\n    <th");

WriteLiteral(" class=\"small-12 large-12 columns first last\"");

WriteLiteral(@" style=""Margin: 0 auto; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 16px; text-align: left; width: 564px;""");

WriteLiteral("><table");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; text-align: lef" +
"t; vertical-align: top; width: 100%;\"");

WriteLiteral("><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral("><th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n        <table");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; text-align: lef" +
"t; vertical-align: top; width: 100%;\"");

WriteLiteral(">\r\n            <thead>\r\n                <tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral(">\r\n                    <th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n                        Date\r\n                    </th>\r\n                    <" +
"th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n                        Course\r\n                    </th>\r\n                   " +
" <th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n                        Reqd.\r\n                    </th>\r\n                </tr" +
">\r\n            </thead>\r\n            <tbody>\r\n");

            
            #line 45 "..\..\UserEmails\MultiCourseInvite.cshtml"
                
            
            #line default
            #line hidden
            
            #line 45 "..\..\UserEmails\MultiCourseInvite.cshtml"
                 foreach (var c in Courses)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral(">\r\n                        <td");

WriteLiteral(@" style=""-moz-hyphens: auto; -webkit-hyphens: auto; Margin: 0; border-collapse: collapse !important; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 1.3; margin: 0; padding: 0; text-align: left; vertical-align: top; word-wrap: break-word;""");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 49 "..\..\UserEmails\MultiCourseInvite.cshtml"
                       Write(string.Format(FormatProvider, "{0:d} {0:t}", c.StartFacultyLocal));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </td>\r\n                        <td");

WriteLiteral(@" style=""-moz-hyphens: auto; -webkit-hyphens: auto; Margin: 0; border-collapse: collapse !important; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 1.3; margin: 0; padding: 0; text-align: left; vertical-align: top; word-wrap: break-word;""");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 52 "..\..\UserEmails\MultiCourseInvite.cshtml"
                       Write(c.CourseFormat.Description);

            
            #line default
            #line hidden
WriteLiteral("\r\n                            (");

            
            #line 53 "..\..\UserEmails\MultiCourseInvite.cshtml"
                        Write(c.CourseFormat.CourseType.Description);

            
            #line default
            #line hidden
WriteLiteral(")\r\n                        </td>\r\n                        <td");

WriteLiteral(@" style=""-moz-hyphens: auto; -webkit-hyphens: auto; Margin: 0; border-collapse: collapse !important; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; hyphens: auto; line-height: 1.3; margin: 0; padding: 0; text-align: left; vertical-align: top; word-wrap: break-word;""");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 56 "..\..\UserEmails\MultiCourseInvite.cshtml"
                        Write(c.FacultyNoRequired - c.CourseParticipants.Count(cp => cp.IsFaculty && cp.IsConfirmed != false));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n");

            
            #line 59 "..\..\UserEmails\MultiCourseInvite.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </tbody>\r\n        </table>\r\n    </th>\n<th");

WriteLiteral(" class=\"expander\"");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0 !impo" +
"rtant; text-align: left; visibility: hidden; width: 0;\"");

WriteLiteral("></th></tr></table></th>\r\n</tr></tbody></table>\r\n<table");

WriteLiteral(" class=\"row\"");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; position: relat" +
"ive; text-align: left; vertical-align: top; width: 100%;\"");

WriteLiteral("><tbody><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral(">\r\n    <th");

WriteLiteral(" class=\"small-12 large-12 columns first last\"");

WriteLiteral(@" style=""Margin: 0 auto; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 16px; text-align: left; width: 564px;""");

WriteLiteral("><table");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; text-align: lef" +
"t; vertical-align: top; width: 100%;\"");

WriteLiteral("><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral("><th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n        <p");

WriteLiteral(" style=\"Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, A" +
"rial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin" +
": 0; margin-bottom: 10px; padding: 0; text-align: left;\"");

WriteLiteral(">\r\n            If you are able to help by attending as faculty <small");

WriteLiteral(" style=\"color: #cacaca; font-size: 80%;\"");

WriteLiteral(">(or require further details)</small>, please log in to <a");

WriteAttribute("href", Tuple.Create(" href=\"", 8585), Tuple.Create("\"", 8616)
            
            #line 68 "..\..\UserEmails\MultiCourseInvite.cshtml"
                                                                                           , Tuple.Create(Tuple.Create("", 8592), Tuple.Create<System.Object, System.Int32>(BaseUrl
            
            #line default
            #line hidden
, 8592), false)
, Tuple.Create(Tuple.Create("", 8600), Tuple.Create("/myCourseInvites", 8600), true)
);

WriteLiteral(" style=\"Margin: 0; color: #2199e8; font-family: Helvetica, Arial, sans-serif; fon" +
"t-weight: normal; line-height: 1.3; margin: 0; padding: 0; text-align: left; tex" +
"t-decoration: none;\"");

WriteLiteral(">Sim Planner Course Invitations</a> \r\n            and select which course(s) you " +
"are able to help with. \r\n        </p>\r\n    </th>\n<th");

WriteLiteral(" class=\"expander\"");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0 !impo" +
"rtant; text-align: left; visibility: hidden; width: 0;\"");

WriteLiteral("></th></tr></table></th>\r\n</tr></tbody></table>\r\n<table");

WriteLiteral(" class=\"row\"");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; position: relat" +
"ive; text-align: left; vertical-align: top; width: 100%;\"");

WriteLiteral("><tbody><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral(">\r\n    <th");

WriteLiteral(" class=\"small-12 large-12 columns first last\"");

WriteLiteral(@" style=""Margin: 0 auto; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin: 0 auto; padding: 0; padding-bottom: 16px; padding-left: 16px; padding-right: 16px; text-align: left; width: 564px;""");

WriteLiteral("><table");

WriteLiteral(" style=\"border-collapse: collapse; border-spacing: 0; padding: 0; text-align: lef" +
"t; vertical-align: top; width: 100%;\"");

WriteLiteral("><tr");

WriteLiteral(" style=\"padding: 0; text-align: left; vertical-align: top;\"");

WriteLiteral("><th");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0; text" +
"-align: left;\"");

WriteLiteral(">\r\n        <p");

WriteLiteral(" style=\"Margin: 0; Margin-bottom: 10px; color: #0a0a0a; font-family: Helvetica, A" +
"rial, sans-serif; font-size: 16px; font-weight: normal; line-height: 1.3; margin" +
": 0; margin-bottom: 10px; padding: 0; text-align: left;\"");

WriteLiteral(">\r\n            Thank you.\r\n        </p>\r\n    </th>\n<th");

WriteLiteral(" class=\"expander\"");

WriteLiteral(" style=\"Margin: 0; color: #0a0a0a; font-family: Helvetica, Arial, sans-serif; fon" +
"t-size: 16px; font-weight: normal; line-height: 1.3; margin: 0; padding: 0 !impo" +
"rtant; text-align: left; visibility: hidden; width: 0;\"");

WriteLiteral("></th></tr></table></th>\r\n</tr></tbody></table>");

        }
    }
}
#pragma warning restore 1591
