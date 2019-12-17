using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InitVent.Common.Extensions;

namespace InitVent.Common.Serialization
{
    public class CsvSerializer
    {
        public String ValueDelimiter { get; private set; }
        public String RowDelimiter { get; private set; }
        public String QuoteCharacter { get; private set; }

        private static readonly String BaseValuePattern = @"\G(?=.)(?<value>'([^']|'')*'|[^,'\r\n]*)(,|(?<eor>\r?\n|(?<eof>\z))+)";

        private Regex ValueRegex
        {
            get
            {
                var pattern = BaseValuePattern
                    .Replace("'", Regex.Escape(QuoteCharacter))
                    .Replace(",", Regex.Escape(ValueDelimiter));

                if (RowDelimiter != null)
                {
                    pattern = pattern
                        .Replace("\r\n", Regex.Escape(RowDelimiter))
                        .Replace("\r?\n", Regex.Escape(RowDelimiter));
                }

                return new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            }
        }

        /// <param name="valueDelimiter">The delimiter to use to separate values.</param>
        /// <param name="rowDelimiter">The delimiter to use to separate rows, or <code>null</code> to use an environment-sensitive newline.</param>
        /// <param name="quoteCharacter">The delimiter to use to quote values.</param>
        public CsvSerializer(String valueDelimiter = ",", String rowDelimiter = null, String quoteCharacter = "\"")
        {
            ValueDelimiter = valueDelimiter;
            RowDelimiter = rowDelimiter;
            QuoteCharacter = quoteCharacter;
        }

        protected String QuoteAndEscape(String value)
        {
            return QuoteCharacter + value.Replace(QuoteCharacter, QuoteCharacter + QuoteCharacter) + QuoteCharacter;
        }

        public String Serialize<TRow>(IEnumerable<TRow> tabularData, Func<TRow, IEnumerable<String>> rowToString, IEnumerable<String> headers = null)
        {
            var data = tabularData.Select(rowToString);
            if (headers != null)
                data.Prepend(headers);

            return Serialize(data);
        }

        public String Serialize<T>(T[,] tabularData, Func<T, String> valueToString = null)
        {
            return Serialize(tabularData.GetRows(), valueToString);
        }

        public String Serialize<T>(IEnumerable<IEnumerable<T>> tabularData, Func<T, String> valueToString = null)
        {
            if (valueToString == null)
                valueToString = value => (value == null ? String.Empty : value.ToString());

            return String.Join(RowDelimiter ?? Environment.NewLine,
                tabularData.Select(rowValues => String.Join(ValueDelimiter, rowValues.Select(value => QuoteAndEscape(valueToString(value))))));
        }

        public T[][] Deserialize<T>(String data, Func<String, T> constructor = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (String.IsNullOrWhiteSpace(data))
                return new T[][] { };

            var matches = ValueRegex.Matches(data).Cast<Match>().ToArray();

            // Verify that entire input was matched
            if (!matches.Any() || !matches.Last().Groups["eof"].Success)
                throw new FormatException("Input data string was not in a format recognized by this serializer.");

            if (constructor == null)
                constructor = value => (T)Convert.ChangeType(value, typeof(T));

            var rowBuffer = new List<T[]>();
            var valueBuffer = new List<T>();

            foreach (var match in matches)
            {
                valueBuffer.Add(constructor(match.Groups["value"].Value));

                if (match.Groups["eor"].Success)
                {
                    rowBuffer.Add(valueBuffer.ToArray());
                    valueBuffer.Clear();
                }
            }

            return rowBuffer.ToArray();
        }
    }
}
