namespace SP.Dto.Utilities
{
    static class StringExtensions
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
    }
}
