using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class RowMap: IEquatable<RowMap> {
        public int MasterRowID { get; set; }
        public int TestRowID { get; set; }
        public List<ColumnSummary> DiffColumns { get; set; }

        public RowMap(int masterRowID, int testRowID, List<ColumnSummary> diffColumns) {
            MasterRowID = masterRowID;
            TestRowID = testRowID;
            DiffColumns = diffColumns;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(MasterRowID);
            sb.Append(";");
            sb.Append(TestRowID);
            sb.Append(";");
            foreach (var item in DiffColumns) {
                sb.Append(item.UniqMatchRate);
                sb.Append(";");
            }
            return sb.ToString().TrimEnd(';');
        }

        public bool Equals(RowMap other) {
            if(this.MasterRowID==other.MasterRowID || this.TestRowID == other.TestRowID) {
                return true;
            } else {
                return false;
            }
        }
    }
}
