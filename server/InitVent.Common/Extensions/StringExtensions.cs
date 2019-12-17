using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InitVent.Common.Extensions
{
    public static class StringExtensions
    {
        public static String ToTitleCaseUsingCulture(this String s, CultureInfo culture = null, bool ignoreInputCase = true)
        {
            var textInfo = (culture ?? CultureInfo.CurrentUICulture).TextInfo;

            return textInfo.ToTitleCase(ignoreInputCase ? textInfo.ToLower(s) : s);
        }

        public static String ToTitleCase(this String s, Predicate<String> isReservedWord = null)
        {
            isReservedWord = isReservedWord ?? (str => false);

            return Regex.Replace(s, @"\w+", match =>
                isReservedWord(match.Value) ? match.Value : match.Value.Substring(0, 1).ToUpper() + match.Value.Substring(1, match.Value.Length - 1).ToLower()
            );

            // Alternative case conversion:
            //return Regex.Replace(s, @"\w+", match => isReservedWord(match.Value) ? match.Value : ToTitleCaseUsingCulture(match.Value, ignoreInputCase: true));
        }

        public static String ToTitleCase(this String s, IEnumerable<String> reservedWords)
        {
            return ToTitleCase(s, word => reservedWords.Contains(word));
        }
    }
}
