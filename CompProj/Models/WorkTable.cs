using CompProj.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj {
    public class WorkTable: IWorkTable {
        public List<string> Headers { get; private set; }
        public List<Row> Data { get; private set; }

        public void LoadDataAsync(List<string> data, char delimiter, bool isHeadersExist) {
            SetHeaders(data[0], delimiter, isHeadersExist);
            Data = ParseToTable(data, delimiter);
        }

       private void SetHeaders(string firstLine, char delimiter, bool isHeadersExist) {
            if (isHeadersExist) {
                Headers = firstLine.Split(delimiter).ToList();
            } else {
                int columnsCount = CountColumns(firstLine, delimiter);
                Headers = GenerateDefaultHeaders(columnsCount);
            }
        }

        private int CountColumns(string line, char delimiter) {
            return line.Split(delimiter).ToList().Count;
        }     

        private List<string> GenerateDefaultHeaders(int numColumns) {
            List<string> defaultHeaders = new List<string>();
            for (int i = 0; i < numColumns; i++) {
                defaultHeaders.Add("Column" + i);
            }
            return defaultHeaders;
        }

        private List<Row> ParseToTable(List<string> list, char delimiter) {
            List<Row> table = new List<Row>();
            foreach (var item in list) {
                Row row = new Row(item, delimiter);
                table.Add(row);
            }
            return table;
        }
    }
}
