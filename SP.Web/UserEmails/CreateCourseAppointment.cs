using HtmlAgilityPack;
using SP.DataAccess;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Principal;
using System.Text;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.iCalendar.Serializers;
using Ical.Net.General;
using Ical.Net.Interfaces.DataTypes;

namespace SP.Web.UserEmails
{
    static class Appointment
    {
        const string mailto = "MAILTO:";
        static public AppointmentStream CreateiCalendar(Course course, IIdentity currentUser)
        {
            var currentCal = new Calendar {
                Method = course.Cancelled ? CalendarMethods.Cancel: CalendarMethods.Publish,
            };
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);
            var timezone = VTimeZone.FromSystemTimeZone(tzi);
            currentCal.AddTimeZone(timezone);

            // Create the event, and add it to the iCalendar
            //
            const string SimPlannerInfo = mailto + "info@SimPlanner.org"; //ToDo read from web.config mail settings

            Event courseEvt = new Event
            {
                Class = "PRIVATE",
                Created = new CalDateTime(course.CreatedUtc),
                LastModified = new CalDateTime(course.LastModifiedUtc),
                Sequence = course.EmailSequence,
                Transparency = TransparencyType.Opaque,
                Status = course.Cancelled? EventStatus.Cancelled:EventStatus.Confirmed,
                Uid = "course" + course.Id.ToString(),
                Priority = 5,
                Location = course.Room.ShortDescription,
                Organizer = new Organizer(SimPlannerInfo) { CommonName = "SimPlanner" },
                Attendees = course.CourseParticipants.Select(MapCourseParticipantToAttendee).ToList(),
                Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation,
                IsAllDay = false,
                Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description
            };
            foreach (var cd in course.AllDays().Take(course.CourseFormat.DaysDuration))
            {
                var start = TimeZoneInfo.ConvertTimeFromUtc(cd.StartUtc, tzi);
                var dayEvt = courseEvt.Copy<Event>();
                dayEvt.Start = new CalDateTime(start, course.Department.Institution.StandardTimeZone);
                dayEvt.Description += " - " + start.ToString("g");
                dayEvt.Duration = TimeSpan.FromMinutes(cd.DurationMins);
                if (course.CourseFormat.DaysDuration > 1)
                {
                    string dayNo = $" (day {cd.Day})";
                    dayEvt.Summary += dayNo;
                    dayEvt.Description += dayNo;
                }
                currentCal.Events.Add(dayEvt);
            }


            

            if (course.Department.Institution.Latitude.HasValue && course.Department.Institution.Longitude.HasValue)
            {
                courseEvt.GeographicLocation = new GeographicLocation(course.Department.Institution.Latitude.Value, course.Department.Institution.Longitude.Value);
            }

            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.

            //set alarm
            Alarm alarm = new Alarm
            {
                Action = AlarmAction.Display,
                Summary = course.Department.Abbreviation + ' ' + course.CourseFormat.CourseType.Abbreviation,
                Trigger = new Trigger(TimeSpan.FromHours(-1))
            };
            
            // Add the alarm to the event
            courseEvt.Alarms.Add(alarm);

            return new AppointmentStream(currentCal);
        }

        private static IAttendee MapCourseParticipantToAttendee(CourseParticipant cp)
        {
            string email = cp.Participant.Email;
            if (!string.IsNullOrEmpty(cp.Participant.AlternateEmail))
            {
                email += ',' + cp.Participant.AlternateEmail;
            }
            return new Attendee(mailto + email)
            {
                CommonName = cp.Participant.FullName,
                Role = "REQ-PARTICIPANT",
                Rsvp = false,
                ParticipationStatus = cp.IsConfirmed.HasValue
                        ? cp.IsConfirmed.Value
                            ? EventParticipationStatus.Accepted
                            : EventParticipationStatus.Declined
                        : EventParticipationStatus.Tentative
            };
        }

        public static void AddFacultyMeeting(Calendar cal, Course course)
        {
            if (!course.FacultyMeetingUtc.HasValue) { return; }
            //CalendarMethods.Request is only valid for single event per calendar
            cal.Method = course.Cancelled ? CalendarMethods.Cancel : CalendarMethods.Publish; 

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);
            var courseEvent = cal.Events.First();
            var start = TimeZoneInfo.ConvertTimeFromUtc(course.StartUtc, tzi);
            Event meeting = new Event
            {
                Class = "Private",
                Created = new CalDateTime(course.CreatedUtc),
                LastModified = new CalDateTime(course.LastModifiedUtc),
                Sequence = course.EmailSequence,
                Uid = "planning" + course.Id.ToString(),
                Priority = 5,
                Transparency = TransparencyType.Opaque,
                Status = course.Cancelled ? EventStatus.Cancelled : EventStatus.Confirmed,
                Description = "planning meeting for " + course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + start.ToString("g"),
                Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation + " planning meeting - " + start.ToString("d"),
                Organizer = courseEvent.Organizer,
                Attendees = course.CourseParticipants.Where(cp => cp.IsFaculty).Select(MapCourseParticipantToAttendee).ToList(),
                Start = new CalDateTime(TimeZoneInfo.ConvertTimeFromUtc(course.FacultyMeetingUtc.Value, tzi), tzi.Id)
            };

