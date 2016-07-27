using HtmlAgilityPack;
using SP.DataAccess;
using System;
using System.Collections.Generic;
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

namespace SP.Web.UserEmails
{
    static class Appointment
    {
        const string mailto = "MAILTO:";
        static public AppointmentStream CreateiCalendar(Course course, IIdentity currentUser)
        {
            var currentCal = new Calendar {
                Method = CalendarMethods.Publish,
                Version = "2.0",
                ProductId = "SimPlanner",
            };
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);
            var timezone = VTimeZone.FromSystemTimeZone(tzi);
            currentCal.AddTimeZone(timezone);

            // Create the event, and add it to the iCalendar
            Event courseEvt = new Event
            {
                Class = "PRIVATE",
                Created = new CalDateTime(course.CreatedUtc),
                LastModified = new CalDateTime(course.LastModifiedUtc),
                Sequence = course.EmailSequence,
                Transparency = TransparencyType.Opaque,
                Status = EventStatus.Confirmed,
                Uid = "course" + course.Id.ToString(),
                Priority = 5,
                Location = course.Room.ShortDescription
            };

            currentCal.Events.Add(courseEvt);

            var start = TimeZoneInfo.ConvertTimeFromUtc(course.StartTimeUtc, tzi);

            // Set information about the event
            courseEvt.DtStart = new CalDateTime(start, course.Department.Institution.StandardTimeZone);
            courseEvt.DtEnd = new CalDateTime(TimeZoneInfo.ConvertTimeFromUtc(course.FinishTimeUtc, tzi), course.Department.Institution.StandardTimeZone); // This also sets the duration
            //evt.IsAllDay = false;
            //evt.Name = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            //evt.Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description;

            courseEvt.Summary = course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + start.ToString("g");

            if (course.Department.Institution.Latitude.HasValue && course.Department.Institution.Longitude.HasValue)
            {
                courseEvt.GeographicLocation = new GeographicLocation(course.Department.Institution.Latitude.Value, course.Department.Institution.Longitude.Value);
            }


            const string SimPlannerInfo = mailto + "info@SimPlanner.org"; //ToDo read from web.config mail settings
            courseEvt.Organizer = new Organizer(SimPlannerInfo) { CommonName = "SimPlanner" };

            foreach (var cp in course.CourseParticipants)
            {
                var at = new Attendee(mailto + cp.Participant.Email)
                {
                    CommonName = cp.Participant.FullName,
                    Role = "REQ-PARTICIPANT",
                    Rsvp = false,
                    ParticipationStatus = cp.IsConfirmed.HasValue
                                        ? cp.IsConfirmed.Value
                                            ? ParticipationStatus.Accepted
                                            : ParticipationStatus.Declined
                                        : ParticipationStatus.Tentative
                };
                courseEvt.Attendees.Add(at);
                if (!string.IsNullOrEmpty(cp.Participant.AlternateEmail))
                {
                    courseEvt.Attendees.Add(new Attendee(mailto + cp.Participant.AlternateEmail)
                    {
                        CommonName = at.CommonName,
                        Role = at.Role,
                        Rsvp = at.Rsvp,
                        ParticipationStatus = at.ParticipationStatus
                    });
                }
            }
                                    
            // Create a serialization context and serializer factory.
            // These will be used to build the serializer for our object.

            //set alarm
            Alarm alarm = new Alarm();
            alarm.Action = AlarmAction.Display;
            alarm.Summary = course.Department.Abbreviation + ' ' + course.CourseFormat.CourseType.Abbreviation;
            alarm.Trigger = new Trigger(TimeSpan.FromHours(-1));
            
            // Add the alarm to the event
            courseEvt.Alarms.Add(alarm);

            return new AppointmentStream(currentCal);
        }

        public static void AddFacultyMeeting(Calendar cal, Course course)
        {
            cal.Method = CalendarMethods.Publish; //if more than 1 event, this is required

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);
            var courseEvent = cal.Events.First();
            Event meeting = cal.Create<Event>();

            meeting.Class = "PRIVATE";

            meeting.Created = new CalDateTime(course.CreatedUtc);
            meeting.DtStamp = courseEvent.DtStamp;
            meeting.LastModified = new CalDateTime(course.LastModifiedUtc);
            meeting.Sequence = course.EmailSequence;
            //evt.Transparency = TransparencyType.Opaque;
            //evt.Status = EventStatus.Confirmed;
            meeting.Uid = "planning" + course.Id.ToString();
            meeting.Priority = 5;

            meeting.Transparency = TransparencyType.Opaque;
            meeting.Status = EventStatus.Confirmed;

            // Set information about the event
            if (course.FacultyMeetingTimeUtc.HasValue)
                meeting.Start = new CalDateTime(TimeZoneInfo.ConvertTimeFromUtc(course.FacultyMeetingTimeUtc.Value, tzi), course.Department.Institution.StandardTimeZone);
            if (course.FacultyMeetingDuration.HasValue)
                meeting.Duration = TimeSpan.FromMinutes(course.FacultyMeetingDuration.Value);
            //evt.Name = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            //evt.Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description;
            if (course.FacultyMeetingRoom != null)
                meeting.Location = course.FacultyMeetingRoom.ShortDescription;

            meeting.Description = "planning meeting for " + course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + course.LocalStart().ToString("g");

            meeting.Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation + " planning meeting - " + course.LocalStart().ToString("d");

            var fac = new HashSet<string>();
            foreach (var cp in course.CourseParticipants)
            {
                if (cp.IsFaculty)
                {
                    fac.Add(mailto + cp.Participant.Email);
                    if (!string.IsNullOrEmpty(cp.Participant.AlternateEmail))
                    {
                        fac.Add(mailto + cp.Participant.Email);
                    }
                }
            }

            foreach (var a in courseEvent.Attendees)
            {
                if (fac.Contains(a.Value.OriginalString))
                {
                    meeting.Attendees.Add(a);
                }
            }

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
