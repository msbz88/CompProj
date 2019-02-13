using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ColumnSummary {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MasterUniqueRows { get; set; }
        public int TestUniqueRows { get; set; }
        public int Matched { get; set; }
        public double MatchingRate { get; set; }
        public double UniquenessRate { get; set; }
        public double UniqMatchRate { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsDouble { get; set; }
        public bool IsHasNulls { get; set; }

        public ColumnSummary(int id, string name, int masterUniqueRows, int testUniqueRows, int matched, double matchingRate, double uniquenessRate, double uniqMatchRate, bool isNumeric, bool isDouble, bool isHasNulls) {
            Id = id;
            Name = name;
            MasterUniqueRows = masterUniqueRows;
            TestUniqueRows = testUniqueRows;
            Matched = matched;
            MatchingRate = matchingRate;
            UniquenessRate = uniquenessRate;
            UniqMatchRate = uniqMatchRate;
            IsNumeric = isNumeric;
            IsDouble = isDouble;
            IsHasNulls = isHasNulls;
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
            sb.Append(Name);
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
            sb.Append(UniqMatchRate);
            sb.Append(";");
            sb.Append(IsNumeric);
            sb.Append(";");
            sb.Append(IsDouble);
            sb.Append(";");
            sb.Append(IsHasNulls);
            return sb.ToString();
        }
    }
}
