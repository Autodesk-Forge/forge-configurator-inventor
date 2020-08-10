using System;
using System.Linq;
using System.Text;
using Shared;

namespace WebApplication.Utilities
{
    public static class ExtractedBomEx
    {
        private static readonly object[][] EmptyData = new object[0][];

        /// <summary>
        /// Convert BOM to CSV representation.
        /// </summary>
        /// <returns>CSV string.</returns>
        public static string ToCSV(this ExtractedBOM bom)
        {
            if (! bom.HasColumns()) throw new ApplicationException("Invalid BOM: header is expected.");

            var builder = new StringBuilder(32 * 1024);
            builder.AppendJoin(",", bom.Columns.Select(c => Encode(c.Label)));
            builder.AppendLine();

            foreach (object[] row in bom.Data ?? EmptyData)
            {
                builder.AppendJoin(",", row.Select(v => Encode(v.ToString())));
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
            return value; // not yet implemented
        }
    }
}
