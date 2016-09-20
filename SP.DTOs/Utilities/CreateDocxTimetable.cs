using DocumentFormat.OpenXml;
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
                        .Include("CourseSlotScenarios")
                        .Include("CourseSlotPresenters")
                        .Include("CourseSlotManikins")
                        .Include("CourseScenarioFacultyRoles")
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
            /*
            ILookup<Guid, CourseSlotPresenter> csps = null;
            var getCsp = new Func<Guid, IEnumerable<CourseSlotPresenter>>(id=> (csps ?? (csps = course.CourseSlotPresenters.ToLookup(c=>c.CourseSlotId)))[id]);
            ILookup<Guid, CourseScenarioFacultyRole> csfrs = null;
            var getCsfr = new Func<Guid, IEnumerable<CourseScenarioFacultyRole>>(id => (csfrs ?? (csfrs = course.CourseScenarioFacultyRoles.ToLookup(c => c.CourseSlotId)))[id]);

            var emptyCsp = new CourseSlotPresenter[0];
            var emptyCsfr = new CourseScenarioFacultyRole[0];
            */
            var returnVar = course.CourseFormat.CourseSlots.Where(cs=>cs.IsActive)
                .OrderBy(cs=>cs.Order).Select(cs=> {
                    var ttr = new TimetableRow
                    {
                        LocalStart = start,
                    };
                    if (cs.ActivityId.HasValue)
                    {
                        ttr.IsScenario = false;
                        ttr.SlotName = cs.Activity.Name;
                        ttr.Faculty = cs.CourseSlotPresenters.Select(csp => csp.Participant.FullName);
                    }
                    else
                    {
                        ttr.IsScenario = false;
                        ttr.SlotName = "Scenario " + (++scenarioCount).ToString();
                        ttr.Faculty = cs.CourseScenarioFacultyRoles.Select(csfr => csfr.Participant.FullName);
                    }
                    start += TimeSpan.FromMinutes(cs.MinutesDuration);
                    return ttr;
            }).ToList();
            returnVar.Add(new TimetableRow { LocalStart = start, SlotName = "Finish", Faculty=new string[0]});
            return returnVar;
        }

        private static ILookup<string, OpenXmlElement> GetMergeFieldDict(this IEnumerable<OpenXmlElement> elements)
        {
            var splitter = new[] { ' ', '"' };
            const string mergefield = "MERGEFIELD";
            return elements.SelectMany(x => x.Descendants<SimpleField>().Select(sf => new { text = sf.Instruction.Value, el = (OpenXmlElement)sf })
                    .Concat(x.Descendants<FieldCode>().Select(fc => new { text = fc.Text, el = (OpenXmlElement)fc })))
                .Select(a => new { words = a.text.Split(splitter, StringSplitOptions.RemoveEmptyEntries), el = a.el })
                .Where(a => mergefield.Equals(a.words.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                .ToLookup(k => string.Join(" ", k.words.Skip(1).TakeWhile(i => i != "\\*")), v => v.el);
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
                            replaceVal = course.EmailSequence.ToString() ;
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
                        InsertMergeFieldText(m, replaceVal);
                    }
                }

                //the first first() will be any of the elements starting with the name slot
                //the second first is assuming each slot, eg slotStart only occurs once in the doc
                //could at a later date come up with some fancy ancestors() to find matching subgroup
                TableRow slotRow = classifiedMergeFields[MergeClassification.Slot].First().First().FindFirstAncestor<TableRow>();
                var ttrs = GetTimeTableRows(course, tzi);

                CloneRows(slotRow, ttrs, (mergeFieldName, ttr) =>
                {
                    switch (mergeFieldName)
                    {
                        case "SlotStart":
                            return ttr.LocalStart.ToString("t", prov);
                        case "SlotActivity":
                            return ttr.SlotName;
                        case "SlotFaculty":
                            return string.Join("\n", ttr.Faculty);
                        default:
                            return $"[Value Not Found - \'{ mergeFieldName }\']";
                    }
                });

                AddScenarios(mainPart, course);

                return stream; 
            }
        }

        private static void CloneRows<T>(TableRow row, IEnumerable<T> rowItemCollection, Func<string, T, string> withMergeField)
        {
            List<T> rowItemList = rowItemCollection as List<T>;
            if (rowItemList == null)
            {
                rowItemList = rowItemCollection.ToList();
            }
            if (rowItemList.Count == 0)
            {
                row.Remove();
                return;
            }

            Table table = row.FindFirstAncestor<Table>();

            TableRow rowClone = (TableRow)row.CloneNode(true);

            int lastRowIndex = rowItemList.Count - 1;
            for (int i = 0; i < rowItemList.Count; i++)
            {
                foreach (var sf in (new[] { row }).GetMergeFieldDict())
                {
                    string replaceVal = withMergeField(sf.Key.Replace(" ", string.Empty).Replace(".", string.Empty), rowItemList[i]);
                    foreach (var s in sf)
                    {
                        InsertMergeFieldText(s, replaceVal);
                    }
                }
                if (i > 0)
                {
                    table.AppendChild(row);
                }
                if (i < lastRowIndex)
                {
                    row = (TableRow)rowClone.CloneNode(true);
                }
                else
                {
                    row = rowClone;
                }
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

            if (course.CourseSlotScenarios.Count == 0)
            {
                allScenarioEls.ForEach(e => e.Remove());
                return;
            }
            var ScenarioElsClone = allScenarioEls.Select(e => e.CloneNode(true)).ToList();
            var csss = course.CourseSlotScenarios.OrderBy(css => css.CourseSlot.Order).ToList();
            var lastRowIndx = csss.Count - 1;

            for (int i = 0; i < csss.Count; i++)
            {
                var css = csss[i];
                var scenarioFields = allScenarioEls.GetMergeFieldDict();
                foreach (var se in scenarioFields)
                {
                    string replaceVal;
                    switch (se.Key.Replace(" ", string.Empty).Replace(".", string.Empty))
                    {
                        case "ScenarioNo":
                            replaceVal = "Scenario " + (i + 1).ToString();
                            break;
                        case "ScenarioName":
                        case "ScenarioBriefDescription":
                            replaceVal = css.Scenario.BriefDescription;
                            break;
                        case "ScenarioFullDescription":
                            replaceVal = css.Scenario.BriefDescription;
                            break;
                    }
                }
                TableRow scenarioRoleRow = scenarioFields.First(k => k.Key.StartsWith("ScenarioRole"))
                    .First().FindFirstAncestor<TableRow>();

                var roles = css.CourseSlot.CourseScenarioFacultyRoles.ToLookup(k=>k.FacultyScenarioRole);
                CloneRows(scenarioRoleRow, roles, (mergeFieldName, role) =>
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

        private static T FindFirstAncestor<T>(this OpenXmlElement element) where T : OpenXmlElement
        {
            T found = null;

            while (found == null && element != null)
            {
                element = element.Parent;
                found = element as T;
            }
            return found;

        }

        private static Run CreateSimpleTextRun(string text)
        {
            Run returnVar = new Run();
            RunProperties runProp = new RunProperties();
            runProp.Append(new NoProof());
            returnVar.Append(runProp);
            returnVar.Append(new Text() { Text = text });
            return returnVar;
        }

        private static void InsertMergeFieldText(OpenXmlElement field, IList<string> listMembers)
        {
            var runs = new List<Run>(listMembers.Count);
            var withNext = new List<OpenXmlLeafElement>();
            foreach (var m in listMembers)
            {
                switch (m)
                {
                    case "\n":
                        withNext.Add(new Break());
                        break;
                    case "\t":
                        withNext.Add(new TabChar());
                        break;
                    default:
                        var run = CreateSimpleTextRun(m);
                        if (withNext.Any())
                        {
                            var t = run.ChildElements;
                            run.RemoveAllChildren();
                            run.Append(withNext.Concat(t));
                            withNext.Clear();
                        }
                        runs.Add(run);
                        break;
                }
            }
            //for now
            if (withNext.Any() && runs.Count > 0)
            {
                runs[runs.Count - 1].Append(withNext);
            }
            var sf = field as SimpleField;
            if (sf != null)
            {
                sf.GetFirstChild<Run>().Remove();
                sf.Append(runs);
            }
            else
            {
                var fc = (FieldCode)field;
                var existingRuns = GetAssociatedRuns(fc);
                var rEnd = existingRuns[existingRuns.Count - 1];
                foreach (var er in existingRuns
                    .SkipWhile(er => !er.ContainsCharType(FieldCharValues.Separate))
                    .Skip(1)
                    .TakeWhile(er => er != rEnd))
                {
                    er.Remove();
                }
                foreach (var r in runs)
                {
                    rEnd.InsertBeforeSelf(r);
                }
                //fc.Append
            }
        }

        private static void InsertMergeFieldText(OpenXmlElement field, string replacementText)
        {
            InsertMergeFieldText(field, replacementText.SplitAndInclude(new[] { '\t', '\n' }));
        }

        private static IList<Run> GetAssociatedRuns(FieldCode fieldCode)
        {
            //adapted from http://stackoverflow.com/questions/8400152/how-can-i-put-a-content-in-a-mergefield-in-docx
            Run rFieldCode = (Run)fieldCode.Parent;
            Run rBegin = rFieldCode.PreviousSibling<Run>();
            Run rCurrent = rFieldCode.NextSibling<Run>();

            var runs = new List<Run>(new[] { rBegin, rCurrent });

            while (!rCurrent.ContainsCharType(FieldCharValues.End))
            {
                rCurrent = rCurrent.NextSibling<Run>();
                runs.Add(rCurrent);
            };

            return runs;
        }

        private static bool ContainsCharType(this Run run, FieldCharValues fieldCharType)
        {
            var fc = run.GetFirstChild<FieldChar>();
            return fc == null
                ? false
                : fc.FieldCharType.Value == fieldCharType;
        }
    }

    internal class TimetableRow
    {
        public DateTime LocalStart { get; set; }
        public string SlotName { get; set; }
        public bool IsScenario { get; set; }
        public IEnumerable<string> Faculty { get; set; }
    }
}
