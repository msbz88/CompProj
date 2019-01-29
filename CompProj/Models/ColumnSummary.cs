using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ColumnSummary {
        public string ColumnName { get; set; }
        public int MasterUniqueRows { get; set; }
        public int TestUniqueRows { get; set; }
        public int Matched { get; set; }
        public double UniqRate { get; set; }
        public double FullRate { get; set; }

        public ColumnSummary(string columnName, int masterUniqueRows, int testUniqueRows, int matched, double uniqRate, double fullRate) {
            ColumnName = columnName;
            MasterUniqueRows = masterUniqueRows;
            TestUniqueRows = testUniqueRows;
            Matched = matched;
            UniqRate = uniqRate;
            FullRate = fullRate;
        }

        //public override string ToString() {
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("[");
        //    sb.Append(ColumnName);
        //    sb.Append("]");
        //    sb.Append(";");
        //    sb.Append("Master Unq Rows: ");
        //    sb.Append(MasterUniqueRows);
        //    sb.Append(";");
        //    sb.Append("Test Unq Rows: ");
        //    sb.Append(TestUniqueRows);
        //    sb.Append(";");
        //    sb.Append("Matched: ");
        //    sb.Append(Matched);
        //    sb.Append(";");
        //    sb.Append("Unique Rate: ");
        //    sb.Append(UniqRate);
        //    sb.Append(";");
        //    sb.Append("Full Rate: ");
        //    sb.Append(FullRate);
        //    return sb.ToString();
        //}

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(ColumnName);
            sb.Append("]");
            sb.Append(";");
            sb.Append(MasterUniqueRows);
            sb.Append(";");
            sb.Append(TestUniqueRows);
            sb.Append(";");
            sb.Append(Matched);
            sb.Append(";");
            sb.Append(UniqRate);
            sb.Append(";");
            sb.Append(FullRate);
            return sb.ToString();
        }
    }
}
