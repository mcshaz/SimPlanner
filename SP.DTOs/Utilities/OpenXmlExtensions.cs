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
        private const string _fieldTerminator = "\\*";
        private const string _fieldBeforeText = "\\b";
        private const string _fieldAfterText = "\\f";

        private static readonly string[] _fieldCmds = new[] { _fieldTerminator, _fieldBeforeText, _fieldAfterText };
        private const string _mergefield = "MERGEFIELD";

        public static ILookup<string, OpenXmlElement> GetMergeFieldDict(this IEnumerable<OpenXmlElement> elements)
        {
            var allFields = elements.SelectMany(x => x.Descendants<SimpleField>().Select(sf => new { text = sf.Instruction.Value, el = (OpenXmlElement)sf })
                        .Concat(x.Descendants<FieldCode>().Select(fc => new { text = fc.Text, el = (OpenXmlElement)fc })));
            return (from fc in allFields
                    let words = fc.text.TrimStart().Split(null,2)
                    where _mergefield.Equals(words.FirstOrDefault(), StringComparison.OrdinalIgnoreCase)
                    select new { text = words[1].FirstWord(), el= fc.el })
                    .ToLookup(k => k.text, v => v.el);
        }

        public static void CloneElement<T>(this OpenXmlElement cloneSource, IEnumerable<T> itemCollection, Func<string, T, string> withMergeField)
        {
            CloneElements(new[] { cloneSource }, itemCollection, (s, t, dummy) => withMergeField(s, t));
        }

        public static void CloneElements<T>(this IList<OpenXmlElement> cloneSource, IEnumerable<T> itemCollection, Func<string, T, IEnumerable<OpenXmlElement>, string> withMergeField)
        {
            IList<T> itemList = itemCollection as IList<T>;
            if (itemList == null)
            {
                itemList = itemCollection.ToList();
            }
            if (itemList.Count == 0)
            {
                foreach (var c in cloneSource)
                {
                    if (c.Parent != null)
                    {
                        c.Remove();
                    }
                }
                return;
            }

            var parent = cloneSource.First().Parent;
            var followingSibling = cloneSource[cloneSource.Count-1].NextSibling();
            /*
            if (parent is TableRow)
            {
                parent = parent.FindFirstAncestor<Table>();
            }
            */

            var virginClonable = cloneSource.Select(c=>c.CloneNode(true)).ToList();

            int lastRowIndex = itemList.Count - 1;
            for (int i = 0; i < itemList.Count; i++)
            {
                var mergeFieldDict = cloneSource.GetMergeFieldDict();
                foreach (var sf in mergeFieldDict)
                {
                    string replaceVal = withMergeField(sf.Key.Replace(" ", string.Empty).Replace(".", string.Empty), itemList[i], sf);
                    if (replaceVal != null)
                    {
                        foreach (var s in sf)
                        {
                            InsertMergeFieldText(s, replaceVal);
                        }
                    }
                }
                if (i > 0) //on 1st run we are adjusting the current row, so no need to insert
                {
                    foreach (var c in cloneSource)
                    {
                        if (followingSibling == null)
                        {
                            parent.AppendChild(c);
                        }
                        else
                        {
                            parent.InsertBefore(c, followingSibling);
                        }
                    }
                    
                }
                if (i < lastRowIndex)
                {
                    cloneSource = virginClonable.Select(c => c.CloneNode(true)).ToList();
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
            var leafs = new List<OpenXmlLeafElement>(listMembers.Count);
            foreach (var m in listMembers)
            {
                switch (m)
                {
                    case "\n":
                        leafs.Add(new Break());
                        break;
                    case "\t":
                        leafs.Add(new TabChar());
                        break;
                    default:
                        leafs.Add(new Text(m));
                        break;
                }
            }
            Run textRun;
            //have to try and preserve formatting - therefore need to insert text into 1st run
            var sf = field as SimpleField;
            if (sf != null)
            {
                foreach (var l in leafs.OfType<Text>())
                {
                    l.Text = InterpretFieldInstructions(l.Text, sf.Instruction);
                }
                var allTextRuns = sf.ChildElements<Run>().ToList(); ;
                textRun = allTextRuns.GetTextRun();
                if (textRun == null)
                {
                    textRun = CreateSimpleRun();
                    sf.Append(textRun);
                }
                foreach (var r in allTextRuns.Where(ar=>ar != textRun))
                {
                    r.Remove();
                }
            }
            else
            {
                var fc = (FieldCode)field;
                
                foreach (var l in leafs.OfType<Text>())
                {
                    l.Text = InterpretFieldInstructions(l.Text, fc.InnerText);
                }
                var existingRuns = GetAssociatedRuns(fc);
                var textRuns = existingRuns.Skip(2)
                    .SkipWhile(er => !er.ContainsCharType(FieldCharValues.Separate))
                    .Skip(1).ToList();
                textRuns.RemoveAt(textRuns.Count - 1);
                textRun = textRuns.GetTextRun();
                if (textRun == null)
                {
                    textRun = CreateSimpleRun();
                    var rEnd = existingRuns[existingRuns.Count - 1];
                    rEnd.InsertBeforeSelf(textRun);
                }

                foreach (var er in textRuns.Where(tr=>tr!= textRun))
                {
                    er.Remove();
                }
                //fc.Append
            }
            textRun.ChildElementsNot<RunProperties>().ToList()
                .ForEach(c=>c.Remove());
            /*
            if (leafs.Count == 0)
            {
                leafs.Add(new Text(string.Empty));
            }
            */
            textRun.Append(leafs);
        }

        private static string InterpretFieldInstructions(string str, string inst)
        {
            if (string.IsNullOrWhiteSpace(inst)) { return str; }
            int instStart = inst.IndexOfWord(3);
            int instFinish = inst.LastIndexOf(_fieldTerminator);
            if (instStart == instFinish) { return str; }
            inst = inst.Substring(instStart, instFinish - instStart);
            bool after = false;
            bool before = false;
            foreach (var c in inst.SplitAndInclude(_fieldCmds))
            {
                switch (c)
                {
                    case _fieldAfterText:
                        after = true;
                        break;
                    case _fieldBeforeText:
                        before = true;
                        break;
                    default:
                        if (after)
                        {
                            str += GetText(c);
                            after = false;
                        }
                        else if (before)
                        {
                            str = GetText(c) + str;
                            before = false;
                        }
                        break;
                }
            }
            return str;
        }

        private static string GetText(string str)
        {
            str = str.Trim();
            if (str[0] == '"')
            {
                str = str.Substring(1, str.Length - 2);
            }
            return str;
        }

        private static Run GetTextRun(this IEnumerable<Run> runs)
        {
            return runs.FirstOrDefault(er => !string.IsNullOrWhiteSpace(er.InnerText))
                ?? runs.FirstOrDefault(er => er.GetFirstChild<Text>() != null);
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

            var runs = new List<Run>(new[] { rBegin, rFieldCode, rCurrent });

            while (!rCurrent.ContainsCharType(FieldCharValues.End))
            {
                rCurrent = rCurrent.NextSibling<Run>();
                runs.Add(rCurrent);
            };

            return runs;
        }

        public static IEnumerable<T> ChildElements<T>(this OpenXmlElement el) where T: OpenXmlElement
        {
            if (el.HasChildren)
            {
                var child = el.GetFirstChild<T>();
                while (child != null)
                {
                    yield return child;
                    child = child.NextSibling<T>();
                }
            }
        }

        public static IEnumerable<OpenXmlElement> ChildElementsNot<T>(this OpenXmlElement el) where T : OpenXmlElement
        {
            if (el.HasChildren)
            {
                foreach(var c in el.ChildElements)
                {
                    if (c.GetType() != typeof(T))
                    {
                        yield return c;
                    }
                }
            }
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
