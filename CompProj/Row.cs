using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj {
    public class Row {
        public List<string> Cells { get; set; }
        private char Delimiter { get; set; }

        public Row(string data, char delimiter) {
            Delimiter = delimiter;
            Cells = data.Split(Delimiter).ToList();
        }

        public string Compare(Row row) {
            StringBuilder comparedRow = new StringBuilder();
            for (int i = 0; i < Cells.Count; i++) {
                if (Cells[i] == row.Cells[i]) {
                    comparedRow.Append("0");
                    comparedRow.Append(Delimiter);
                } else {
                    comparedRow.Append(Cells[i] + " | " + row.Cells[i]);
                    comparedRow.Append(Delimiter);
                }
            }
            return comparedRow.ToString();
        }




    }
}
