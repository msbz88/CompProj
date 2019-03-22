using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class CompareTable {
        IWorkTable MasterTable { get; set; }
        IWorkTable TestTable { get; set; }
        List<int> IdColumns { get; set; }
        public List<ComparedRow> Data {get;set;}
        public List<string> Extra { get; set; }

        public CompareTable(IWorkTable masterTable, IWorkTable testTable, List<int> idColumns) {
            Data = new List<ComparedRow>();
            Extra = new List<string>();
            MasterTable = masterTable;
            TestTable = testTable;
            IdColumns = idColumns;
        }

        public void SaveToFile(string filePath) {
            var compareHeaders = new List<string>();
            compareHeaders.Add("Diff");
            compareHeaders.AddRange(MasterTable.Headers.ColumnIndexIn(IdColumns));
            var columnsWithDiff = GetColumnsWithDeviations();
            compareHeaders.AddRange(MasterTable.Headers.ColumnIndexIn(columnsWithDiff));
            var result = new List<string>();
            result.Add(string.Join(MasterTable.Delimiter, compareHeaders));
            result.AddRange(AddIdentificationColumns(columnsWithDiff, MasterTable.Delimiter));
            File.WriteAllLines(filePath, result);
            //Data.Clear();
        }

        private List<int> GetColumnsWithDeviations() {
            var result = new List<int>();
            for (int i = 0; i < MasterTable.ColumnsCount; i++) {
                var isDiff = GetColumn(i).Any(r => r != "0");
                if (isDiff) {
                    result.Add(i);
                }
            }
            return result;
        }

        private IEnumerable<string> AddIdentificationColumns(List<int> columnsWithDiff, string delimiter) {
            return from d in Data
                   join m in MasterTable.Rows on d.MasterRowID equals m.Id
                   let diff = d.Diff
                   let id = m.ColumnIndexIn(IdColumns)
                   let data = d.ColumnIndexIn(columnsWithDiff)
                   select diff + delimiter + string.Join(delimiter, id.Concat(data));
        }

        private IEnumerable<string> GetColumn(int columnPosition) {
            return Data.Select(r => r.Data[columnPosition]);
        }

        private void AddHeadersForExtraData() {
            Extra.Add("Id" + MasterTable.Delimiter + "Version" + MasterTable.Delimiter + string.Join(MasterTable.Delimiter, MasterTable.Headers.Data));
        }

        public void AddExtraData(IEnumerable<string> extraData) {
            if (extraData.Count() > 0) {
                if (Extra.Count == 0) {
                    AddHeadersForExtraData();
                }
                Extra.AddRange(extraData);
            }
        }
    }
}
