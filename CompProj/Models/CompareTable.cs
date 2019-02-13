using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class CompareTable {
        public List<int> IdHeaders { get; set; }
        public IWorkTable BaseTable { get; set; }
        public List<ComparedRow> Data {get;set;}

        public CompareTable() {
        }

        public void SaveToFile(string filePath, List<int> idHeaders, IWorkTable baseTable) {
            List<string> result = new List<string>();
            var delimiter = BaseTable.Headers.Delimiter.ToString();
            var compareHeaders = new List<string>();
            compareHeaders.Add("Diff");
            compareHeaders.AddRange(BaseTable.Headers.ColumnIndexIn(IdHeaders));          
            compareHeaders.AddRange(BaseTable.Headers.Columns);
            result.Add(string.Join(delimiter, compareHeaders));
            result.AddRange(AddIdColumsToData());
            File.WriteAllLines(filePath, result);
            Data.Clear();
        }

        private List<string> AddIdColumsToData() {
            var delimiter = BaseTable.Headers.Delimiter.ToString();
            var query = from compRows in Data
                        join rows in BaseTable.Rows on compRows.MasterRowID equals rows.Id
                        select new { RowID = rows.ColumnIndexIn(IdHeaders), ComparedRows = compRows};
            List <string> newRow = new List<string>();
            List<string> result = new List<string>();
            foreach (var item in query) {
                newRow.Add(item.ComparedRows.Diff.ToString());
                newRow.AddRange(item.RowID);
                newRow.AddRange(item.ComparedRows.Columns);
                result.Add(string.Join(delimiter, newRow));
                newRow.Clear();
            }
            return result;
        }

        private void ExcludeColumnsWithoutDeviations() {

        }
    }
}
