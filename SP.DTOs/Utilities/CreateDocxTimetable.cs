using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;

namespace SP.Dto.Utilities
{
    public static class CreateDocxTimetable
    {
        private static readonly string[] _includeList = new [] {"CourseParticipants.Participant",
            "Department.Institution.Culture",
            "CourseFormat.CourseSlots.Activity.ActivityChoices",
            "CourseFormat.CourseType",
            "CourseSlotPresenters",
            "CourseSlotManikins.Manikin",
            "CourseScenarioFacultyRoles.FacultyScenarioRole",
            "CourseSlotActivities.Activity",
            "CourseSlotActivities.Scenario"
        };
        public static IQueryable<CourseDto> GetCourseIncludes(MedSimDtoRepository repo)
        {
            return repo.GetCourses(_includeList);
        }
        public static DbQuery<Course> GetCourseIncludes(MedSimDbContext context)
        {
            DbQuery<Course> returnVar = context.Courses;
            foreach (var i in _includeList) {
                returnVar = returnVar.Include(i);
            }
            return returnVar;
        }

        static void UpdateMetadata(WordprocessingDocument doc, Course course)
        {
            //doc.AddCoreProperty();
            throw new NotImplementedException();
        }

        static IList<TimetableRow> GetTimeTableRows(Course course)
        {
            var start = course.StartLocal;
            int scenarioCount = 0;
            var csps = course.CourseSlotPresenters.ToLookup(c=>c.CourseSlotId);
            //var csfrs = course.CourseScenarioFacultyRoles.ToLookup(c => c.CourseSlotId);
            var csas = course.CourseSlotActivities.ToDictionary(c => c.CourseSlotId);
            var emptyStringArray = new string[0];

            var returnVar = course.CourseFormat.CourseSlots.Where(cs=>cs.IsActive)
                .OrderBy(cs=>cs.Order).Select(cs=> {
                    var ttr = new TimetableRow
                    {
                        LocalStart = start,
                    };
                    CourseSlotActivity activity;
                    csas.TryGetValue(cs.Id, out activity);
                    if (cs.ActivityId.HasValue)
                    {
                        ttr.IsScenario = false;
                        ttr.SlotName = cs.Activity.Name;
                        ttr.SlotActivity = activity?.Activity?.Description;
                        ttr.Faculty = csps[cs.Id]?.Select(csp => csp.Participant.FullName)
                            ?? emptyStringArray;
                    }
                    else
                    {
                        ttr.IsScenario = false;
                        ttr.SlotName = "Scenario " + (++scenarioCount).ToString();
                        ttr.SlotActivity = activity?.Scenario?.BriefDescription;
                        ttr.Faculty = emptyStringArray;//csfrs[cs.Id]?.Select(csfr => csfr.Participant.FullName)
                        //    ?? emptyStringArray;
                    }
                    start += TimeSpan.FromMinutes(cs.MinutesDuration);
                    return ttr;
            }).ToList();
            returnVar.Add(new TimetableRow { LocalStart = start, SlotName = "Finish", Faculty=new string[0]});
            return returnVar;
        }

