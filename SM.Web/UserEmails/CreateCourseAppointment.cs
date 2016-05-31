using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using HtmlAgilityPack;
using SM.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace SM.Web.UserEmails
{
    static class Appointment
    {
        const string mailto = "MAILTO:";
        static public AppointmentStream CreateiCalendar(Course course, IIdentity currentUser)
        {
            iCalendar iCal = new iCalendar {
                Method = CalendarMethods.Request, 
                Version = "2.0",
                ProductID = "SimManager",
            };

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);
            iCalTimeZone timezone = iCalTimeZone.FromSystemTimeZone(tzi);
            iCal.AddTimeZone(timezone);

            // Create the event, and add it to the iCalendar
            Event courseEvt = iCal.Create<Event>();

            courseEvt.Class = "PUBLIC";

            courseEvt.Created = new iCalDateTime(course.Created);
            courseEvt.DTStamp = new iCalDateTime(DateTime.UtcNow);
            courseEvt.LastModified = new iCalDateTime(course.LastModified);
            courseEvt.Sequence = course.EmailSequence;
            courseEvt.Transparency = TransparencyType.Opaque;
            courseEvt.Status = EventStatus.Confirmed;
            courseEvt.UID = "course" + course.Id.ToString();
            courseEvt.Priority = 5;

            var start = TimeZoneInfo.ConvertTimeFromUtc(course.StartTime, tzi);

            // Set information about the event
            courseEvt.DTStart = new iCalDateTime(start, course.Department.Institution.StandardTimeZone);
            courseEvt.DTEnd = new iCalDateTime(TimeZoneInfo.ConvertTimeFromUtc(course.FinishTime, tzi), course.Department.Institution.StandardTimeZone); // This also sets the duration
            //evt.IsAllDay = false;
            //evt.Name = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            //evt.Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description;
            courseEvt.Location = course.Room.ShortDescription;
            courseEvt.Summary = course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + start.ToString("g");

            if (course.Department.Institution.Latitude.HasValue && course.Department.Institution.Longitude.HasValue)
            {
                courseEvt.GeographicLocation = new GeographicLocation(course.Department.Institution.Latitude.Value, course.Department.Institution.Longitude.Value);
            }

            var org = course.CourseParticipants.Where(cp => cp.IsOrganiser).FirstOrDefault(cp => cp.Participant.UserName == currentUser.Name)
                ?? course.CourseParticipants.FirstOrDefault();

            
            if (org == null)
            {
                const string simManagerInfo = mailto + "info@SimManager.org"; //ToDo read from web.config mail settings
                courseEvt.Organizer = new Organizer(simManagerInfo) { CommonName = "SimManager" };
            }
            else
            {
                courseEvt.Organizer = new Organizer(mailto + org.Participant.Email) { CommonName = org.Participant.FullName };
            }
            courseEvt.Attendees.AddRange(from cp in course.CourseParticipants
                             where cp != org
                             select (IAttendee)new Attendee(mailto + cp.Participant.Email) {
                                CommonName = cp.Participant.FullName,
                                Role = "REQ-PARTICIPANT",
                                RSVP = false,
                                ParticipationStatus = cp.IsConfirmed.HasValue
                                    ?cp.IsConfirmed.Value
                                        ? ParticipationStatus.Accepted
                                        : ParticipationStatus.Declined
                                    :ParticipationStatus.Tentative
                            });
            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.

            //set alarm
            Alarm alarm = new Alarm();
            alarm.Action = AlarmAction.Display;
            alarm.Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            alarm.Trigger = new Trigger(TimeSpan.FromHours(-1));
            
            // Add the alarm to the event
            courseEvt.Alarms.Add(alarm);

            return new AppointmentStream(iCal);
        }

        public static void AddFacultyMeeting(iCalendar iCal, Course course)
        {
            iCal.Method = CalendarMethods.Publish; //if more than 1 event, this is required

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);
            var courseEvent = iCal.Events.First();
            Event meeting = iCal.Create<Event>();

            meeting.Class = "PUBLIC";

            meeting.Created = new iCalDateTime(course.Created);
            meeting.DTStamp = courseEvent.DTStamp;
            meeting.LastModified = new iCalDateTime(course.LastModified);
            meeting.Sequence = course.EmailSequence;
            //evt.Transparency = TransparencyType.Opaque;
            //evt.Status = EventStatus.Confirmed;
            meeting.UID = "planning" + course.Id.ToString();
            meeting.Priority = 5;

            meeting.Transparency = TransparencyType.Opaque;
            meeting.Status = EventStatus.Confirmed;

            // Set information about the event
            if (course.FacultyMeetingTime.HasValue)
                meeting.Start = new iCalDateTime(TimeZoneInfo.ConvertTimeFromUtc(course.FacultyMeetingTime.Value, tzi), course.Department.Institution.StandardTimeZone);
            if (course.FacultyMeetingDuration.HasValue)
                meeting.Duration = TimeSpan.FromMinutes(course.FacultyMeetingDuration.Value);
            //evt.Name = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            //evt.Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description;
            if (course.FacultyMeetingRoom != null)
                meeting.Location = course.FacultyMeetingRoom.ShortDescription;

            meeting.Description = "planning meeting for " + course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + course.LocalStart().ToString("g");

            meeting.Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation + " planning meeting - " + course.LocalStart().ToString("d");

            var fac = new HashSet<string>(from cp in course.CourseParticipants where cp.IsFaculty select mailto + cp.Participant.Email);

            meeting.Attendees.AddRange(courseEvent.Attendees.Where(a=>fac.Contains(a.Value.OriginalString)));

            meeting.Organizer = courseEvent.Organizer;

            Alarm alarm = new Alarm();
            alarm.Action = AlarmAction.Display;
            alarm.Summary = meeting.Summary;
            alarm.Trigger = new Trigger(TimeSpan.FromHours(-1));

            // Add the alarm to the event
            meeting.Alarms.Add(alarm);
        }
    }

    public sealed class AppointmentStream : IDisposable
    {
        public AppointmentStream(iCalendar iCal)
        {
            Ical = iCal;
            _serializer = new iCalendarSerializer();
            

        }
        public readonly iCalendar Ical;
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
                    _calType.Parameters.Add("method", Ical.Method);
                }
                return _calType;
            }
        }
        public void AddAppointment(MailMessage msg)
        {

            var evt = (Event)Ical.Events.First();

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

            string calString = _serializer.SerializeToString(Ical);

            //writeOutlookFormat

            // Event description HTML text
            // X-ALT-DESC;FMTTYPE=text/html
            /*
            var cal = AlternateView.CreateAlternateViewFromString(calString, CalType);
            msg.AlternateViews.Add(cal);
            msg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
            */
            //end writeOutlookFormat

            //File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "calendar.ics", calString);
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
                Ical.Dispose();
                _isDisposed = true;
            }
            
        }
    }
}
