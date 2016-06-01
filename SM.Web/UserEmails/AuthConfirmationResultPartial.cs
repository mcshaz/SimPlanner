using SM.DataAccess;
using System;
using System.Text.RegularExpressions;

namespace SM.Web.UserEmails
{
    public partial class AuthConfirmationResult : IMailBody
    {
        public string Title { get { return "request to alter confirmation for simulation on " + FormattedDate(CourseParticipant.Course.StartTime); } }
        public CourseParticipant CourseParticipant { get; set; }
        public Participant Auth { get; set; }
        public bool IsChanged { get; set; }
        public string BaseUrl { get; set; }
        TimeZoneInfo _tzi;
        TimeZoneInfo Tzi
        {
            get
            {
                return _tzi ?? (_tzi = TimeZoneInfo.FindSystemTimeZoneById(CourseParticipant.Course.Department.Institution.StandardTimeZone));
            }
        }
        DateTime LocalTime(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, Tzi);
        }
        string FormattedDate(DateTime date)
        {
            return LocalTime(date).ToString("g", ToStringHelper.FormatProvider);
        }

        public IFormatProvider ToStringFormatProvider
        {
            set
            {
                ToStringHelper.FormatProvider = value;
            }
        }

        public string CourseName
        {
            get {
                string returnVar = CourseParticipant.Course.CourseFormat.CourseType.Description;
                if (!Regex.IsMatch(returnVar, @"(\bcourse\b)|(\bsim(ulation)?\b)", RegexOptions.IgnoreCase))
                {
                    returnVar += " course";
                }
                return CourseParticipant.Course.Department.Abbreviation + " " + returnVar;
            } 
        }
        
    }
}
