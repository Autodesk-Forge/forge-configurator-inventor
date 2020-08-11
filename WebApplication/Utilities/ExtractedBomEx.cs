using System;
using System.Linq;
using System.Text;
using Shared;

namespace WebApplication.Utilities
{
    public static class ExtractedBomEx
    {
        /// <summary>
        /// Convert BOM to CSV representation.
        /// </summary>
        /// <returns>CSV string.</returns>
        public static string ToCSV(this ExtractedBOM bom)
        {
            if (! bom.HasColumns()) throw new ApplicationException("Invalid BOM: header is expected.");

            var columnsLength = bom.Columns.Length;

            var builder = new StringBuilder(32 * 1024);
            builder.AppendJoin(",", bom.Columns.Select(column => Encode(column.Label)));
            builder.AppendLine();

            for (var i = 0; i < bom.Data?.Length; i++)
            {
                object[] row = bom.Data[i];
                if (row.Length != columnsLength)
                    throw new ApplicationException(
                        $"Invalid BOM: row {i} has different number of columns than header.");

                builder.AppendJoin(",", row.Select(value => Encode(value.ToString())));
                builder.AppendLine();
            }

            return builder.ToString();
        }

        public static bool HasColumns(this ExtractedBOM bom)
        {
            return bom.Columns?.Length > 0;
        }

        public static bool HasData(this ExtractedBOM bom)
        {
            return bom.Data?.Length > 0 && bom.Data?[0].Length > 0;
        }

        private static string Encode(string value)
        {
            // - Fields with embedded commas or double-quote characters must be quoted
            if (value.Contains(","))
            {
                return "\"" +
                       // - Each of the embedded double-quote characters must be represented by a pair of double-quote characters
                       value.Replace("\"", "\"\"") +
                       "\"";
            }
            else
            {
                return value;
            }
        }
    }
}
