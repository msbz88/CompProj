using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompProj.Models {
    public class WorkTable: IWorkTable {
        public Row Headers { get; private set; }
        public List<Row> Data { get; private set; }

        public void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist) {
            if (isHeadersExist) {
                Headers = new Row(data[0], delimiter);
            } else {
                int columnsCount = CountColumns(data[0], delimiter);
                Headers = GenerateDefaultHeaders(columnsCount, delimiter);
            }         
            Data = ParseToTable(data.Skip(1), delimiter);
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
            return new Row(defaultHeaders.ToString().TrimEnd(delimiter), delimiter);
        }

        private List<Row> ParseToTable(IEnumerable<string> list, char delimiter) {
            List<Row> table = new List<Row>();
            foreach (var item in list) {
                Row row = new Row(item, delimiter);
                table.Add(row);
            }
            return table;
        }
    }
}
