using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Dto
{
    public static class OpenXmlExtensions
    {
        public static ILookup<string, OpenXmlElement> GetMergeFieldDict(this IEnumerable<OpenXmlElement> elements)
        {
            var splitter = new[] { ' ', '"' };
            const string mergefield = "MERGEFIELD";
            return elements.SelectMany(x => x.Descendants<SimpleField>().Select(sf => new { text = sf.Instruction.Value, el = (OpenXmlElement)sf })
                    .Concat(x.Descendants<FieldCode>().Select(fc => new { text = fc.Text, el = (OpenXmlElement)fc })))
                .Select(a => new { words = a.text.Split(splitter, StringSplitOptions.RemoveEmptyEntries), el = a.el })
                .Where(a => mergefield.Equals(a.words.FirstOrDefault(), StringComparison.OrdinalIgnoreCase))
                .ToLookup(k => string.Join(" ", k.words.Skip(1).TakeWhile(i => i != "\\*")), v => v.el);
        }

        public static void CloneElement<T>(this OpenXmlElement cloneSource, IEnumerable<T> itemCollection, Func<string, T, string> withMergeField)
        {
            CloneElements(new[] { cloneSource }, itemCollection, (s, t, dummy) => withMergeField(s, t));
        }

        public static void CloneElements<T>(this IList<OpenXmlElement> cloneSource, IEnumerable<T> itemCollection, Func<string, T, ILookup<string, OpenXmlElement>, string> withMergeField)
        {
            List<T> itemList = itemCollection as List<T>;
            if (itemList == null)
            {
                itemList = itemCollection.ToList();
            }
            if (itemList.Count == 0)
            {
                foreach (var c in cloneSource)
                {
                    c.Remove();
                }
                return;
            }

            var parent = cloneSource.First().Parent;
            var lastSibling = cloneSource.Last().NextSibling();
            if (parent is TableRow)
            {
                parent = parent.FindFirstAncestor<Table>();
            }

            var virginClonable = cloneSource.Select(c=>c.CloneNode(true)).ToList();

            int lastRowIndex = itemList.Count - 1;
            for (int i = 0; i < itemList.Count; i++)
            {
                var mergeFieldDict = cloneSource.GetMergeFieldDict();
                foreach (var sf in mergeFieldDict)
                {
                    string replaceVal = withMergeField(sf.Key.Replace(" ", string.Empty).Replace(".", string.Empty), itemList[i], mergeFieldDict);
                    foreach (var s in sf)
                    {
                        InsertMergeFieldText(s, replaceVal);
                    }
                }
                if (i > 0)
                {
                    foreach (var c in cloneSource)
                    {
                        if (lastSibling == null)
                        {
                            parent.AppendChild(c);
                        }
                        else
                        {
                            parent.InsertBefore(c, lastSibling);
                        }
                    }
                    
                }
                if (i < lastRowIndex)
                {
                    cloneSource = cloneSource.Select(c => c.CloneNode(true)).ToList();
                }
                else
                {
                    cloneSource = virginClonable;
                }
            }
        }
        public static T FindFirstAncestor<T>(this OpenXmlElement element) where T : OpenXmlElement
        {
            T found = null;
            while (found == null && element != null)
            {
                element = element.Parent;
                found = element as T;
            }
            return found;
        }

        private static Run CreateSimpleRun()
        {
            Run returnVar = new Run();
            RunProperties runProp = new RunProperties();
            runProp.Append(new NoProof());
            returnVar.Append(runProp);
            return returnVar;
        }

        private static Run CreateSimpleTextRun(string text)
        {
            var returnVar = CreateSimpleRun();
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
                        var run = CreateSimpleRun();
                        run.Append(withNext);
                        run.Append(new Text { Text=m });
                        withNext.Clear();
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
                sf.RemoveAllChildren();
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

        public static void InsertMergeFieldText(this OpenXmlElement field, string replacementText)
        {
            InsertMergeFieldText(field, replacementText.SplitAndInclude(new[] { '\t', '\n' }));
        }

        public static IList<Run> GetAssociatedRuns(this FieldCode fieldCode)
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

        public static bool ContainsCharType(this Run run, FieldCharValues fieldCharType)
        {
            var fc = run.GetFirstChild<FieldChar>();
            return fc == null
                ? false
                : fc.FieldCharType.Value == fieldCharType;
        }
    }
}