        public static void InsertBasicTimeTable(MainDocumentPart mainPart, Course course)
        {
            IFormatProvider prov = course.Department.Institution.Culture.CultureInfo;


            var mergeFields = mainPart.HeaderParts.Cast<OpenXmlPart>()
                .Concat(mainPart.FooterParts.Cast<OpenXmlPart>())
                .Concat(new OpenXmlPart[] { mainPart })
                .Select(x => x.RootElement)
                .GetMergeFieldDict();

            var mergeClassDict = Enum.GetValues(typeof(MergeClassification))
                .Cast<MergeClassification>()
                .ToDictionary(k => k.ToString());
            var names = mergeClassDict.Keys.OrderByDescending(k => k).ToList(); //descending so that longer (more precise) names come first

            var defaultType = MergeClassification.General.ToString();
            names.Remove(defaultType);

            var classifiedMergeFields = mergeFields
                .ToLookup(fc => mergeClassDict[names.FirstOrDefault(n => fc.Key.StartsWith(n)) ?? defaultType]);

            var faculty = course.CourseParticipants.ToLookup(cp => cp.IsFaculty);

            foreach (var mf in classifiedMergeFields[MergeClassification.General])
            {
                string replaceVal;
                switch (mf.Key.Replace(".", string.Empty).Replace(" ", string.Empty))
                {
                    case "CourseFormatDescription":
                        replaceVal = course.CourseFormat.Description;
                        break;
                    case "CourseStart":
                        replaceVal = course.StartLocal.ToString("D", prov);
                        break;
                    case "CourseTypeAbbreviation":
                        replaceVal = course.CourseFormat.CourseType.Abbreviation;
                        break;
                    case "CourseTypeDescription":
                        replaceVal = course.CourseFormat.CourseType.Description;
                        break;
                    case "Department":
                    case "DepartmentAbbreviation":
                        replaceVal = course.Department.Abbreviation;
                        break;
                    case "DepartmentName":
                        replaceVal = course.Department.Name;
                        break;
                    case "Faculty":
                        replaceVal = string.Join("\t", faculty[true].Select(cp => cp.Participant.FullName));
                        break;
                    case "Institution":
                    case "InstitutionAbbreviation":
                        replaceVal = course.Department.Institution.Abbreviation;
                        break;
                    case "InstitutionName":
                        replaceVal = course.Department.Institution.Name;
                        break;
                    case "Participants":
                        replaceVal = string.Join("\t", faculty[false].Select(cp => cp.Participant.FullName));
                        break;
                    default:
                        replaceVal = $"[Value Not Found - \'{ mf.Key }\']";
                        break;
                }
                foreach (var m in mf)
                {
                    m.InsertMergeFieldText(replaceVal);
                }
            }

            //the first first() will be any of the elements starting with the name slot
            //the second first is assuming each slot, eg slotStart only occurs once in the doc
            //could at a later date come up with some fancy ancestors() to find matching subgroup
            TableRow slotRow = classifiedMergeFields[MergeClassification.Slot].First().First().FindFirstAncestor<TableRow>();
            var ttrs = GetTimeTableRows(course);

            slotRow.CloneElement(ttrs, (mergeFieldName, ttr) =>
            {
                switch (mergeFieldName)
                {
                    case "SlotStart":
                        return ttr.LocalStart.ToString("t", prov);
                    case "SlotActivity":
                        return ttr.SlotActivity ?? string.Empty;
                    case "SlotName":
                        return ttr.SlotName;
                    case "SlotFaculty":
                        return string.Join("\n", ttr.Faculty);
                    default:
                        return $"[Value Not Found - \'{ mergeFieldName }\']";
                }
            });
        }
        //--------------------------------------------
        enum MergeClassification { General, Slot, /* Faculty, Participants, */ Scenario, ScenarioRole }

        public static WordprocessingDocument CreateDoc(string sourceFile, out MemoryStream stream)
        {
            byte[] byteArray = File.ReadAllBytes(sourceFile);
            stream = new MemoryStream();
            stream.Write(byteArray, 0, byteArray.Length);
            WordprocessingDocument document = WordprocessingDocument.Open(stream, true);
                // If your sourceFile is a different type (e.g., .DOTX), you will need to change the target type like so:
            document.ChangeDocumentType(WordprocessingDocumentType.Document);

                // Get the MainPart of the document
            return document;
        }
        public static DualTimetable CreateBothTimetables(Course course, string sourceFile)
        {
            MemoryStream facultyStream;
            using (var facultyDoc = CreateDoc(sourceFile, out facultyStream))
            {
                var mainFacultyPart = facultyDoc.MainDocumentPart;
                InsertBasicTimeTable(mainFacultyPart, course);
                var participantStream = new MemoryStream((int)facultyStream.Length);
                facultyStream.CopyTo(participantStream);
                using (var participantDoc = WordprocessingDocument.Open(participantStream, true))
                {
                    RemoveScenarios(participantDoc.MainDocumentPart);
                }

                AddScenarios(mainFacultyPart, course);

                return new DualTimetable {
                    FacultyTimetable = facultyStream,
                    ParticipantTimetable = participantStream
                };
            }
        }

        public static MemoryStream CreateFullTimetableDocx(Course course, string sourceFile)
        {
            MemoryStream stream;
            using (var doc = CreateDoc(sourceFile, out stream))
            {
                var mainPart = doc.MainDocumentPart;
                InsertBasicTimeTable(mainPart, course);
                AddScenarios(mainPart, course);
                return stream;
            }
        }

        public static MemoryStream CreateTimetableEventsDocx(Course course, string sourceFile)
        {
            MemoryStream stream;
            using (var doc = CreateDoc(sourceFile, out stream))
            {
                var mainPart = doc.MainDocumentPart;
                InsertBasicTimeTable(mainPart, course);
                RemoveScenarios(mainPart);
                return stream;
            }
        }


