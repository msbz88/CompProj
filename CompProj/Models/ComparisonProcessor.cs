using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonProcessor : IComparisonProcessor {
        StringBuilder ComparedRowSB = new StringBuilder();
        PerformanceCounter perfCounter = new PerformanceCounter();
        List<string> timings = new List<string>();

        public List<string> Execute(IWorkTable masterTable, IWorkTable testTable) {
            //gather stats
            perfCounter.Start();
            var columnsStat = GatherStatistics(masterTable, testTable);
            perfCounter.Stop();
            timings.Add("Gather Statistics;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);
            //analyse
            perfCounter.Start();
            List<int> compKeys = Analyse(columnsStat);
            perfCounter.Stop();
            timings.Add("Analyse;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);

            Group(masterTable, testTable, compKeys);

            //compare
            perfCounter.Start();
            List<string> resultToSave = new List<string>();
            var delimiter = masterTable.Headers.Delimiter.ToString();
            var compHeadres = "MasterRowID" + delimiter + "TestRowID" + delimiter + "Diff" + delimiter + string.Join(delimiter, masterTable.Headers.Columns);
            resultToSave.Add(compHeadres);
            var comparisonResult = Match(masterTable, testTable, compKeys);
            foreach (var item in comparisonResult) {
                resultToSave.Add(item.ToString());
            }          
            perfCounter.Stop();
            timings.Add("Match;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);
            //extra 
            perfCounter.Start();
            List<string> extra = new List<string>();
            extra.Add("Id" + delimiter + "Version" + delimiter + string.Join(delimiter, masterTable.Headers.Columns));
            var masterExtra = GetExtraRows(masterTable, comparisonResult.Select(r => r.MasterRowID).Distinct());
            var testExtra = GetExtraRows(testTable, comparisonResult.Select(r => r.TestRowID).Distinct());
            extra.AddRange(masterExtra);
            extra.AddRange(testExtra);
            perfCounter.Stop();
            timings.Add("Extra;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\extra.txt", extra);
            ;
            //save results
            var statToPrint = new List<string>();
            foreach (var item in columnsStat) {
                statToPrint.Add(item.ToString());
            }
            statToPrint.Add(GetCompColumnNames(masterTable.Headers, compKeys));
            statToPrint.Add("Master rows;"+masterTable.RowsCount+";Test rows;"+testTable.RowsCount+";Comparison;" + comparisonResult.Count());

            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\res.txt", statToPrint);
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\resComp.txt", resultToSave);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);
            return resultToSave;
        }

        private ComparedRow Compare(Row masterRow, Row testRow) {
            ComparedRowSB.Clear();         
            int rowDiff = 0;
            for (int i = 0; i < masterRow.Columns.Count; i++) {
                if (masterRow.Columns[i] == testRow.Columns[i]) {
                    ComparedRowSB.Append("0");
                    ComparedRowSB.Append(masterRow.Delimiter);
                } else {
                    ComparedRowSB.Append(masterRow.Columns[i]);
                    ComparedRowSB.Append(" | ");
                    ComparedRowSB.Append(testRow.Columns[i]);
                    ComparedRowSB.Append(masterRow.Delimiter);
                    rowDiff++;
                }
            }
            return new ComparedRow(0, masterRow.Id, testRow.Id, rowDiff, ComparedRowSB.ToString(), masterRow.Delimiter);
        }

        private IEnumerable<string> GetExtraRows(IWorkTable table, IEnumerable<int> comparedId) {
            var filter = table.Data.Select(r => r.Id).Except(comparedId);
            var delimiter = table.Headers.Delimiter.ToString();
            return from r in table.Data
                   join f in filter on r.Id equals f
                   select r.Id + delimiter + table.Name + delimiter + string.Join(delimiter, r.Columns);
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

        private IEnumerable<ComparedRow> Match(IWorkTable masterTable, IWorkTable testTable, List<int> compKeys) {
            return from m in masterTable.Data
                   join t in testTable.Data on new {compositeKey = string.Join("", m.GetMultipleColumnsByIndex(compKeys)) }
                   equals new { compositeKey = string.Join("", t.GetMultipleColumnsByIndex(compKeys)) }
                   select Compare(m, t);
        }

        private string GetCompColumnNames(Row headers, List<int> compKeys) {
            return "[" + string.Join(" ] + [", headers.GetMultipleColumnsByIndex(compKeys)) + "]";
        }

        private int DistinctCount(List<string> list) {
            return list.Distinct().Count();
        }

        private void Group(IWorkTable masterTable, IWorkTable testTable, List<int> compKeys) {
            //var query = masterTable.Data.GroupBy(item => item.GetMultipleColumnsByIndex(compKeys))
            //     .Select(grouping => string.Join(";", grouping.FirstOrDefault().Columns))
            //     .ToList();

            var query = from m in masterTable.Data
                        group m by string.Join(";", m.GetMultipleColumnsByIndex(compKeys)) into g
                        where g.Count() > 1
                        select g.Key + ";" + g.Count();

            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\group.txt", query);
        }
     
        private List<int> Analyse(List<ColumnSummary> columnsStat) {
            var maxMatchingRate = columnsStat.Max(col=>col.MatchingRate);
            //var maxCoverage = columnsStat.Where(col=>col.MatchingRate == maxMatchingRate).Max(col=>col.UniquenessRate);
            return columnsStat.Where(col => col.MatchingRate == maxMatchingRate).GroupBy(col=>col.UniquenessRate).Select(col => col.First().Id).ToList();
        }

        private List<ColumnSummary> GatherStatistics(IWorkTable masterTable, IWorkTable testTable) {
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

                var totalRows = masterTable.RowsCount > testTable.RowsCount ? testTable.RowsCount : masterTable.RowsCount;
                var totalUnqRows = mDistCount > tDistCount ? tDistCount : mDistCount;

                double uRate = 0;
                double fRate = Math.Round(((double)uColumn.Count/ totalRows)*100,2);
                double tRate = Math.Round(((double)totalUnqRows / totalRows)* 100,2);
                uRate = CalculateRate(mDistCount, tDistCount, uColumn.Count);
                bool hasNulls = uColumn.Any(val=>val=="");

                ColumnSummary columnSummary = new ColumnSummary(i, masterTable.Headers.Columns[i], mDistCount, tDistCount, uColumn.Count, uRate, tRate, fRate, hasNulls);
                columnsStat.Add(columnSummary);              
                mColumn.Clear();
                tColumn.Clear();
                uColumn.Clear();
            }
            return columnsStat;
        }
    }
}
