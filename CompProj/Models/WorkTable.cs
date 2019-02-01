using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompProj.Models {
    public class WorkTable: IWorkTable {
        public string Name { get; set; }
        public Row Headers { get; private set; }
        public List<Row> Data { get; private set; }
        public int RowsCount { get; private set; }
        public int ColumnsCount { get; private set; }

        public WorkTable(string name) {
            Name = name;
        }

        public void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist) {
            ColumnsCount = CountColumns(data[0], delimiter);
            if (isHeadersExist) {
                Headers = new Row(0, data[0], delimiter);
            } else {
                Headers = GenerateDefaultHeaders(ColumnsCount, delimiter);
            }         
            Data = ParseToTable(data.Skip(1), delimiter);
            RowsCount = Data.Count;
        }

        private int CountColumns(string line, char delimiter) {
            return line.Split(delimiter).ToList().Count;
        }     

        private Row GenerateDefaultHeaders(int numColumns, char delimiter) {
            StringBuilder defaultHeaders = new StringBuilder();
            for (int i = 0; i < numColumns; i++) {
                defaultHeaders.Append("Column");
                defaultHeaders.Append(i);
                defaultHeaders.Append(delimiter);
            }
            return new Row(0, defaultHeaders.ToString().TrimEnd(delimiter), delimiter);
        }

        private List<Row> ParseToTable(IEnumerable<string> list, char delimiter) {
            int rowId = 1;
            return list.Select(line => new Row(rowId++, line, delimiter)).ToList();
        }

        public List<string> GetColumn(int columnPosition) {
            List<string> column = new List<string>();
            foreach (var row in Data) {
                column.Add(row.Columns[columnPosition]);
            }
            return column;
        }

        public void SaveToFile(string filePath) {
            List<string> result = new List<string>();
            var delimiter = Headers.Delimiter.ToString();
            var headres = "Id" + delimiter + string.Join(delimiter, Headers.Columns);
            result.Add(headres);
            foreach (var item in Data) {
                result.Add(item.ToString());
            }
            File.WriteAllLines(filePath, result);
        }
    }
}
