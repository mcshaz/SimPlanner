using System.Collections.Generic;

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

        public static string[] SplitAndInclude(this string s, string[] separator)
        {

            if (s == null || s.Length == 0)
            {
                return new string[0];
            }

            int[] sepList = new int[s.Length];
            int[] lengthList = new int[s.Length];

            int numReplaces = MakeSeparatorList(s, separator, ref sepList, ref lengthList);

            //Handle the special case of no replaces and special count.
            if (0 == numReplaces)
            {
                return new string[] { s };
            }

            return InternalSplitKeepEmptyEntries(s, sepList, lengthList, numReplaces);
        }
        //ms code referencesource.microsoft.com/#mscorlib/system/string.cs,10c5b3e0ef2b8a70
        public static string[] SplitAndInclude(this string s, char[] separator)
        {
            if (s == null || s.Length == 0)
            {
                return new string[0];
            }

            int[] sepList = new int[s.Length];
            int numReplaces = MakeSeparatorList(s, separator, ref sepList);

            //Handle the special case of no replaces and special count.
            if (0 == numReplaces)
            {
                return new string[] { s };
            }

            return InternalSplitKeepEmptyEntries(s, sepList, null,numReplaces);
        }

        // Note a few special case in this function:
        //     If there is no separator in the string, a string array which only contains 
        //     the original string will be returned regardless of the count. 
        //

        private static string[] InternalSplitKeepEmptyEntries(string s, int[] sepList, int[] lengthList, int numReplaces)
        {
            int currIndex = 0;
            int arrIndex = 0;

            //Allocate space for the new array.
            //+1 for the string from the end of the last replace to the end of the String.
            int maxItems = numReplaces * 2 + 1;
            string[] splitStrings = new string[maxItems];

            for (int i = 0; i < numReplaces && currIndex < s.Length; i++)
            {
                if (sepList[i] - currIndex > 0)
                {
                    splitStrings[arrIndex++] = s.Substring(currIndex, sepList[i] - currIndex);
                }

                if (lengthList == null)
                {
                    splitStrings[arrIndex++] = new string(s[sepList[i]], 1);
                    currIndex = sepList[i] +  1;
                }
                else
                {
                    splitStrings[arrIndex++] = s.Substring(sepList[i], lengthList[i]);
                    currIndex = sepList[i] + lengthList[i];
                }

            }

            //Handle the last string at the end of the array if there is one.
            if (currIndex < s.Length)
            {
                splitStrings[arrIndex++] = s.Substring(currIndex);
            }

            if (arrIndex == maxItems)
            {
                return splitStrings;
            }

            var stringArray = new string[arrIndex];
            for (int j = 0; j < arrIndex; j++) //looping may be quicker than array.copy for < ~32 elements according to http://stackoverflow.com/questions/7483893/is-array-copy-faster-than-for-loop-for-2d-arrays
            {
                stringArray[j] = splitStrings[j];
            }
            return stringArray;
        }
        //--------------------------------------------------------------------    
        // This function returns number of the places within baseString where 
        // instances of characters in Separator occur.         
        // Args: separator  -- A string containing all of the split characters.
        //       sepList    -- an array of ints for split char indicies.
        //--------------------------------------------------------------------    
        private static unsafe int MakeSeparatorList(string s, char[] separator, ref int[] sepList)
        {
            int foundCount = 0;

            int sepListCount = sepList.Length;
            int sepCount = separator.Length;
            //If they passed in a string of chars, actually look for those chars.
            fixed (char* pwzChars = s, pSepChars = separator)
            {
                for (int i = 0; i < s.Length && foundCount < sepListCount; i++)
                {
                    char* pSep = pSepChars;
                    for (int j = 0; j < sepCount; j++, pSep++)
                    {
                        if (pwzChars[i] == *pSep)
                        {
                            sepList[foundCount++] = i;
                            break;
                        }
                    }
                }
            }
            return foundCount;
        }

        //--------------------------------------------------------------------    
        // This function returns number of the places within baseString where 
        // instances of separator strings occur.         
        // Args: separators -- An array containing all of the split strings.
        //       sepList    -- an array of ints for split string indicies.
        //       lengthList -- an array of ints for split string lengths.
        //-------------------------------------------------------------------- 
        private static unsafe int MakeSeparatorList(string s, string[] separators, ref int[] sepList, ref int[] lengthList)
        {
            int foundCount = 0;
            int sepListCount = sepList.Length;
            int sepCount = separators.Length;

            fixed (char* pwzChars = s)
            {
                for (int i = 0; i < s.Length && foundCount < sepListCount; i++)
                {
                    for (int j = 0; j < separators.Length; j++)
                    {
                        string separator = separators[j];
                        if (string.IsNullOrEmpty(separator))
                        {
                            continue;
                        }
                        int currentSepLength = separator.Length;
                        if (pwzChars[i] == separator[0] && currentSepLength <= s.Length - i)
                        {
                            if (currentSepLength == 1
                                || string.CompareOrdinal(s, i, separator, 0, currentSepLength) == 0)
                            {
                                sepList[foundCount] = i;
                                lengthList[foundCount] = currentSepLength;
                                foundCount++;
                                i += currentSepLength - 1;
                                break;
                            }
                        }
                    }
                }
            }
            return foundCount;
        }

        public static unsafe int IndexOfWord(this string str, int wordCount = 1)
        {
            if (str == null)
            {
                return -1;
            }
            int hits = 0;
            int index = 0;
            bool lastWasWhitespace = true;

            fixed (char* pfixed = str)
            {
                for (char* p = pfixed; *p != 0; p++)
                { 
                    switch (*p)
                    {
                        case '\u0020': case '\u00A0': case '\u1680': case '\u2000': case '\u2001': case '\u2002': case '\u2003': case '\u2004': case '\u2005': case '\u2006': case '\u2007': case '\u2008': case '\u2009': case '\u200A': case '\u202F': case '\u205F': case '\u3000': case '\u2028': case '\u2029': case '\u0009': case '\u000A': case '\u000B': case '\u000C': case '\u000D': case '\u0085':
                            lastWasWhitespace = true;
                            break;
                        default:
                            if (lastWasWhitespace)
                            {
                                ++hits;
                                if (hits == wordCount)
                                {
                                    return index;
                                }
                            }
                            lastWasWhitespace = false;
                            break;
                    }
                    ++index;
                }
            }
            return -1;
        }

        public static string FirstWord(this string str)
        {
            int start = IndexOfWord(str);
            if (start == -1)
            {
                return str==null
                    ?null
                    :string.Empty;
            }
            int end = start+1;
            while (end < str.Length && !char.IsWhiteSpace(str[end]))
            {
                ++end;
            }
            return str.Substring(start, end - start);
        }

        public static unsafe string MultiToSingleWhitespace(this string str, char whitespace=' ')
        {
            fixed (char* pfixed = str)
            {
                bool lastWasWhitespace = false;
                char* dst = pfixed;
                for (char* p = pfixed; *p != 0; p++)
                { 
                    switch (*p)
                    {
                        case '\u0020': case '\u00A0': case '\u1680': case '\u2000': case '\u2001': case '\u2002': case '\u2003': case '\u2004': case '\u2005': case '\u2006': case '\u2007': case '\u2008': case '\u2009': case '\u200A': case '\u202F': case '\u205F': case '\u3000': case '\u2028': case '\u2029': case '\u0009': case '\u000A': case '\u000B': case '\u000C': case '\u000D': case '\u0085':
                            if (!lastWasWhitespace)
                            {
                                *dst++ = whitespace;
                            }
                            lastWasWhitespace = true;
                            continue;
                        default:
                            *dst++ = *p;
                            lastWasWhitespace = false;
                            break;
                    }
                }
                return new string(pfixed, 0, (int)(dst - pfixed));
            }

        }
        public static unsafe string RemoveAllWhitespace(string str)
        {
            fixed (char* pfixed = str)
            {
                char* dst = pfixed;
                for (char* p = pfixed; *p != 0; p++)
                    switch (*p)
                    {
                        case '\u0020': case '\u00A0': case '\u1680': case '\u2000': case '\u2001': case '\u2002': case '\u2003': case '\u2004': case '\u2005': case '\u2006': case '\u2007': case '\u2008': case '\u2009': case '\u200A': case '\u202F': case '\u205F': case '\u3000': case '\u2028': case '\u2029': case '\u0009': case '\u000A': case '\u000B': case '\u000C': case '\u000D': case '\u0085':
                            continue;
                        default:
                            *dst++ = *p;
                            break;
                    }

                return new string(pfixed, 0, (int)(dst - pfixed));
            }

        }
    }

}
