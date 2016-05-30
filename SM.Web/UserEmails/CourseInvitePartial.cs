using SM.DataAccess;
using System;
using System.Text.RegularExpressions;

namespace SM.Web.UserEmails
{
    public partial class CourseInvite : IMailBody
    {
        public string Title { get { return "Please Confirm - simulation Course " + FormattedDate(CourseParticipant.Course.StartTime); } }
        public CourseParticipant CourseParticipant { get; set; }
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
        string _rsvpFormat;
        string RsvpFormat
        {
            get
            {
                if (_rsvpFormat == null)
                {
                    _rsvpFormat = BaseUrl + "index.html#/rsvp?ParticipantId={0}&CourseId={1}&Attending={2}";
                }
                return _rsvpFormat;
            }
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
                return returnVar;
            } 
        }

        public string StartTime
        {
            get
            {
                return string.Format(ToStringHelper.FormatProvider, "{0:D} at {0:t}", LocalTime(CourseParticipant.Course.StartTime));
            }
        }

        public string FinishTime
        {
            get
            {
                return CourseParticipant.Course.LocalFinish().ToString("g", ToStringHelper.FormatProvider);
            }
        }

        public string GetNotificationUrl(bool canAttend)
        {
            return string.Format(RsvpFormat, CourseParticipant.ParticipantId, CourseParticipant.CourseId, canAttend?"1":"0");
        }
        
    }
}
