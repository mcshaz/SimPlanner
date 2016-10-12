using Draw = DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using SP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System;

namespace SP.Dto.Utilities
{
    public static class CreateCertificates
    {
        public static string CertificateName(Course course) {
            return CreateDocxTimetable.CourseNameWithoutExt(course) + " Certificates.pptx";
        }
        public static DbQuery<Course> GetCourseIncludes(MedSimDtoRepository repo)
        {
            return GetCourseIncludes(repo.Context);
        }
        public static DbQuery<Course> GetCourseIncludes(MedSimDbContext context)
        {
            return context.Courses.Include("CourseParticipants.Participant.ProfessionalRole")
                        .Include("CourseParticipants.Participant.Department.Institution")
                        .Include("Department.Institution.Culture")
                        .Include("CourseFormat.CourseType");
        }

        /*

public static Stream CreateCertificateZip(Course course, string sourceFile)
{
    var returnVar = new MemoryStream();
    using (ZipArchive archive = new ZipArchive(returnVar, ZipArchiveMode.Create, true))
    {
        var baseName = Path.GetFileNameWithoutExtension(CertificateName(course));
        var docfile = archive.CreateEntry(baseName + ".docx");
        string csvFileName = baseName + ".csv";
        using (var archEntry = docfile.Open())
        {
            using (var docStream = OpenTemplateAndChangeSource(course, sourceFile, csvFileName))
            {
                docStream.Position = 0;
                docStream.CopyTo(archEntry);
            }
        }
        var csvfile = archive.CreateEntry(csvFileName);
        using (var archEntry = csvfile.Open())
        {
            using (var tw = new StreamWriter(archEntry, Encoding.UTF8, 1028, true))
            {
                const char sepChar = ','; //eventually get this from docx
                string sepString = new string(sepChar, 1);
                string replaceInlineSep = '\\' + sepString;


            }
        }
    }
    return returnVar;
}

internal static string SqlString(Course course)
{
    var returnVar = new StringBuilder();
    var select = "' AS FullName, '" +
        course.CourseFormat.CourseType.Description + "' AS CourseTypeDescription, '" +
        course.CourseFormat.CourseType.Abbreviation + "' AS CourseTypeAbbreviation, '" +
        course.CourseFormat.Description + "' AS CourseFormatDescription, '" +
        course.Department.Name + "' AS DepartmentName, '" +
        course.Department.Abbreviation + "' AS DepartmentAbbreviation, '" +
        course.Department.Institution.Name + "' AS InstitutionName, '" +
        course.Department.Institution.Abbreviation + "' AS InstitutionAbbreviation, '" +
        course.StartLocal.ToString("yyyy-MM-dd HH:mm:ss") + "' AS StartDate";
    bool isFirst = true;
    foreach (var cp in course.CourseParticipants)
    {
        if (!isFirst)
        {
            returnVar.Append(" UNION ");
        } else
        {
            isFirst = false;
        }
        returnVar.Append(" SELECT '");
        returnVar.Append(cp.Participant.FullName.Replace("'", "''"));
        returnVar.Append(select);
    }
    return returnVar.ToString();
}

internal static MemoryStream OpenWordTemplateAndChangeSource(Course course,string sourceFile, string csvFileName)
{
byte[] byteArray = File.ReadAllBytes(sourceFile);
MemoryStream stream = new MemoryStream();
stream.Write(byteArray, 0, byteArray.Length);
using (WordprocessingDocument document = WordprocessingDocument.Open(stream, true))
{
var settings = document.MainDocumentPart.DocumentSettingsPart;
var mergeDataSource = settings.ExternalRelationships
    .First(er => er.RelationshipType.EndsWith("relationships/mailMergeSource"));

settings.DeleteExternalRelationship(mergeDataSource);

//container.AddExternalRelationship(mergeDataSource.RelationshipType, new Uri(csvFileName, UriKind.Relative),mergeDataSource.Id);
var oldFileLinkName = mergeDataSource.Uri.Segments[mergeDataSource.Uri.Segments.Length-1];
var mailMerge = settings.Settings.Descendants<MailMerge>().First();
var dataType = mailMerge.Descendants<DataType>().First();
dataType.Val = MailMergeDataValues.Query;
mailMerge.Query.Val = SqlString(course);
//mailMerge.DataSourceReference.Id = null;
}
return stream;
}

var replaceDict = new Dictionary<string, string>() {
    ["CourseTypeDescription"] = course.CourseFormat.CourseType.Description,
    ["CourseTypeAbbreviation"] = course.CourseFormat.CourseType.Abbreviation,
    ["CourseFormatDescription"] = course.CourseFormat.Description,
    ["DepartmentName"] = course.Department.Name,
    ["DepartmentAbbreviation"] = course.Department.Abbreviation,
    ["InstitutionName"] = course.Department.Institution.Name,
    ["InstitutionAbbreviation"] = course.Department.Institution.Abbreviation
};
*/
        const string organiserName = "OrganiserName";
        const string organiserRole = "OrganiserRole";
        public static MemoryStream CreatePptxCertificates(Course course, string sourceFile)
        {
            const string fullNameTxt = "FullName";
            byte[] byteArray = File.ReadAllBytes(sourceFile);
            MemoryStream stream = new MemoryStream();
            stream.Write(byteArray, 0, byteArray.Length);
            using (PresentationDocument document = PresentationDocument.Open(stream, true))
            {
                var master1LayoutPart = document.PresentationPart.SlideMasterParts.First()
                    .SlideLayoutParts.First();

                #region MasterSlide
                var formattedDate = course.StartLocal.ToString("dddd, MMMM dd, yyyy", course.Department.Institution.Culture.CultureInfo);

                var organisers = course.CourseParticipants.Where(cp => cp.IsOrganiser).ToList();
                var organiserCells = new List<OrganiserXml>(organisers.Count);
                organiserCells.Add(new OrganiserXml());
                Draw.Text fullName = null;
                foreach (var t in master1LayoutPart.SlideLayout.Descendants<Draw.Text>())
                {
                    t.Text = t.Text
                    .Replace("CourseTypeDescription", course.CourseFormat.CourseType.Description)
                    .Replace("CourseTypeAbbreviation", course.CourseFormat.CourseType.Abbreviation)
                    .Replace("CourseFormatDescription", course.CourseFormat.Description)
                    .Replace("DepartmentName", course.Department.Name)
                    .Replace("DepartmentAbbreviation", course.Department.Abbreviation)
                    .Replace("InstitutionName", course.Department.Institution.Name)
                    .Replace("InstitutionAbbreviation", course.Department.Institution.Abbreviation)
                    .Replace("StartDate", formattedDate);

                    if (t.Text.Contains(organiserName))
                    {
                        organiserCells[0].NameCell = t.FindFirstAncestor<Draw.TableCell>();
                    }
                    if (t.Text.Contains(organiserRole))
                    {
                        organiserCells[0].RoleCell = t.FindFirstAncestor<Draw.TableCell>();
                    }
                    if (t.Text.Contains(fullNameTxt)) { fullName = t; }
                }
                if (organisers.Count > 0)
                {
                    var parentTable = (organiserCells[0].NameCell ?? organiserCells[0].RoleCell).FindFirstAncestor<Draw.Table>();
                    var grid = parentTable.ChildElements<Draw.TableGrid>().First();
                    var gridCols = grid.ChildElements<Draw.GridColumn>().ToList();

                    var tableWidth = gridCols.Sum(gc => gc.Width);
                    var requiredColWidths = (tableWidth / organisers.Count);

                    //assuming for now table with 1 col
                    for (int i = 1; i < organisers.Count; i++)
                    {
                        grid.AppendChild(new Draw.GridColumn() { Width = requiredColWidths });
                        organiserCells.Add(organiserCells[0].Clone());
                    }

                    for (int i = 0; i < organisers.Count; i++)
                    {
                        organiserCells[i].SetText(organisers[i].Participant.FullName, organisers[i].Participant.ProfessionalRole.Description);
                    }
                };
                #endregion //MasterSlide
                #region certificates
                var participants = (from cp in course.CourseParticipants
                                    where !cp.IsFaculty
                                    orderby cp.Participant.FullName
                                    select cp.Participant.FullName).ToList();

                //find equivalent id in first slide
                document.PresentationPart.DeleteParts(document.PresentationPart.SlideParts);
                SlideIdList slideIdList = document.PresentationPart.Presentation.SlideIdList;
                foreach (var c in slideIdList.ChildElements)
                {
                    c.Remove();
                }
                document.PresentationPart.Presentation.Save();

                foreach (var part in participants)
                {
                    IEnumerable<Shape> placeholderShapes;
                    AppendNewSlide(document.PresentationPart, master1LayoutPart, out placeholderShapes);

                    foreach (var r in (from phs in placeholderShapes
                                         from dr in phs.Descendants<Draw.Run>()
                                         where dr.Text.Text.IndexOf(fullNameTxt, StringComparison.OrdinalIgnoreCase) > -1
                                         select dr))
                    {
                        //should probably be using innerText and xml powertools regex replace
                        r.Text.Text = r.Text.Text.Replace(fullNameTxt, part);
                    }
                }

                document.PresentationPart.Presentation.Save();

                //find equivalent id in 1st slide

                #endregion //certificates


            }
            return stream;
        }

