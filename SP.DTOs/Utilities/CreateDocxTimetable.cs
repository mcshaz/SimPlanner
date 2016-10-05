﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SP.Dto.Utilities
{
    public static class CreateDocxTimetable
    {
        public static Course GetCourseWithIncludes(Guid courseId, MedSimDbContext context)
        {
            return context.Courses.Include("CourseParticipants.Participant")
                        .Include("Department.Institution.Culture")
                        .Include("CourseFormat.CourseSlots.Activity.ActivityChoices")
                        .Include("CourseSlotPresenters")
                        .Include("CourseSlotManikins")
                        .Include("CourseScenarioFacultyRoles.FacultyScenarioRole")
                        .Include("CourseSlotActivities.Activity")
                        .Include("CourseSlotActivities.Scenario")
                        .First(c=>c.Id == courseId);
        }

        static void UpdateMetadata(WordprocessingDocument doc, Course course)
        {
            //doc.AddCoreProperty();
        }

        static IList<TimetableRow> GetTimeTableRows(Course course, TimeZoneInfo tzi)
        {
            var start = TimeZoneInfo.ConvertTimeFromUtc(course.StartUtc, tzi);
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


        //--------------------------------------------
        enum MergeClassification { General, Slot, /* Faculty, Participants, */ Scenario, ScenarioRole }
        public static MemoryStream CreateTimetableDocx(Course course, string sourceFile)
        {
            byte[] byteArray = File.ReadAllBytes(sourceFile);
            MemoryStream stream = new MemoryStream();
            stream.Write(byteArray, 0, byteArray.Length);
            using (WordprocessingDocument document = WordprocessingDocument.Open(stream, true))
            {
                IFormatProvider prov = course.Department.Institution.Culture.GetCultureInfo();
                var tzi = TimeZoneInfo.FindSystemTimeZoneById(course.Department.Institution.StandardTimeZone);

                // If your sourceFile is a different type (e.g., .DOTX), you will need to change the target type like so:
                document.ChangeDocumentType(WordprocessingDocumentType.Document);

                // Get the MainPart of the document
                MainDocumentPart mainPart = document.MainDocumentPart;

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
                    switch (mf.Key.Replace(".",string.Empty).Replace(" ", string.Empty))
                    {
                        case "CourseFormatDescription":
                            replaceVal = course.CourseFormat.Description ;
                            break;
                        case "CourseStart":
                            replaceVal = TimeZoneInfo.ConvertTimeFromUtc(course.StartUtc, tzi).ToString("D", prov) ;
                            break;
                        case "CourseTypeAbbreviation":
                            replaceVal = course.CourseFormat.CourseType.Abbreviation ;
                            break;
                        case "CourseTypeDescription":
                            replaceVal = course.CourseFormat.CourseType.Description ;
                            break;
                        case "Version":
                            replaceVal = course.Version.ToString() ;
                            break;
                        case "Department":
                        case "DepartmentAbbreviation":
                            replaceVal = course.Department.Abbreviation ;
                            break;
                        case "DepartmentName":
                            replaceVal = course.Department.Name ;
                            break;
                        case "Faculty":
                            replaceVal = string.Join("\t",faculty[true].Select(cp => cp.Participant.FullName));
                            break;
                        case "Institution":
                        case "InstitutionAbbreviation":
                            replaceVal = course.Department.Institution.Abbreviation ;
                            break;
                        case "InstitutionName":
                            replaceVal = course.Department.Institution.Name ;
                            break;
                        case "Participants":
                            replaceVal = string.Join("\t",faculty[false].Select(cp => cp.Participant.FullName));
                            break;
                        default:
                            replaceVal = $"[Value Not Found - \'{ mf.Key }\']" ;
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
                var ttrs = GetTimeTableRows(course, tzi);

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

                AddScenarios(mainPart, course);
                stream.Position = 0;
                return stream; 
            }
        }

        private static void AddScenarios(MainDocumentPart doc, Course course)
        {
            var firstParaWithSectionBreak = doc.RootElement.Descendants<Paragraph>()
                .First(p => p.Descendants<SectionProperties>().Any());

            var allScenarioEls = firstParaWithSectionBreak.Parent.ChildElements
                .SkipWhile(c => c != firstParaWithSectionBreak)
                .TakeWhile(c => c.GetType() != typeof(SectionProperties))
                .ToList();

            var csss = (from cs in course.CourseFormat.CourseSlots
                        where cs.IsActive && cs.ActivityId==null
                        orderby cs.Order
                        select cs).ToList();

            var csas = course.CourseSlotActivities
                .Where(ca=>ca.ScenarioId != null)
                .ToDictionary(ca=>ca.CourseSlotId);

            var manikins = course.CourseSlotManikins
                .ToLookup(m => m.CourseSlotId);

            var roleFacultyEls = new List<ScenarioRoleEl>();
            int i = 0;
            allScenarioEls.CloneElements(csss, (mergeFieldName, css, elements) =>
            {
                switch (mergeFieldName)
                {
                    case "ScenarioRole":
                    case "ScenarioRoleFaculty":
                        if (roleFacultyEls.Count == 0 || roleFacultyEls[roleFacultyEls.Count - 1].SlotId != css.Id)
                        {
                            roleFacultyEls.Add(new ScenarioRoleEl { Row = elements.First().FindFirstAncestor<TableRow>(), SlotId = css.Id });
                        }
                        return null;
                    case "ScenarioNo":
                        return "Scenario " + (++i).ToString();
                    case "ScenarioName":
                    case "ScenarioBriefDescription":
                        CourseSlotActivity csab;
                        if (csas.TryGetValue(css.Id, out csab))
                        {
                            return csab.Scenario?.BriefDescription ?? string.Empty;
                        }
                        return string.Empty;
                    case "ScenarioFullDescription":
                        CourseSlotActivity csaf;
                        if (csas.TryGetValue(css.Id, out csaf))
                        {
                            return csaf.Scenario?.FullDescription ?? string.Empty;
                        }
                        return string.Empty;
                    case "Manikins":
                        return string.Join("\t",manikins[css.Id].Select(m=>m.Manikin.Description));
                    default:
                        return $"[Value Not Found - \'{ mergeFieldName }\']";
                }
            });

            var roles = course.CourseScenarioFacultyRoles
                .ToLookup(k => k.CourseSlotId);

            foreach (var sre in roleFacultyEls)
            {
                var scenarioRoles = roles[sre.SlotId].ToLookup(csfr => csfr.FacultyScenarioRole)
                    .OrderBy(l => l.Key.Order);
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
