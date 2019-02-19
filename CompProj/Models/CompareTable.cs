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

        public CompareTable(List<int> idHeaders, IWorkTable baseTable, List<ComparedRow> data) {
            IdHeaders = idHeaders;
            BaseTable = baseTable;
            Data = data;
        }

        public void SaveToFile(string filePath, List<int> idHeaders, IWorkTable baseTable) {
            List<string> result = new List<string>();
            var delimiter = BaseTable.Headers.Delimiter.ToString();
            var compareHeaders = new List<string>();
            compareHeaders.Add("Diff");
            compareHeaders.AddRange(BaseTable.Headers.ColumnIndexIn(IdHeaders));
            var colWithDiff = GetColumnsWithDeviations();
            compareHeaders.AddRange(BaseTable.Headers.ColumnIndexIn(colWithDiff));
            result.Add(string.Join(delimiter, compareHeaders));
            result.AddRange(AddIdColumsToData(colWithDiff));
            File.WriteAllLines(filePath, result);
            Data.Clear();
        }

        private List<string> AddIdColumsToData(List<int> colsWithDiff) {
            var delimiter = BaseTable.Headers.Delimiter.ToString();
            var query = from compRows in Data
                        join rows in BaseTable.Rows on compRows.MasterRowID equals rows.Id
                        select new { RowID = rows.ColumnIndexIn(IdHeaders), Diff = compRows.Diff, ComparedRows = compRows.ColumnIndexIn(colsWithDiff) };
            List <string> newRow = new List<string>();
            List<string> result = new List<string>();
            foreach (var item in query) {
                newRow.Add(item.Diff.ToString());
                newRow.AddRange(item.RowID);
                newRow.AddRange(item.ComparedRows);
                result.Add(string.Join(delimiter, newRow));
                newRow.Clear();
            }
            return result;
        }

        private List<int> GetColumnsWithDeviations() {
            List<int> res = new List<int>(0);
            for (int i = 0; i < BaseTable.ColumnsCount; i++) {
                var isDiff = GetColumn(i).Any(r => r != "0");
                if (isDiff) {
                    res.Add(i);
                }               
            }
            return res;
        }

        private List<string> GetColumn(int columnPosition) {
            return Data.Select(r => r.Columns[columnPosition]).ToList();
        }
    }
}