        internal class OrganiserXml
        {
            public Draw.TableCell NameCell { get; set; }
            public Draw.TableCell RoleCell { get; set; }
            public void SetText(string Name, string Role)
            {
                if (NameCell != null)
                {
                    foreach (var t in NameCell.Descendants<Draw.Text>())
                    {
                        t.Text = t.Text.Replace(organiserName, Name);
                    }
                }
                if (RoleCell != null)
                {
                    foreach (var t in RoleCell.Descendants<Draw.Text>())
                    {
                        t.Text = t.Text.Replace(organiserRole, Role);
                    }
                }
            }
            public OrganiserXml Clone()
            {
                var returnVar = new OrganiserXml();
                if (NameCell != null)
                {
                    returnVar.NameCell = (Draw.TableCell)NameCell.CloneNode(true);
                }
                if (RoleCell != null)
                {
                    returnVar.RoleCell = (Draw.TableCell)RoleCell.CloneNode(true);
                }
                return returnVar;
            }
        }

        //http://stackoverflow.com/questions/32076114/c-sharp-openxml-sdk-2-5-insert-new-slide-from-slide-masters-with-the-layout
        public static SlidePart AppendNewSlide(PresentationPart presentationPart, SlideLayoutPart masterLayoutPart, out IEnumerable<Shape> placeholderShapes)
        {
            Slide clonedSlide = new Slide() {
                ColorMapOverride = new ColorMapOverride {
                    MasterColorMapping = new Draw.MasterColorMapping()
                }
            };

            SlidePart clonedSlidePart = presentationPart.AddNewPart<SlidePart>();
            clonedSlidePart.Slide = clonedSlide;
            clonedSlidePart.AddPart(masterLayoutPart);
            clonedSlide.Save(clonedSlidePart);

            var masterShapeTree = masterLayoutPart.SlideLayout.CommonSlideData.ShapeTree;

            placeholderShapes = (from s in masterShapeTree.ChildElements<Shape>()
                                   where s.NonVisualShapeProperties.OfType<ApplicationNonVisualDrawingProperties>().Any(anvdp=>anvdp.PlaceholderShape != null)
                                   select new Shape()
                                   {
                                       NonVisualShapeProperties = (NonVisualShapeProperties)s.NonVisualShapeProperties.CloneNode(true),
                                       TextBody = new TextBody(s.TextBody.ChildElements<Draw.Paragraph>().Select(p => p.CloneNode(true))) {
                                           BodyProperties = new Draw.BodyProperties(),
                                           ListStyle = new Draw.ListStyle()
                                       },
                                       ShapeProperties = new ShapeProperties()
                                   }).ToList();

            clonedSlide.CommonSlideData = new CommonSlideData
            {
                ShapeTree = new ShapeTree(placeholderShapes) {
                    GroupShapeProperties = (GroupShapeProperties)masterShapeTree.GroupShapeProperties.CloneNode(true),
                    NonVisualGroupShapeProperties = (NonVisualGroupShapeProperties)masterShapeTree.NonVisualGroupShapeProperties.CloneNode(true)
                }
            };

            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

            // Find the highest slide ID in the current list.
            uint maxSlideId = slideIdList.Max(c=>(uint?)((SlideId)c).Id) ?? 256;

            // Insert the new slide into the slide list after the previous slide.
            slideIdList.Append(new SlideId() {
                Id = ++maxSlideId,
                RelationshipId = presentationPart.GetIdOfPart(clonedSlidePart)
            });
            //presentationPart.Presentation.Save();

            return clonedSlidePart;
        }
    }
}
