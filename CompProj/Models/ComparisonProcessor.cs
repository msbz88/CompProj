using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    class ComparisonProcessor : IComparisonProcessor {
        public void Compare(IWorkTable masterTable, IWorkTable testTable) {
            //var query = from m in tableMaster
            //            join t in tableTest on m.Cells[GetValue("SECURITY_ID")] equals t.Cells[GetValue("SECURITY_ID")]
            //            select m.Compare(t);

            //foreach (var item in query) {
            //    AppendText(item);
            //}
        }

        //private int GetValue(string name) {
        //    return Headers.FindIndex(item => item == name);
        //}

        public string Compare(Row row) {
            StringBuilder comparedRow = new StringBuilder();
            for (int i = 0; i < row.Cells.Count; i++) {
                if (row.Cells[i] == row.Cells[i]) {
                    comparedRow.Append("0");
                    comparedRow.Append(row.Delimiter);
                } else {
                    comparedRow.Append(row.Cells[i] + " | " + row.Cells[i]);
                    comparedRow.Append(row.Delimiter);
                }
            }
            return comparedRow.ToString();
        }
    }
}
