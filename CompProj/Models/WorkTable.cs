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
        public List<Row> Rows { get; private set; }
        public int RowsCount { get; private set; }
        public int ColumnsCount { get; private set; }

        public WorkTable(string name) {
            Name = name;
        }

        public WorkTable(List<Row> data, bool isHeadersExist) {
            Rows = data;
            RowsCount = Rows.Count;
            ColumnsCount = data[0].Columns.Count;
            if (isHeadersExist) {
                Headers = data[0];
            } else {
                Headers = GenerateDefaultHeaders(ColumnsCount, data[0].Delimiter);
            }
        }

        public void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist) {
            ColumnsCount = CountColumns(data[0], delimiter);
            if (isHeadersExist) {
                Headers = new Row(0, data[0], delimiter);
            } else {
                Headers = GenerateDefaultHeaders(ColumnsCount, delimiter);
            }         
            Rows = ParseToTable(data.Skip(1), delimiter);
            RowsCount = Rows.Count;
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
            return Rows.Select(r => r.Columns[columnPosition]).ToList();
        }

        public List<string> GetDistinctColumnValues(int columnPosition) {
            return Rows.Select(r => r.Columns[columnPosition]).Distinct().ToList();
        }

        public void SaveToFile(string filePath) {
            List<string> result = new List<string>();
            var delimiter = Headers.Delimiter.ToString();
            var headres = "RowNum"+delimiter+"Id" + delimiter + string.Join(delimiter, Headers.Columns);
            result.Add(headres);
            foreach (var item in Rows) {
                result.Add(item.ToString());
            }
            File.WriteAllLines(filePath, result);
        }

        public void ApplyRowNumberInGroup(List<int> compKeys) {          
            var query = from r in Rows
                        group r by r.MaterialiseKey(compKeys)
                        into g
                        where g.Count() > 1
                        //orderby g.Select(r=>r.MaterialiseKey(orderBy))                       
                        select g;
            foreach (var group in query) {
                int RowNumber = 0;
                foreach (var row in group) {
                    row.RowGroupNumber = RowNumber++;
                }
            }
        }
    }
}
