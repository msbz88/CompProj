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

        private List<string> GetColumn(IWorkTable table, int columnPosition) {
            List<string> column = new List<string>();
            foreach (var row in table.Data) {
                column.Add(row.Columns[columnPosition]);
            }
            return column;
        }

        private List<string> GetDistinctValues(List<string> master, List<string> test) {
            return (from m in master.Distinct()
                    join t in test.Distinct() on m equals t
                    select m).ToList();
        }

        private double CalculateRate(List<string> master, List<string> test, List<string> matchedValues) {
            double finalRate = 0;
            if (master.Distinct().Count() > 2 || test.Distinct().Count() > 2) {
                var currMasterCol = Math.Round(((double)matchedValues.Count / master.Distinct().Count()) * 100, 2);
                var currTestCol = Math.Round(((double)matchedValues.Count / test.Distinct().Count()) * 100, 2);
                finalRate = currMasterCol > currTestCol ? currTestCol : currMasterCol;
            }
            return finalRate;
        }

        private string GatherResults(List<string> masterCol, List<string> testCol, List<string> res) {
            return " Master: " + masterCol.Distinct().Count()
                    + " Test: " + testCol.Distinct().Count()
                    + " Matched: " + res.Count
                    + " M rate: " + Math.Round(((double)res.Count / masterCol.Distinct().Count()) * 100, 2)
                    + " T rate: " + Math.Round(((double)res.Count / testCol.Distinct().Count()) * 100, 2);
        }

        private IEnumerable<string> Match(IWorkTable masterTable, IWorkTable testTable, List<int> compKeys) {
            return from m in masterTable.Data
                   join t in testTable.Data on new {a = String.Join("", m.GetMultipleColumnsByIndex(compKeys)) }
                   equals new { a = String.Join("", t.GetMultipleColumnsByIndex(compKeys)) }
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

        private List<int> Analyse(IWorkTable masterTable, IWorkTable testTable) {
            List<string> masterCol = new List<string>();
            List<string> testCol = new List<string>();
            List<int> compKeys = new List<int>();
            var statToPrint = new List<string>();         
        
            for (int i = 0; i < masterTable.Headers.Columns.Count; i++) {
                masterCol = GetColumn(masterTable, i);
                testCol = GetColumn(testTable, i);
                List<string> res = GetDistinctValues(masterCol, testCol);
                double total = 0;
                total = CalculateRate(masterCol, testCol, res);
                if (total > 75) {
                    compKeys.Add(i);
                }
                statToPrint.Add("[" + masterTable.Headers.Columns[i] + "]" + GatherResults(masterCol, testCol, res));               
                masterCol.Clear();
                testCol.Clear();
                res.Clear();
            }
            statToPrint.Add(GetCompColumnNames(masterTable.Headers, compKeys));     
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\res.txt", statToPrint);
            return compKeys;
        }
    }
}