            // Set information about the event
            if (course.FacultyMeetingDuration.HasValue)
                meeting.Duration = TimeSpan.FromMinutes(course.FacultyMeetingDuration.Value);
            //evt.Name = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            //evt.Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description;
            if (course.FacultyMeetingRoom != null)
                meeting.Location = course.FacultyMeetingRoom.ShortDescription;

            Alarm alarm = new Alarm
            {
                Action = AlarmAction.Display,
                Summary = meeting.Summary,
                Trigger = new Trigger(TimeSpan.FromHours(-1))
            };

            // Add the alarm to the event
            meeting.Alarms.Add(alarm);

            cal.Events.Add(meeting);
        }
    }

    public sealed class AppointmentStream : IDisposable
    {
        public AppointmentStream(Calendar cal)
        {
            Cal = cal;
            _serializer = new CalendarSerializer();
            

        }
        public readonly Calendar Cal;
        readonly SerializerBase _serializer;

        ContentType _calType;
        ContentType CalType
        {
            get
            {
                if (_calType == null)
                {
                    _calType = new ContentType("text/calendar");
                    _calType.CharSet = Encoding.UTF8.HeaderName;
                    _calType.Name = "appointment.ics";
                    _calType.Parameters.Add("method", Cal.Method);
                }
                return _calType;
            }
        }
        public void AddAppointment(MailMessage msg)
        {

            var evt = (Event)Cal.Events.First();

            evt.Description = msg.Body;
            
            var htmlView = msg.AlternateViews.First(av => av.ContentType.MediaType == MediaTypeNames.Text.Html);
            string html;

            var htmlDoc = new HtmlDocument();
            htmlView.ContentStream.Position = 0;
            using (var sr = new StreamReader(htmlView.ContentStream, Encoding.UTF8, false, 1024, true))
            {
                htmlDoc.Load(sr);
                foreach (var img in htmlDoc.DocumentNode.SelectNodes("//img"))
                {
                    img.RemoveAll();
                }
                html = htmlDoc.DocumentNode.OuterHtml;
            }
            htmlView.ContentStream.Position = 0;

            //add html to ical
            const string altDesc = "X-ALT-DESC";
            CalendarProperty prop = (CalendarProperty)evt.Properties[altDesc];

            if (prop == null)
            {
                prop = new CalendarProperty(altDesc);
                prop.AddParameter("FMTTYPE", MediaTypeNames.Text.Html);
                evt.AddProperty(prop);
            }
            prop.SetValue(html);
            //end add HTML to ical

            string calString = _serializer.SerializeToString(Cal);

            //writeOutlookFormat

            // Event description HTML text
            // X-ALT-DESC;FMTTYPE=text/html
            /*
            var cal = AlternateView.CreateAlternateViewFromString(calString, CalType);
            msg.AlternateViews.Add(cal);
            msg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
            */
            //end writeOutlookFormat

            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "calendar.ics", calString);
            var attach = System.Net.Mail.Attachment.CreateAttachmentFromString(calString, CalType);

            msg.Attachments.Add(attach);

        }

        /*
        static string InsertEvery(StreamReader sr, int every, string insert)
        {
            //NB potential error below byte array not necessarily related to string length
            var buffer = new char[sr.BaseStream.Length + (insert.Length * sr.BaseStream.Length / every)];
            var insertChars = insert.ToCharArray();
            int increment = every + insertChars.Length;
            int writeIndex = 0;
            while (writeIndex + increment < buffer.Length)
            {
                sr.Read(buffer, writeIndex, every);
                writeIndex += every;
                insertChars.CopyTo(buffer, writeIndex);
                writeIndex += insertChars.Length;
            }
            sr.Read(buffer, writeIndex, buffer.Length-writeIndex);
            return new string(buffer);

        }
        */

        bool _isDisposed;
        public void Dispose() { Dispose(true); }
        ~AppointmentStream() { Dispose(false); }
        void Dispose(bool disposing)
        { // would be protected virtual if not sealed 
            if (disposing)
            { // only run this logic when Dispose is called
                GC.SuppressFinalize(this);
                // and anything else that touches managed objects
            }
            if (!_isDisposed)
            {
                Cal.Dispose();
                _isDisposed = true;
            }
            
        }
    }
}