        private static List<OpenXmlElement> GetScenarioEls(MainDocumentPart doc)
        {
            var firstParaWithSectionBreak = doc.RootElement.Descendants<Paragraph>()
                .First(p => p.Descendants<SectionProperties>().Any());

            return firstParaWithSectionBreak.Parent.ChildElements
                .SkipWhile(c => c != firstParaWithSectionBreak)
                .TakeWhile(c => c.GetType() != typeof(SectionProperties))
                .ToList();
        }

        public static void RemoveScenarios(MainDocumentPart doc)
        {
            var allScenarioEls = GetScenarioEls(doc);
            foreach (var el in allScenarioEls)
            {
                el.Remove();
            }
        }

        public static void AddScenarios(MainDocumentPart doc, Course course)
        {
            var allScenarioEls = GetScenarioEls(doc);

            var csas = course.CourseSlotActivities
                .Where(ca => ca.ScenarioId != null)
                .ToDictionary(ca => ca.CourseSlotId);

            var manikins = course.CourseSlotManikins
                .ToLookup(m => m.CourseSlotId);

            var roles = course.CourseScenarioFacultyRoles
                .ToLookup(k => k.CourseSlotId);
            int i = 0;
            var csss = (from cs in course.CourseFormat.CourseSlots.OrderBy(c=>c.Order)
                        let scenarioNo = ++i
                        where cs.IsActive && (csas.ContainsKey(cs.Id) || manikins[cs.Id].Any() || roles[cs.Id].Any())
                        select new { cs, scenarioNo}).ToList();

            var roleFacultyEls = new List<ScenarioRoleEl>();
            allScenarioEls.CloneElements(csss, (mergeFieldName, css, elements) =>
            {
                switch (mergeFieldName)
                {
                    case "ScenarioRole":
                    case "ScenarioRoleFaculty":
                        if (roleFacultyEls.Count == 0 || roleFacultyEls[roleFacultyEls.Count - 1].SlotId != css.cs.Id)
                        {
                            roleFacultyEls.Add(new ScenarioRoleEl { Row = elements.First().FindFirstAncestor<TableRow>(), SlotId = css.cs.Id });
                        }
                        return null;
                    case "ScenarioNo":
                        return "Scenario " + css.scenarioNo.ToString();
                    case "ScenarioName":
                    case "ScenarioBriefDescription":
                        CourseSlotActivity csab;
                        if (csas.TryGetValue(css.cs.Id, out csab))
                        {
                            return csab.Scenario?.BriefDescription ?? string.Empty;
                        }
                        return string.Empty;
                    case "ScenarioFullDescription":
                        CourseSlotActivity csaf;
                        if (csas.TryGetValue(css.cs.Id, out csaf))
                        {
                            return csaf.Scenario?.FullDescription ?? string.Empty;
                        }
                        return string.Empty;
                    case "Manikins":
                        return string.Join("\t",manikins[css.cs.Id].Select(m=>m.Manikin.Description));
                    default:
                        return $"[Value Not Found - \'{ mergeFieldName }\']";
                }
            });

            foreach (var sre in roleFacultyEls)
            {
                var scenarioRoles = roles[sre.SlotId].OrderBy(csfr => csfr.FacultyScenarioRole.Order)
                    .ToLookup(csfr => csfr.FacultyScenarioRole);

                if (scenarioRoles.Count == 0)
                {
                    sre.Row.FindFirstAncestor<Table>().Remove();
                }
                else
                {
                    sre.Row.CloneElement(scenarioRoles, (mergeFieldName, role) =>
                    {
                        switch (mergeFieldName)
                        {
                            case "ScenarioRole":
                                return role.Key.Description;
                            case "ScenarioRoleFaculty":
                                return string.Join("\n", role.Select(r => r.Participant.FullName));
                            default:
                                return $"[Value Not Found - \'{ mergeFieldName }\']";
                        }
                    });
                }
            }
        }
        internal static string CourseNameWithoutExt(Course course)
        {
            string dateString = course.StartLocal.ToString("d", course.Department.Institution.Culture.CultureInfo).Replace('/', '-');
            return $"{course.Department.Abbreviation} {course.CourseFormat.CourseType.Abbreviation} - {dateString}";
        }
        public static string TimetableName(Course course)
        {
            return CourseNameWithoutExt(course) + "Timetable.docx";
        }

    }

    internal class ScenarioRoleEl
    {
        public Guid SlotId { get; set; }
        public TableRow Row { get; set; }
    }

    internal class TimetableRow
    {
        public DateTime LocalStart { get; set; }
        public string SlotName { get; set; }
        public string SlotActivity { get; set; }
        public bool IsScenario { get; set; }
        public IEnumerable<string> Faculty { get; set; }
    }

}
