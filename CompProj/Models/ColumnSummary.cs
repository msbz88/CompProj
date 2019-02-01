using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ColumnSummary {
        public int Id { get; set; }
        public string ColumnName { get; set; }
        public int MasterUniqueRows { get; set; }
        public int TestUniqueRows { get; set; }
        public int Matched { get; set; }
        public double MatchingRate { get; set; }
        public double UniquenessRate { get; set; }
        public double FullRate { get; set; }
        public bool HasNulls { get; set; }

        public ColumnSummary(int id, string columnName, int masterUniqueRows, int testUniqueRows, int matched, double matchingRate, double uniquenessRate, double fullRate, bool hasNulls) {
            Id = id;
            ColumnName = columnName;
            MasterUniqueRows = masterUniqueRows;
            TestUniqueRows = testUniqueRows;
            Matched = matched;
            MatchingRate = matchingRate;
            UniquenessRate = uniquenessRate;
            FullRate = fullRate;
            HasNulls = hasNulls;
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
            sb.Append(MatchingRate);
            sb.Append(";");
            sb.Append(UniquenessRate);
            sb.Append(";");
            sb.Append(FullRate);
            sb.Append(";");
            sb.Append(HasNulls);
            return sb.ToString();
        }
    }
}
