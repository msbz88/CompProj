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
        CompareTable CompareTable = new CompareTable();

        public List<string> Execute(IWorkTable masterTable, IWorkTable testTable) {
            //    //gather stats
            //    perfCounter.Start();
            //    var allColumnsStat = GatherStatistics(masterTable, testTable);
            //    perfCounter.Stop();
            //    timings.Add("Gather Statistics;" + perfCounter.ElapsedTimeMs);
            //    //analyse
            //    perfCounter.Start();
            //    var compKeyBase = Analyse(allColumnsStat);
            //    perfCounter.Stop();
            //    timings.Add("Analyse;" + perfCounter.ElapsedTimeMs);
            //    //composite key
            //    perfCounter.Start();
            //    var compKeysComposite = FindCompositeKey(masterTable, testTable, compKeyBase, allColumnsStat);
            //    perfCounter.Stop();
            //    timings.Add("FindCompositeKey;" + perfCounter.ElapsedTimeMs);
            //    //compare
            //    perfCounter.Start();
            //    List<string> resultToSave = new List<string>();
            //    var delimiter = masterTable.Headers.Delimiter.ToString();
            //    var compHeadres = "MasterRowID" + delimiter + "TestRowID" + delimiter + "Diff" + delimiter + string.Join(delimiter, masterTable.Headers.Columns);
            //    resultToSave.Add(compHeadres);
            //    var comparisonResult = Match(masterTable, testTable, compKeysComposite);
            //    foreach (var item in comparisonResult) {
            //        resultToSave.Add(item.ToString());
            //    }
            //    perfCounter.Stop();
            //    timings.Add("Match;" + perfCounter.ElapsedTimeMs);
            //    //extra
            //    perfCounter.Start();
            //    List<string> extra = new List<string>();
            //    extra.Add("Id" + delimiter + "Version" + delimiter + string.Join(delimiter, masterTable.Headers.Columns));
            //    var masterExtra = GetExtraRows(masterTable, comparisonResult.Select(r => r.MasterRowID).Distinct());
            //    var testExtra = GetExtraRows(testTable, comparisonResult.Select(r => r.TestRowID).Distinct());
            //    extra.AddRange(masterExtra);
            //    extra.AddRange(testExtra);
            //    perfCounter.Stop();
            //    timings.Add("Extra;" + perfCounter.ElapsedTimeMs);
            //    if (extra.Count > 1) {
            //        File.WriteAllLines(@"C:\Users\MSBZ\Desktop\extra.txt", extra);
            //    }          
            //    ;
            //    //save results
            //    var statToPrint = new List<string>();
            //    foreach (var item in allColumnsStat) {
            //        statToPrint.Add(item.ToString());
            //    }
            //    statToPrint.Add(GetCompColumnNames(masterTable.Headers, compKeysComposite));
            //    statToPrint.Add("Master rows;"+masterTable.RowsCount+";Test rows;"+testTable.RowsCount+";Comparison;" + comparisonResult.Count());

            //    File.WriteAllLines(@"C:\Users\MSBZ\Desktop\res.txt", statToPrint);
            //    File.WriteAllLines(@"C:\Users\MSBZ\Desktop\resComp.txt", resultToSave);
            //    File.AppendAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);
            Go(masterTable, testTable);
            return null;
        }

        private void Go(IWorkTable masterTable, IWorkTable testTable) {
            //gather base stat        
            var baseStat = GatherStatistics(masterTable, testTable);
            //analyse
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\baseStat.txt", baseStat.Select(r=>r.ToString()));        
            var compKeysIndexes = Analyse(baseStat);
            File.AppendAllText(@"C:\Users\MSBZ\Desktop\baseStat.txt", "baseKeyIndex: " + string.Join(";", masterTable.Headers.ColumnIndexIn(compKeysIndexes)));
            //get uniq records with base key      
            perfCounter.Start();
            var groupedMaster = Group(masterTable, compKeysIndexes);
            var groupedTest = Group(testTable, compKeysIndexes);
            perfCounter.Stop();
            timings.Add("GetUniqueRows;" + perfCounter.ElapsedTimeMs);
            //base comparison
            perfCounter.Start();
            var baseCompare = MatchUniqueRows(groupedMaster.Where(r => r.Value.Count()==1).SelectMany(r => r.Value).ToList(), groupedTest.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList(), compKeysIndexes);
            CompareTable.Data.AddRange(baseCompare.Select(r => r).ToList());
            perfCounter.Stop();
            timings.Add("MatchUniqueRows;" + perfCounter.ElapsedTimeMs);
            //non-unique
            var groupsM = groupedMaster.Where(r => r.Value.Count() > 1);
            var groupsT = groupedTest.Where(r => r.Value.Count() > 1);
            var groups = from m in groupsM join t in groupsT on m.Key equals t.Key select m.Key;
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\groups.txt", string.Join(";", masterTable.Headers.ColumnIndexIn(compKeysIndexes))+Environment.NewLine);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\groups.txt", groups);
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\nonUStat.txt", "");
            int run = 0;
            List<string> remainM = new List<string>();
            List<string> remainT = new List<string>();
            foreach (var key in groups) {
                var m = new WorkTable(groupsM.Where(r => r.Key == key).SelectMany(r => r.Value).ToList(), false);
                var t = new WorkTable(groupsT.Where(r => r.Key == key).SelectMany(r => r.Value).ToList(), false);
                ProcessGroup(m, t, compKeysIndexes, run);             
                run++;
            }
            //extra
            List<string> extra = new List<string>();
            var delimiter = masterTable.Headers.Delimiter;
            extra.Add("Id" + delimiter + "Version" + delimiter + string.Join(delimiter.ToString(), masterTable.Headers.Columns));
            var masterExtra = GetExtraRows(masterTable, CompareTable.Data.Select(r => r.MasterRowID).Distinct());
            var testExtra = GetExtraRows(testTable, CompareTable.Data.Select(r => r.TestRowID).Distinct());
            extra.AddRange(masterExtra);
            extra.AddRange(testExtra);
            if (extra.Count > 1) {
                File.WriteAllLines(@"C:\Users\MSBZ\Desktop\extra.txt", extra);
            }
            //save to file
            string comparedRecordsFile = @"C:\Users\MSBZ\Desktop\comparedRecords.txt";
            CompareTable.SaveToFile(comparedRecordsFile, compKeysIndexes, masterTable);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);
        }

        private void ProcessGroup(IWorkTable masterTable, IWorkTable testTable, List<int> compKeysIndexes, int run) {
            //while () {
                var stat = GatherStatistics(masterTable, testTable);
                var newKeysIndexes = AnalyseInGroup(stat);
                if (newKeysIndexes.Count == 0) {
                    masterTable.ApplyRowNumberInGroup(compKeysIndexes);
                    testTable.ApplyRowNumberInGroup(compKeysIndexes);
                    var newCompare = Match(masterTable.Rows, testTable.Rows, compKeysIndexes);
                    CompareTable.Data.AddRange(newCompare);
                } else {
                    var grpMaster = Group(masterTable, newKeysIndexes);
                    var grpTest = Group(testTable, newKeysIndexes);
                    var uMasterRows = grpMaster.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
                    var uTestRows = grpTest.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
                    var newCompare = MatchUniqueRows(uMasterRows, uTestRows, newKeysIndexes);
                    CompareTable.Data.AddRange(newCompare);
                }         
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\nonUStat.txt", stat.Select(r => run + ";" + r.ToString() + ";" + string.Join("+", masterTable.Headers.ColumnIndexIn(newKeysIndexes))));
          //  }
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
            var filter = table.Rows.Select(r => r.Id).Except(comparedId);
            var delimiter = table.Headers.Delimiter.ToString();
            return from r in table.Rows
                   join f in filter on r.Id equals f
                   select r.Id + delimiter + table.Name + delimiter + string.Join(delimiter, r.Columns);
        }

        private List<string> GetDistinctValues(IEnumerable<string> master, IEnumerable<string> test) {
            return (from m in master.Distinct()
                    join t in test.Distinct() on m equals t
                    select m).ToList();
        }

        private double CalculateRate(int uniqueRowsMaster, int uniqueRowsTest, int matchedValues) {
            double finalRate = 0;
            var lowerNumber = uniqueRowsMaster > uniqueRowsTest ? uniqueRowsMaster : uniqueRowsTest;
            finalRate = ((double)matchedValues / lowerNumber) * 100;             
            return Math.Round(finalRate, 2);
        }

        private IEnumerable<ComparedRow> Match(IEnumerable<Row> masterRows, IEnumerable<Row> testRows, List<int> compKeys) {
            return from m in masterRows
                   join t in testRows on new {CompositeKey = m.MaterialiseKey(compKeys), RowNumber = m.RowGroupNumber }
                   equals new { CompositeKey = t.MaterialiseKey(compKeys), RowNumber = t.RowGroupNumber }    
                   select Compare(m, t);
        }

        private IEnumerable<ComparedRow> MatchUniqueRows(IEnumerable<Row> masterRows, IEnumerable<Row> testRows, List<int> uniqKey) {
            return from m in masterRows
                   join t in testRows on m.MaterialiseKey(uniqKey) equals t.MaterialiseKey(uniqKey)
                   select Compare(m, t);
        }

        private string GetCompColumnNames(Row headers, List<int> compKeys) {
            var compHeaders = new List<string>();
            foreach (var item in compKeys) {
                compHeaders.Add(headers.Columns[item]);
            }
            return "["+string.Join("];[", compHeaders)+"]";
        }

        private int DistinctCount(IEnumerable<string> list) {
            return list.Distinct().Count();
        }

        private Dictionary<string, List<Row>> Group(IWorkTable table, List<int> baseKeyIndex) {
            var query = from rows in table.Rows
                        group rows by rows.MaterialiseKey(baseKeyIndex) into grp
                       //where grp.Count()==1
                        select grp;
            return query.ToDictionary(group=>group.Key, group=>group.ToList());
        }

        private Dictionary<string, List<Row>> Group(IWorkTable masterTable, IWorkTable testTable, List<int> compKeys) {
            var table = from m in masterTable.Rows join t in testTable.Rows on m.MaterialiseKey(compKeys) equals t.MaterialiseKey(compKeys) select m;
            var query = from m in table
                        group m by m.MaterialiseKey(compKeys) into g
                        where g.Count() > 1
                        select g;
            Dictionary<string, List<Row>> result = query.ToDictionary(group => group.Key, group => group.ToList());
            return result;
        }

        private List<int> AnalyseInGroup(List<ColumnSummary> columnsStat) {
            var uniqKey = columnsStat.FirstOrDefault(col => col.UniqMatchRate == 100);
            var clearedStats = columnsStat.Where(col => col.MatchingRate != 0).ToList();
            if (uniqKey != null) {
                return new List<int>() { uniqKey.Id };
            } else if (clearedStats.Count == 0) {
                return new List<int>();
            } else {              
                var maxMatchingRate = clearedStats.Max(col => col.MatchingRate);
                var maxUniquenessRate = clearedStats.Where(col => col.MatchingRate == maxMatchingRate).Max(col => col.UniquenessRate);
                var compKeys = clearedStats.Where(col => col.MatchingRate == maxMatchingRate && col.UniquenessRate == maxUniquenessRate).Select(col => col.Id);
                return compKeys.ToList();
            }
        }

        private List<int> Analyse(List<ColumnSummary> columnsStat) {
            var uniqKey = columnsStat.FirstOrDefault(col => col.UniqMatchRate == 100);
            if (uniqKey != null) {
                return new List<int>() { uniqKey.Id };
            } else {
                var clearedStats = columnsStat.Where(col => !col.IsDouble && !col.IsHasNulls);
                var maxMatchingRate = clearedStats.Max(col => col.MatchingRate);
                var compKeys = clearedStats.Where(col => col.MatchingRate == maxMatchingRate).Select(col => col.Id);
                return compKeys.ToList();
            }
        }

        private double CalculatePercentage(int x, int y) {
            return Math.Round(((double)x / y) * 100, 2);
        }

        private List<ColumnSummary> GatherStatistics(IWorkTable masterTable, IWorkTable testTable) {
            var totalRows = masterTable.RowsCount > testTable.RowsCount ? testTable.RowsCount : masterTable.RowsCount;
            List<ColumnSummary> columnsStat = new List<ColumnSummary>();
            int n = 0;
            long l = 0;
            var masterColumValues = new List<string>();
            var testColumnValues = new List<string>();
            var uniqColumsValues = new List<string>();

            for (int i = 0; i < masterTable.ColumnsCount; i++) {
                masterColumValues = masterTable.GetColumn(i);
                testColumnValues = testTable.GetColumn(i);
                uniqColumsValues = GetDistinctValues(masterColumValues, testColumnValues);

                var mDistCount = DistinctCount(masterColumValues);
                var tDistCount = DistinctCount(testColumnValues);
                var matchedCount = mDistCount > tDistCount ? tDistCount : mDistCount;

                double uniqMatchRate = CalculatePercentage(uniqColumsValues.Count, totalRows);
                double uniquenessRate = CalculatePercentage(matchedCount, totalRows);
                double matchingRate = CalculateRate(mDistCount, tDistCount, uniqColumsValues.Count);

                
                bool isNumeric = uniqColumsValues.Where(item => item != "" && item.ToUpper() != "NULL").Distinct().All(item => int.TryParse(item, out n) || long.TryParse(item, out l));
                double d;
                bool isDouble = isNumeric ? false : uniqColumsValues.Where(item => item != "" && item.ToUpper() != "NULL").Distinct().All(item => double.TryParse(item, out d));
                bool isHasNulls = uniqColumsValues.Any(item => item == "" || item.ToUpper() == "NULL");
                if (mDistCount > 1 || tDistCount > 1) {
                    ColumnSummary columnSummary = new ColumnSummary(i, masterTable.Headers.Columns[i], mDistCount, tDistCount, uniqColumsValues.Count, matchingRate, uniquenessRate, uniqMatchRate, isNumeric, isDouble, isHasNulls);
                    columnsStat.Add(columnSummary);
                }
            }
            return columnsStat;
        }
    }
}
