using SP.DataAccess;
using System;
using System.Text.RegularExpressions;

namespace SP.Web.UserEmails
{
    public partial class ReverseConfirmation : IMailBody
    {
        public ReverseConfirmation(string authorizationToken)
        {
            AuthorizationToken = authorizationToken;
        }
        
        public string Title { get {
                return string.Format(ToStringHelper.FormatProvider, "participant request to alter confirmation for {0} on {1:d}",
                    CourseParticipant.Course.CourseFormat.CourseType.Abbreviation,
                    LocalTime(CourseParticipant.Course.StartTimeUtc)); } }
        private CourseParticipant _courseParticipant;
        public CourseParticipant CourseParticipant
        {
            get { return _courseParticipant; }
            set
            {
                _courseParticipant = value;
                ToStringFormatProvider = _courseParticipant.Course.Department.Institution.Culture.GetCultureInfo();
            }
        }
        readonly string AuthorizationToken;
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
                    _rsvpFormat = BaseUrl + "index.html#/rsvp?ParticipantId={0}&CourseId={1}&Attending={2}&Auth=" + AuthorizationToken;
                }
                return _rsvpFormat;
            }
        }

        string GetCourseRef()
        {
            return BaseUrl + "index.html#/course/" + CourseParticipant.Course.Id.ToString();
        }

        public IFormatProvider ToStringFormatProvider
        {
            get
            {
                return ToStringHelper.FormatProvider;
            }
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


        public string GetNotificationUrl(bool canAttend)
        {
            return string.Format(RsvpFormat, CourseParticipant.ParticipantId, CourseParticipant.CourseId, canAttend?"1":"0");
        }
        
    }
}
