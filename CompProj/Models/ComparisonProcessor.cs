using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonProcessor : IComparisonProcessor {
        StringBuilder ComparedRow = new StringBuilder();
        PerformanceCounter perfCounter = new PerformanceCounter();
        List<string> timings = new List<string>();

        public List<string> Execute(IWorkTable masterTable, IWorkTable testTable) {
            //find comp keys
            perfCounter.Start();
            List<int> compKeys = Analyse(masterTable, testTable);
            perfCounter.Stop();
            timings.Add("Analyse;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);
            //compare
            perfCounter.Start();
            List<string> result = new List<string>();
            result.Add(CreateNewHeaders(masterTable.Headers, compKeys));
            result.AddRange(Match(masterTable, testTable, compKeys));
            perfCounter.Stop();
            timings.Add("Match;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);
            //save results
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\resComp.txt", result);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);
            return result;
        }

        private string Compare(Row row1, Row row2, List<int> compKeys) {
            ComparedRow.Clear();
            var columnsId = row1.GetMultipleColumnsByIndex(compKeys);
            foreach (var item in columnsId) {
                ComparedRow.Append(item);
                ComparedRow.Append(row1.Delimiter);
            }
            for (int i = 0; i < row1.Columns.Count; i++) {
                if (row1.Columns[i] == row2.Columns[i]) {
                    ComparedRow.Append("0");
                    ComparedRow.Append(row1.Delimiter);
                } else {
                    ComparedRow.Append(row1.Columns[i] + " | " + row2.Columns[i]);
                    ComparedRow.Append(row1.Delimiter);
                }
            }
            return ComparedRow.ToString().TrimEnd(row1.Delimiter);
        }

        private List<string> GetDistinctValues(List<string> master, List<string> test) {
            return (from m in master.Distinct()
                    join t in test.Distinct() on m equals t
                    select m).ToList();
        }

        private double CalculateRate(int uniqueRowsMaster, int uniqueRowsTest, int matchedValues) {
            double finalRate = 0;
            if (uniqueRowsMaster > 2 || uniqueRowsTest > 2) {
                var lowerNumber = uniqueRowsMaster > uniqueRowsTest ? uniqueRowsTest : uniqueRowsMaster;
                finalRate = ((double)matchedValues / lowerNumber) * 100;             
            }
            return Math.Round(finalRate, 2);
        }

        private IEnumerable<string> Match(IWorkTable masterTable, IWorkTable testTable, List<int> compKeys) {
            return from m in masterTable.Data
                   join t in testTable.Data on new {a = string.Join("", m.GetMultipleColumnsByIndex(compKeys)) }
                   equals new { a = string.Join("", t.GetMultipleColumnsByIndex(compKeys)) }
                   select Compare(m, t, compKeys);
        }

        private string CreateNewHeaders(Row headers, List<int> compKeys) {
            StringBuilder resultHeaders = new StringBuilder();
            foreach (var item in headers.GetMultipleColumnsByIndex(compKeys)) {
                resultHeaders.Append(item);
                resultHeaders.Append(headers.Delimiter);
            }
            foreach (var item in headers.Columns) {
                resultHeaders.Append(item);
                resultHeaders.Append(headers.Delimiter);
            }
            return resultHeaders.ToString();
        }

        private string GetCompColumnNames(Row headers, List<int> compKeys) {
            return "[" + string.Join(" ] + [", headers.GetMultipleColumnsByIndex(compKeys)) + "]";
        }

        private int DistinctCount(List<string> list) {
            return list.Distinct().Count();
        }

        private List<int> Analyse(IWorkTable masterTable, IWorkTable testTable) {
            List<int> compKeys = new List<int>();
            List<string> mColumn = new List<string>();
            List<string> tColumn = new List<string>();
            List<string> uColumn = new List<string>();            
            List<ColumnSummary> columnsStat = new List<ColumnSummary>();   
        
            for (int i = 0; i < masterTable.ColumnsCount; i++) {
                mColumn = masterTable.GetColumn(i);
                tColumn = testTable.GetColumn(i);
                uColumn = GetDistinctValues(mColumn, tColumn);

                var mDistCount = DistinctCount(mColumn);
                var tDistCount = DistinctCount(tColumn);
                double uRate = 0;
                double fRate = (double)uColumn.Count/(masterTable.RowsCount>testTable.RowsCount? testTable.RowsCount: masterTable.RowsCount);
                uRate = CalculateRate(mDistCount, tDistCount, uColumn.Count);
                if (uRate > 75) {
                    compKeys.Add(i);
                }
                ColumnSummary columnSummary = new ColumnSummary(masterTable.Headers.Columns[i], mDistCount, tDistCount, uColumn.Count, uRate, Math.Round(fRate, 2)*100);
                columnsStat.Add(columnSummary);              
                mColumn.Clear();
                tColumn.Clear();
                uColumn.Clear();
            }
            var statToPrint = new List<string>();
            foreach (var item in columnsStat) {
                statToPrint.Add(item.ToString());
            }
            statToPrint.Add(GetCompColumnNames(masterTable.Headers, compKeys));
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\res.txt", statToPrint);
            return compKeys;
        }
    }
}
