using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FastMember;

namespace CLI
{
    public class ConsoleTable
    {
        public ConsoleTable(params string[] columns)
            : this(new ConsoleTableOptions { Columns = new List<string>(columns) })
        {
        }

        public ConsoleTable(ConsoleTableOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.Rows = new List<object[]>();
            this.Columns = new List<object>(options.Columns);
        }

        public IList<object> Columns { get; set; }
        public IList<object[]> Rows { get; protected set; }

        public ConsoleTableOptions Options { get; protected set; }

        public ConsoleTable AddColumn(IEnumerable<string> names)
        {
            foreach (var name in names)
                this.Columns.Add(name);
            return this;
        }

        public ConsoleTable AddRow(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (!this.Columns.Any())
                throw new Exception("Please set the columns first");

            if (this.Columns.Count != values.Length)
                throw new Exception(
                                    $"The number columns in the row ({this.Columns.Count}) does not match the values ({values.Length}");

            this.Rows.Add(values);
            return this;
        }

        public static ConsoleTable From<T>(IEnumerable<T> values, params string[] columns)
        {
            var table = new ConsoleTable();
            var cols = GetColumns<T>(columns).ToList();
            table.AddColumn(cols);
            foreach (var propertyValues in values.Select(x => ObjectAccessor.Create(x)).Select(value => cols.Select(column => value[column])))
                table.AddRow(propertyValues.ToArray());
            return table;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = this.ColumnLengths();

            // create the string format with padding
            var format = Enumerable.Range(0, this.Columns.Count)
                                   .Select(i => " | {" + i + ",-" + columnLengths[i] + "}")
                                   .Aggregate((s, a) => s + a) + " |";

            // find the longest formatted line
            var maxRowLength = Math.Max(0, this.Rows.Any() ? this.Rows.Max(row => string.Format(format, row).Length) : 0);
            var columnHeaders = string.Format(format, this.Columns.ToArray());

            // longest line is greater of formatted columnHeader and longest row
            var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

            // add each row
            var results = this.Rows.Select(row => string.Format(format, row)).ToList();

            // create the divider
            var divider = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

            builder.AppendLine(divider);
            builder.AppendLine(columnHeaders);

            foreach (var row in results)
            {
                builder.AppendLine(divider);
                builder.AppendLine(row);
            }

            builder.AppendLine(divider);

            if (this.Options.EnableCount)
            {
                builder.AppendLine("");
                builder.AppendFormat(" Count: {0}", this.Rows.Count);
            }

            return builder.ToString();
        }

        public string ToMarkDownString()
        {
            return this.ToMarkDownString('|');
        }

        private string ToMarkDownString(char delimiter)
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = this.ColumnLengths();

            // create the string format with padding
            var format = this.Format(columnLengths, delimiter);

            // find the longest formatted line
            var columnHeaders = string.Format(format, this.Columns.ToArray());

            // add each row
            var results = this.Rows.Select(row => string.Format(format, row)).ToList();

            // create the divider
            var divider = Regex.Replace(columnHeaders, @"[^|]", "-");

            builder.AppendLine(columnHeaders);
            builder.AppendLine(divider);
            results.ForEach(row => builder.AppendLine(row));

            return builder.ToString();
        }

        public string ToMinimalString()
        {
            return this.ToMarkDownString(char.MinValue);
        }

        public string ToStringAlternative()
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = this.ColumnLengths();

            // create the string format with padding
            var format = this.Format(columnLengths);

            // find the longest formatted line
            var columnHeaders = string.Format(format, this.Columns.ToArray());

            // add each row
            var results = this.Rows.Select(row => string.Format(format, row)).ToList();

            // create the divider
            var divider = Regex.Replace(columnHeaders, @"[^|]", "-");
            var dividerPlus = divider.Replace("|", "+");

            builder.AppendLine(dividerPlus);
            builder.AppendLine(columnHeaders);

            foreach (var row in results)
            {
                builder.AppendLine(dividerPlus);
                builder.AppendLine(row);
            }

            builder.AppendLine(dividerPlus);

            return builder.ToString();
        }

        private string Format(List<int> columnLengths, char delimiter = '|')
        {
            var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();
            var format = (Enumerable.Range(0, this.Columns.Count)
                                    .Select(i => " " + delimiterStr + " {" + i + ",-" + columnLengths[i] + "}")
                                    .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
            return format;
        }

        private List<int> ColumnLengths()
        {
            var columnLengths = this.Columns
                                    .Select((t, i) => this.Rows.Select(x => x[i])
                                                          .Union(new[] { this.Columns[i] })
                                                          .Where(x => x != null)
                                                          .Select(x => x.ToString().Length).Max())
                                    .ToList();
            return columnLengths;
        }

        public void Write(CellFormat cellFormat = CellFormat.Default)
        {
            switch (cellFormat)
            {
                case CellFormat.Default:
                    Console.WriteLine(this.ToString());
                    break;
                case CellFormat.MarkDown:
                    Console.WriteLine(this.ToMarkDownString());
                    break;
                case CellFormat.Alternative:
                    Console.WriteLine(this.ToStringAlternative());
                    break;
                case CellFormat.Minimal:
                    Console.WriteLine(this.ToMinimalString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cellFormat), cellFormat, null);
            }
        }

        private static IEnumerable<string> GetColumns<T>(string[] columns) => columns?.Any() ?? false
                                                                                  ? columns
                                                                                  : typeof(T).GetProperties().Select(x => x.Name);
    }
}