using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonProcessor /*: IComparisonProcessor */{
        StringBuilder ComparedRowSB = new StringBuilder();
        PerformanceCounter perfCounter = new PerformanceCounter();
        List<string> timings = new List<string>();
        CompareTable CompareTable;

        public void Execute(IWorkTable masterTable, IWorkTable testTable) {
            //gather base stat  
            perfCounter.Start();
            var baseStat = GatherStatistics(masterTable, testTable);
            perfCounter.Stop();
            timings.Add("Gather stat;" + perfCounter.ElapsedTimeMs);
            //analyse
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\baseStat.txt", baseStat.Select(r => r.ToString()));
            var pivotKeysIndexes = AnalyseForPivotKey(baseStat);
            File.AppendAllText(@"C:\Users\MSBZ\Desktop\baseStat.txt", "baseKeyIndex: " + string.Join(";", masterTable.Headers.ColumnIndexIn(pivotKeysIndexes)));
            //get uniq records with base key      
            perfCounter.Start();
            var groupedMaster = Group(masterTable, pivotKeysIndexes);
            var groupedTest = Group(testTable, pivotKeysIndexes);
            perfCounter.Stop();
            timings.Add("GetUniqueRows;" + perfCounter.ElapsedTimeMs);
            //base comparison
            perfCounter.Start();
            var baseCompare = MatchUniqueRows(groupedMaster.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList(), groupedTest.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList(), pivotKeysIndexes);
            CompareTable = new CompareTable(pivotKeysIndexes, masterTable, baseCompare.ToList());
            perfCounter.Stop();
            timings.Add("MatchUniqueRows;" + perfCounter.ElapsedTimeMs);
            //non-unique
            var groupsM = groupedMaster.Where(r => r.Value.Count() > 1);
            var groupsT = groupedTest.Where(r => r.Value.Count() > 1);
            var groups = from m in groupsM join t in groupsT on m.Key equals t.Key select m.Key;
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\groups.txt", string.Join(";", masterTable.Headers.ColumnIndexIn(pivotKeysIndexes)) + Environment.NewLine);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\groups.txt", groups.Select(r=>r.ToString()));
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\nonUStat.txt", "");
            int run = 0;
            List<string> remainM = new List<string>();
            List<string> remainT = new List<string>();
            perfCounter.Start();
            foreach (var key in groups) {
                var m = new WorkTable(groupsM.Where(r => r.Key == key).SelectMany(r => r.Value).ToList(), false);
                var t = new WorkTable(groupsT.Where(r => r.Key == key).SelectMany(r => r.Value).ToList(), false);
                var initCompKeys = new List<int>(pivotKeysIndexes);
                ProcessGroup(m, t, initCompKeys, run);
                run++;
            }
            perfCounter.Stop();
            timings.Add("Proccess;" + perfCounter.ElapsedTimeMs);
            //extra
            List<string> extra = new List<string>();
            var delimiter = masterTable.Headers.Delimiter;
            perfCounter.Start();
            extra.Add("Id" + delimiter + "Version" + delimiter + string.Join(delimiter.ToString(), masterTable.Headers.Columns));
            var masterExtra = GetExtraRows(masterTable, CompareTable.Data.Select(r => r.MasterRowID).Distinct());
            var testExtra = GetExtraRows(testTable, CompareTable.Data.Select(r => r.TestRowID).Distinct());
            extra.AddRange(masterExtra);
            extra.AddRange(testExtra);
            perfCounter.Stop();
            timings.Add("Extra;" + perfCounter.ElapsedTimeMs);
            if (extra.Count > 1) {
                File.WriteAllLines(@"C:\Users\MSBZ\Desktop\extra.txt", extra);
            }
            //save to file
            string comparedRecordsFile = @"C:\Users\MSBZ\Desktop\comparedRecords.txt";
            SaveSummary(masterTable.RowsCount, testTable.RowsCount, CompareTable.Data.Count, extra.Count - 1);
            CompareTable.SaveToFile(comparedRecordsFile, pivotKeysIndexes, masterTable);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);
        }

        private void SaveSummary(int mRowsCount, int tRowsCount, int compRowsCount, int extraRowsCount) {
            var actRowsDiff = mRowsCount >= tRowsCount ? mRowsCount - tRowsCount : tRowsCount - mRowsCount;
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\Summary.txt",
                "Master: " + mRowsCount + ";Test: " + tRowsCount + ";Act diff: " + actRowsDiff + ";Compared: " + compRowsCount + ";Extra: " + extraRowsCount);
        }

        private void ProcessGroup(IWorkTable masterTable, IWorkTable testTable, List<int> compKeysIndexes, double run) {
            var stat = GatherStatistics(masterTable, testTable);           
            var newKeysIndexes = AnalyseInGroup(stat);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\nonUStat.txt", stat.Select(r => run + ";" + r.ToString() + ";" + string.Join("+", masterTable.Headers.ColumnIndexIn(newKeysIndexes))));
            if (newKeysIndexes.Count == 0) {
                masterTable.ApplyRowNumberInGroup(compKeysIndexes);
                testTable.ApplyRowNumberInGroup(compKeysIndexes);
                var newCompare = Match(masterTable.Rows, testTable.Rows, compKeysIndexes);
                CompareTable.Data.AddRange(newCompare);
            } else {
                compKeysIndexes.AddRange(newKeysIndexes);
                var grpMaster = Group(masterTable, compKeysIndexes);
                var grpTest = Group(testTable, compKeysIndexes);
                var uMasterRows = grpMaster.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
                var uTestRows = grpTest.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
                var newCompare = MatchUniqueRows(uMasterRows, uTestRows, compKeysIndexes);
                CompareTable.Data.AddRange(newCompare);
                var mRemainings = grpMaster.Where(r => r.Value.Count() > 1);
                var tRemainings = grpTest.Where(r => r.Value.Count() > 1);
                if(mRemainings.Count() > 0 && tRemainings.Count() > 0) {
                    var groups = from mRem in mRemainings join tRem in tRemainings on mRem.Key equals tRem.Key select mRem.Key;
                    foreach (var item in groups) {
                        var m = new WorkTable(mRemainings.Where(r => r.Key == item).SelectMany(r => r.Value).ToList(), false);
                        var t = new WorkTable(tRemainings.Where(r => r.Key == item).SelectMany(r => r.Value).ToList(), false);
                        var inCompKeys = new List<int>(compKeysIndexes);
                        ProcessGroup(m, t, inCompKeys, run + 0.1);
                    }
                }
            }         
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
                   join t in testRows on new { CompositeKey = m.MaterialiseKey(compKeys), RowNumber = m.RowGroupNumber }
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
            return "[" + string.Join("];[", compHeaders) + "]";
        }

        private int DistinctCount(IEnumerable<string> list) {
            return list.Distinct().Count();
        }

        private Dictionary<int, List<Row>> Group(IWorkTable table, List<int> baseKeyIndex) {
            var query = from rows in table.Rows
                        group rows by rows.MaterialiseKey(baseKeyIndex) into grp
                        //where grp.Count()==1
                        select grp;
            return query.ToDictionary(group => group.Key, group => group.ToList());
        }

        private List<int> AnalyseInGroup(List<ColumnSummary> columnsStat) {
            var uniqKey = columnsStat.FirstOrDefault(col => col.UniqMatchRate == 100);
            var clearedStats = columnsStat.Where(col => col.MatchingRate != 0).ToList();
            if (uniqKey != null) {
                return new List<int>() { uniqKey.Id };
            } else if (clearedStats.Count == 0) {
                return new List<int>();
            } else {
                var maxUniqMatchRate = clearedStats.Max(col => col.UniqMatchRate);
                var maxUniquenessRate = clearedStats.Where(col => col.UniqMatchRate == maxUniqMatchRate).Max(col => col.UniquenessRate);
                var compKeys = clearedStats.Where(col => col.UniqMatchRate == maxUniqMatchRate && col.UniquenessRate == maxUniquenessRate).Select(col => col.Id);
                return compKeys.ToList();
            }
        }

        private List<int> AnalyseForPivotKey(List<ColumnSummary> columnsStat) {
            var uniqKey = columnsStat.FirstOrDefault(col => col.UniqMatchRate == 100);
            if (uniqKey != null) {
                return new List<int>() { uniqKey.Id };
            } else {
                var clearedStats = columnsStat.Where(col => !col.IsDouble);
                var maxMatchingRate = clearedStats.Max(col => col.MatchingRate);
                var compKeys = clearedStats.Where(col => col.MatchingRate == maxMatchingRate).Select(col => col.Id);
                return compKeys.ToList();
            }
        }

        private double CalculatePercentage(int x, int y) {
            return Math.Round(((double)x / y) * 100, 2);
        }

        private bool IsNumeric(IEnumerable<string> column) {
            int n = 0;
            long l = 0;
            return column.Where(item => item != "" && item.ToUpper() != "NULL").Distinct().All(item => int.TryParse(item, out n) || long.TryParse(item, out l));
        }

        private bool IsDouble(IEnumerable<string> column) {
            double d;
            return column.Where(item => item != "" && item.ToUpper() != "NULL").Distinct().All(item => double.TryParse(item, out d));
        }

        private bool HasNulls(IEnumerable<string> column) {
            return column.Any(item => item == "" || item.ToUpper() == "NULL");
        }

        private List<ColumnSummary> GatherStatistics(IWorkTable masterTable, IWorkTable testTable) {
            var totalRows = masterTable.RowsCount > testTable.RowsCount ? testTable.RowsCount : masterTable.RowsCount;
            var columnsStat = new List<ColumnSummary>(masterTable.ColumnsCount);
            IEnumerable<string> uniqColumsValues;
            var masterColumn = new List<string>();
            var testColumn = new List<string>();

            for (int i = 0; i < masterTable.ColumnsCount; i++) {
                // Parallel.For(0, masterTable.ColumnsCount, i => {
                var mTask = Task.Run(() => masterTable.GetDistinctColumnValues(i));
                var tTask = Task.Run(() => testTable.GetDistinctColumnValues(i));
                Task.WaitAll(mTask, tTask);
                masterColumn = mTask.Result;
                testColumn = tTask.Result;
                uniqColumsValues = from m in masterColumn join t in testColumn on m equals t select m;

                var mDistCount = DistinctCount(masterColumn);
                var tDistCount = DistinctCount(testColumn);
                var matchedCount = mDistCount > tDistCount ? tDistCount : mDistCount;
                var uniqCount = uniqColumsValues.Count();

                var uniqMatchRate = CalculatePercentage(uniqCount, totalRows);
                var uniquenessRate = CalculatePercentage(matchedCount, totalRows);
                var matchingRate = CalculateRate(mDistCount, tDistCount, uniqCount);

                var isNumeric = IsNumeric(uniqColumsValues);
                var isDouble = isNumeric ? false : IsDouble(uniqColumsValues);
                var hasNulls = HasNulls(uniqColumsValues);
                if (mDistCount > 2 || tDistCount > 2) {
                    ColumnSummary columnSummary = new ColumnSummary(i, masterTable.Headers.Columns[i], mDistCount, tDistCount, uniqCount, matchingRate, uniquenessRate, uniqMatchRate, isNumeric, isDouble, hasNulls);
                    columnsStat.Add(columnSummary);
                }
                //     });
            }
            return columnsStat;
        }
    }
}
