using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Dto.Utilities
{
    public static class StringExtensions
    {
        public static string PascalToCamelCase(this string instr)
        {
            if (string.IsNullOrEmpty(instr)) { return instr; }
            return char.ToLowerInvariant(instr[0]) + instr.Substring(1);
        }

        public static string CamelToPascalCase(this string instr)
        {
            if (string.IsNullOrEmpty(instr)) { return instr; }
            return char.ToUpperInvariant(instr[0]) + instr.Substring(1);
        }

        public static string ToSeparateWords(this string instr)
        {
            return System.Text.RegularExpressions.Regex.Replace(instr, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");
        }

        //http://stackoverflow.com/questions/7148768/string-split-by-index-params
        public static string[] SplitAt(this string source, IList<int> index)
        {
            string[] output = new string[index.Count + 1];
            int pos = 0;

            for (int i = 0; i < index.Count; pos = index[i++])
            {
                output[i] = source.Substring(pos, index[i] - pos);
            }

            output[index.Count] = source.Substring(pos);
            return output;
        }

        public static IList<int> IndexesOf(this string s, char value)
        {
            var foundIndexes = new List<int>();

            for (int i = s.IndexOf(value); i > -1; i = s.IndexOf(value, i + 1))
            {
                foundIndexes.Add(i);
            }
            return foundIndexes;
        }

        //ms code referencesource.microsoft.com/#mscorlib/system/string.cs,10c5b3e0ef2b8a70
        //if performance critical, run along the pointer
        public static IList<string> SplitAndInclude(this string s, char[] anyOf)
        {
            var returnVar = new List<string>();
            int nextStart = 0;
            int i;
            for (i=0;i<s.Length;i++)
            {
                if (anyOf.Contains(s[i]))
                {
                    if (nextStart!=i) 
                    {
                        returnVar.Add(s.Substring(nextStart, i- nextStart));
                    }
                    returnVar.Add(s[i].ToString());
                    nextStart = i+1;
                }
            }
            if (nextStart < i)
            {
                returnVar.Add(s.Substring(nextStart, i - nextStart));
            }
            return returnVar;
        }

    }

}
