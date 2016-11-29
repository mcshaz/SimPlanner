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
using Ical.Net.Interfaces.DataTypes;
using System.Collections.Generic;
using SP.Dto;

namespace SP.Web.UserEmails
{
    public static class Appointment
    {
        private static Calendar CreateCal(Course course)
        {
            var currentCal = new Calendar
            {
                Method = course.Cancelled ? CalendarMethods.Cancel : CalendarMethods.Request,
            };
            var timezone = VTimeZone.FromSystemTimeZone(course.Department.Institution.TimeZone);
            currentCal.AddTimeZone(timezone);
            return currentCal;
        }
        private static GeographicLocation GetGeoLocation(Course course)
        {
            if (course.Department.Institution.Latitude.HasValue && course.Department.Institution.Longitude.HasValue)
            {
                return new GeographicLocation(course.Department.Institution.Latitude.Value, course.Department.Institution.Longitude.Value);
            }
            return null;
        }
        private static Organizer GetOrganizer(Course course)
        {
            const string SimPlannerInfo = mailto + "info@sim-planner.com"; //ToDo read from web.config mail settings
            return new Organizer() { Value = new Uri(SimPlannerInfo), CommonName = "sim-planner.com" };
        }

        const string mailto = "mailto:";
        static public Calendar CreateCourseAppointment(Course course, IIdentity currentUser)
        {
            TimeZoneInfo tzi = course.Department.Institution.TimeZone;
            var currentCal = CreateCal(course);
            // Create the event, and add it to the iCalendar
            //
            Event courseEvt = new Event
            {
                Class = "PRIVATE",
                Created = new CalDateTime(course.CreatedUtc),
                LastModified = new CalDateTime(course.CourseDatesLastModified),
                Sequence = course.EmailSequence,
                Transparency = TransparencyType.Opaque,
                Status = course.Cancelled ? EventStatus.Cancelled : EventStatus.Confirmed,
                Uid = "course" + course.Id.ToString(),
                Priority = 5,
                Location = course.Room.ShortDescription,
                Attendees = course.CourseParticipants.Select(MapCourseParticipantToAttendee).ToList(),
                Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation,
                IsAllDay = false,
                Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description,
                GeographicLocation = GetGeoLocation(course),
                Organizer = GetOrganizer(course),
                //DtStamp = - this is probably being inserted - check
            };
            System.Diagnostics.Debug.WriteLine(courseEvt.Organizer);
            foreach (var cd in course.AllDays().Take(course.CourseFormat.DaysDuration))
            {
                var dayEvt = courseEvt.Copy<Event>();
                dayEvt.Start = new CalDateTime(course.StartLocal, tzi.Id);
                dayEvt.Description += " - " + course.StartLocal.ToString("g");
                dayEvt.Duration = TimeSpan.FromMinutes(cd.DurationMins);
                if (course.CourseFormat.DaysDuration > 1)
                {
                    string dayNo = $" (day {cd.Day})";
                    dayEvt.Summary += dayNo;
                    dayEvt.Description += dayNo;
                }
                currentCal.Events.Add(dayEvt);
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

            return currentCal;
        }


        private static IAttendee MapCourseParticipantToAttendee(CourseParticipant cp)
        {
            string email = cp.Participant.Email;
            if (!string.IsNullOrEmpty(cp.Participant.AlternateEmail))
            {
                email += "?cc=" + cp.Participant.AlternateEmail;
            }
            var uri = new Uri(mailto + email);
            return new Attendee(uri)
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

        public static Calendar CreateFacultyCalendar(Course course)
        {
            var currentCal = CreateCal(course);
            var facultyMeet = CreateFacultyMeetingEvent(course);
            currentCal.Events.Add(facultyMeet);
            return currentCal;
        }

        public static void AppendFacultyMeetingEvent(Calendar cal, Course course)
        {
            if (!course.FacultyMeetingUtc.HasValue) { return; }
            //CalendarMethods.Request is only valid for single event per calendar
            cal.Method = course.Cancelled ? CalendarMethods.Cancel : CalendarMethods.Publish;
            var courseEvent = cal.Events.First();
            var facultyMeet = CreateFacultyMeetingEvent(course);
            cal.Events.Add(facultyMeet);
        }

        public static Event CreateFacultyMeetingEvent(Course course)
        {
            Event meeting = new Event
            {
                Class = "PRIVATE",
                Created = new CalDateTime(course.CreatedUtc),
                LastModified = new CalDateTime(course.FacultyMeetingDatesLastModified),
                Sequence = course.EmailSequence,
                Uid = "planning" + course.Id.ToString(),
                Priority = 5,
                Organizer = GetOrganizer(course),
                Transparency = TransparencyType.Opaque,
                Status = course.Cancelled ? EventStatus.Cancelled : EventStatus.Confirmed,
                Description = "planning meeting for " + course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + course.StartLocal.ToString("g"),
                Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation + " planning meeting for " + course.StartLocal.ToString("d"),
                Attendees = course.CourseParticipants.Where(cp => cp.IsFaculty).Select(MapCourseParticipantToAttendee).ToList(),
                Start = new CalDateTime(course.FacultyMeetingLocal.Value, course.Department.Institution.StandardTimeZone),
                GeographicLocation = GetGeoLocation(course)
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

            return meeting;
        }
    }

    public sealed class AppointmentStream : IDisposable
    {
        public AppointmentStream(params Calendar[] cals)
        {
            _cals = new List<CalendarInfo>();
            _serializer = new CalendarSerializer();
            foreach (var c in cals)
            {
                Add(c);
            }
        }

        public void Add(Calendar cal)
        {
            var evt = cal.Events.First();
            var stream = new MemoryStream();
            //should work but truncating the stream at the moment - ? flush needed in the ical.net source code
            //galaxy13
            //_serializer.Serialize(cal, stream, Encoding.UTF8);
            //stream.Flush();
            using (var sw = new StreamWriter(stream, Encoding.UTF8, 2500, true))
            {
                sw.Write(_serializer.SerializeToString(cal));
            }
                
            _cals.Add(new CalendarInfo
            {
                Name = (evt.Summary ?? evt.Description ?? "Appointment") + ".ics",
                Data = stream,
                Method = cal.Method
            });
        }

        public void Replace(params Calendar[] cals)
        {
            _cals.Clear();
            foreach (var c in cals)
            {
                Add(c);
            }
        }

        /// <summary>
        /// For testing
        /// </summary>
        public IEnumerable<Stream> GetStreams()
        {
            foreach (var s in _cals)
            {
                s.Data.Position = 0;
                yield return s.Data;
            }
        } 

        private readonly IList<CalendarInfo> _cals;
        private readonly SerializerBase _serializer;

        /*
        private void AddHtmlFromMsg(MailMessage msg)
        {
            foreach (var c in Cals)
            {
                var evt = (Event)c.Events.First();

                evt.Description = msg.Body;

                var htmlView = msg.AlternateViews.First(av => av.ContentType.MediaType == MediaTypeNames.Text.Html);

                //remove image tags from doc
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
                //end remove image tags

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
            }
        }
        */

        public void AddAppointmentsTo(MailMessage msg)
        {

            //writeOutlookFormat

            // Event description HTML text
            // X-ALT-DESC;FMTTYPE=text/html
            /*
            var cal = AlternateView.CreateAlternateViewFromString(calString, CalType);
            msg.AlternateViews.Add(cal);
            msg.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
            */
            //end writeOutlookFormat

            //hack alert - calendars could conceivably have different methods
            foreach (var c in _cals)
            {
                var contentType = new ContentType("text/calendar")
                {
                    CharSet = Encoding.UTF8.HeaderName,
                    Name = c.Name
                };
                contentType.Parameters.Add("method", c.Method);

                c.Data.Position = 0;
                var outStream = new MemoryStream((int)c.Data.Length);
                c.Data.CopyTo(outStream);
                var attach = new System.Net.Mail.Attachment(outStream, contentType); //System.Net.Mail.Attachment.CreateAttachmentFromString(s, calType);

                msg.Attachments.Add(attach);
            }
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
                foreach (var c in _cals)
                {
                    c.Data.Dispose();
                }
                _isDisposed = true;
            }

        }
    }

    internal class CalendarInfo
    {
        public Stream Data { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
    }

}
