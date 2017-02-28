using SP.DataAccess;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization.iCalendar.Serializers;
using Ical.Net.Interfaces.DataTypes;
using System.Collections.Generic;
using SP.Dto;
using SP.Dto.Utilities;

namespace SP.Web.UserEmails
{
    public static class Appointment
    {
        public static Calendar CreateCal(IEnumerable<Event> events)
        {
            var currentCal = new Calendar
            {
                Method = CalendarMethods.Publish
            };
            var timezoneIds = new HashSet<string>();
            foreach (var e in events)
            {
                if (!string.IsNullOrEmpty(e.Start.TimeZoneName))
                {
                    timezoneIds.Add(e.Start.TimeZoneName);
                }
                currentCal.Events.Add(e);
            }

            foreach (var tz in timezoneIds)
            {
                currentCal.AddTimeZone(new VTimeZone { TzId = tz });
            }
            //var timezone = VTimeZone.FromSystemTimeZone(course.Department.Institution.TimeZone);
            //currentCal.AddTimeZone(timezone);
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

        public static List<Event> MapCoursesToEvents(IEnumerable<CourseParticipant> courseParticipants)
        {
            var returnVar = new List<Event>();
            foreach (var cp in courseParticipants)
            {
                var course = cp.Course;

                Event courseEvt = new Event
                {
                    Class = "PUBLIC",
                    Created = new CalDateTime(course.CreatedUtc),
                    LastModified = new CalDateTime(course.CourseDatesLastModified),
                    Sequence = course.Version,
                    //pending fix
                    Transparency = TransparencyType.Opaque,
                    Status = course.Cancelled ? EventStatus.Cancelled : EventStatus.Confirmed,
                    Uid = "course" + course.Id.ToString(),
                    Priority = 5,
                    Location = course.Room.ShortDescription,
                    Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation,
                    IsAllDay = false,
                    Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description + (cp.IsFaculty?" [Faculty]":" [Participant]"),
                    GeographicLocation = GetGeoLocation(course)
                    //DtStamp = - this is being inserted 
                };
                //now add aditional days
                foreach (var cd in course.AllDays().Take(course.CourseFormat.DaysDuration))
                {
                    var dayEvt = cd.Day < course.CourseFormat.DaysDuration
                        ? courseEvt.Copy<Event>()
                        : courseEvt;
                    dayEvt.Start = new CalDateTime(cp.IsFaculty?cd.StartFacultyUtc:cd.StartParticipantUtc);
                    dayEvt.Duration = TimeSpan.FromMinutes(cp.IsFaculty ? cd.DurationFacultyMins: cd.DurationParticipantMins);
                    if (course.CourseFormat.DaysDuration > 1)
                    {
                        string dayNo = $" (day {cd.Day})";
                        dayEvt.Summary += dayNo;
                        dayEvt.Description += dayNo;
                    }
                    if (cd.Day == 1)
                    {
                        dayEvt.Alarms.Add(new Alarm
                        {
                            Action = AlarmAction.Display,
                            Summary = course.Department.Abbreviation + ' ' + course.CourseFormat.CourseType.Abbreviation,
                            Trigger = new Trigger(TimeSpan.FromHours(-24))
                        });
                    }
                    returnVar.Add(dayEvt);
                }
            }

            return returnVar;
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
                Class = "PUBLIC",
                Created = new CalDateTime(course.CreatedUtc),
                LastModified = new CalDateTime(course.FacultyMeetingDatesLastModified),
                Sequence = course.Version,
                Uid = "planning" + course.Id.ToString(),
                Priority = 5,
                //pending ical.net fix
                Transparency = TransparencyType.Opaque,
                Status = course.Cancelled ? EventStatus.Cancelled : EventStatus.Confirmed,
                Description = "planning meeting for " + course.Department.Name + " " + course.CourseFormat.CourseType.Description + " - " + course.StartFacultyLocal.ToString("g"),
                Summary = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation + " planning meeting for " + course.StartFacultyLocal.ToString("d"),
                Start = new CalDateTime(course.FacultyMeetingUtc.Value), //, course.Department.Institution.StandardTimeZone),
                GeographicLocation = GetGeoLocation(course)
            };

            // Set information about the event
            meeting.Duration = TimeSpan.FromMinutes(course.FacultyMeetingDuration.Value);
                
            //evt.Name = course.Department.Abbreviation + " " + course.CourseFormat.CourseType.Abbreviation;
            //evt.Description = course.Department.Name + " " + course.CourseFormat.CourseType.Description;
            if (course.FacultyMeetingRoom != null)
            {
                meeting.Location = course.FacultyMeetingRoom.ShortDescription;
            }

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
            var stream = _serializer.SerializeToString(cal).ToStream();
            //should work but truncating the stream at the moment - ? flush needed in the ical.net source code
            //galaxy13
            //_serializer.Serialize(cal, stream, Encoding.UTF8);
            //stream.Flush();
                
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
                var contentType = new ContentType("text/plain") //"text/calendar"
                {
                    CharSet = Encoding.UTF8.HeaderName,
                    Name = c.Name
                };
                //contentType.Parameters.Add("method", c.Method);

                c.Data.Position = 0;
                var outStream = new MemoryStream((int)c.Data.Length); //should be able to wrap stream in Stream.Synchronized & 
                c.Data.CopyTo(outStream);
                outStream.Position = 0;
                var attach = new System.Net.Mail.Attachment(outStream, contentType); //System.Net.Mail.Attachment.CreateAttachmentFromString(s, calType);
                
                attach.ContentDisposition.FileName = c.Name;
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
