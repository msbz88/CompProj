using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparedRow : Row {
        public int MasterRowID { get; set; }
        public int TestRowID { get; set; }
        public int Diff { get; set; }
        public string IdColumns { get; set; }

        public ComparedRow(int id, int masterRowID, int testRowID, int diff, string idColumns, string[] data) : base(id, data) {
            MasterRowID = masterRowID;
            TestRowID = testRowID;
            Diff = diff;
            IdColumns = idColumns;
        }

        //public override string ToString() {
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(MasterRowID);
        //    sb.Append(Delimiter);
        //    sb.Append(TestRowID);
        //    sb.Append(Delimiter);
        //    sb.Append(Diff);
        //    sb.Append(Delimiter);
        //    foreach (var item in Data) {
        //        sb.Append(item);
        //        sb.Append(Delimiter);
        //    }          
        //    return sb.ToString().TrimEnd(Delimiter);
        //}
    }
}
