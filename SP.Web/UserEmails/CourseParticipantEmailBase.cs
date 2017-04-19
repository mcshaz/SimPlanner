using SP.DataAccess;
using System.Text.RegularExpressions;

namespace SP.Web.UserEmails
{
    public class CourseParticipantEmailBase : EmailBase
    {

        private CourseParticipant _courseParticipant;
        public CourseParticipant CourseParticipant
        {
            get { return _courseParticipant; }
            set
            {
                _courseParticipant = value;
                FormatProvider = value.Department.Institution.Culture.CultureInfo;
            }
        }

        public string CourseName
        {
            get
            {
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
