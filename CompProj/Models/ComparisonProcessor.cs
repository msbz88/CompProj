using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonProcessor /*: IComparisonProcessor */{
        List<string> ComparedRowSB = new List<string>();
        PerformanceCounter PerfCounter;
        CompareTable CompareTable;
        IWorkTable MasterTable;
        IWorkTable TestTable;
        string Delimiter;
        List<ColumnSummary> BaseStat;
        List<int> PivotKeysIndexes { get; set; }
        public ComparisonProcessor(PerformanceCounter perfCounter) {
            PerfCounter = perfCounter;
        }

        public async Task Execute(IWorkTable masterTable, IWorkTable testTable) {
            MasterTable = masterTable;
            TestTable = testTable;
            Delimiter = masterTable.Delimiter;
            //gather base stat  
            PerfCounter.Start();
            BaseStat = GatherStatistics(masterTable.Rows, testTable.Rows);
            PerfCounter.Stop("Base Gather Stat");
            //analyse
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\baseStat.txt", BaseStat.Select(r => r.ToString()));
            PerfCounter.Start();
            PivotKeysIndexes = await Task.Run(() => AnalyseForPivotKey(masterTable.Rows, BaseStat));
            PerfCounter.Stop("AnalyseForPivotKey");
            File.AppendAllText(@"C:\Users\MSBZ\Desktop\baseStat.txt", "baseKeyIndex: " + string.Join(";", masterTable.Headers.ColumnIndexIn(PivotKeysIndexes)));
            //non-unique
            PerfCounter.Start();
            var groupsM = Group(masterTable.Rows, PivotKeysIndexes);
            var groupsT = Group(testTable.Rows, PivotKeysIndexes);
            PerfCounter.Stop("Base Group");
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\groups.txt", string.Join(";", masterTable.Headers.ColumnIndexIn(PivotKeysIndexes)) + Environment.NewLine);
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\nonUStat.txt", "");
            PerfCounter.Start();
            var compareHeaders = masterTable.Headers.ColumnIndexIn(PivotKeysIndexes).ToList();
            compareHeaders.AddRange(masterTable.Headers.Data);
            CompareTable = new CompareTable(masterTable, testTable, PivotKeysIndexes);
            var uMasterRows = groupsM.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
            var uTestRows = groupsT.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
            CompareTable.Data.AddRange(Match(uMasterRows, uTestRows, PivotKeysIndexes));
            PerfCounter.Stop("Preparison");

            PerfCounter.Start();
            var mRemainings = groupsM.Where(r => r.Value.Count() > 1).ToList();
            var tRemainings = groupsT.Where(r => r.Value.Count() > 1).ToList();
            int run = 0;
            var groups = from m in mRemainings
                         join t in tRemainings on m.Key equals t.Key
                         select ProcessGroup(m.Value, t.Value, ref run);
            var res = groups.SelectMany(r => r);
            if (res.Any()) {
                CompareTable.Data.AddRange(res);
            }
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\run.txt", run.ToString());
            PerfCounter.Stop("Process");
            //extra
            PerfCounter.Start();
            var masterExtra = GetExtraRows(masterTable, CompareTable.Data.Select(r => r.MasterRowID).Distinct());
            var testExtra = GetExtraRows(testTable, CompareTable.Data.Select(r => r.TestRowID).Distinct());
            CompareTable.AddExtraData(masterExtra);
            CompareTable.AddExtraData(testExtra);
            PerfCounter.Stop("Extra");
            if (CompareTable.Extra.Count > 0) {
                File.WriteAllLines(@"C:\Users\MSBZ\Desktop\extra.txt", CompareTable.Extra);
            }
            //save to file
            string comparedRecordsFile = @"C:\Users\MSBZ\Desktop\comparedRecords.txt";
            SaveSummary(masterTable.RowsCount, testTable.RowsCount, CompareTable.Data.Count, CompareTable.Extra.Count);
            PerfCounter.Start();
            CompareTable.SaveToFile(comparedRecordsFile);
            PerfCounter.Stop("Save comparison");
            PerfCounter.SaveAllResults();
        }

        private void SaveSummary(int mRowsCount, int tRowsCount, int compRowsCount, int extraRowsCount) {
            var actRowsDiff = mRowsCount >= tRowsCount ? mRowsCount - tRowsCount : tRowsCount - mRowsCount;
            File.WriteAllText(@"C:\Users\MSBZ\Desktop\Summary.txt",
                "Master: " + mRowsCount + ";Test: " + tRowsCount + ";Act diff: " + actRowsDiff + ";Compared: " + compRowsCount + ";Extra: " + extraRowsCount);
        }

        private List<ComparedRow> ProcessGroup(List<Row> masterRows, List<Row> testRows, ref int run) {
            var comparedRows = new List<ComparedRow>();
            //masterTable.SaveToFile(@"C:\Users\MSBZ\Desktop\tempMaster.txt");
            //testTable.SaveToFile(@"C:\Users\MSBZ\Desktop\tempTest.txt");
            //var stat = GatherStatistics(masterRows, testRows);
            //var newKeysIndexes = AnalyseInGroup(stat);
            //File.AppendAllLines(@"C:\Users\MSBZ\Desktop\nonUStat.txt", stat.Select(r => run + ";" + r.ToString()));
            //if (newKeysIndexes.Count == 0) {
            //    ApplyRowNumberInGroup(masterRows, compKeysIndexes);
            //    ApplyRowNumberInGroup(testRows, compKeysIndexes);
            //    comparedRows.AddRange(Match(masterRows, testRows, compKeysIndexes));
            //} else {

            run += masterRows.Count;

            var allreadyMatched = new List<int>();
            foreach (var masterRow in masterRows) {
                var testRowsChunk = testRows.Where(item => !allreadyMatched.Contains(item.Id)).ToList();
                Row testRow;
                if (testRowsChunk.Count == 0) {
                    break;
                } else if (testRowsChunk.Count == 1) {
                    testRow = testRowsChunk.First();                  
                } else {
                    testRow = MatchSingle(masterRow, testRowsChunk);
                }
                comparedRows.Add(Compare(masterRow, testRow));
                allreadyMatched.Add(testRow.Id);
            }

            //var grpMaster = Group(masterRows, newKeysIndexes);
            //var grpTest = Group(testRows, newKeysIndexes);
            //var uMasterRows = grpMaster.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
            //var uTestRows = grpTest.Where(r => r.Value.Count() == 1).SelectMany(r => r.Value).ToList();
            //comparedRows.AddRange(Match(uMasterRows, uTestRows, newKeysIndexes));
            //var mRemainings = grpMaster.Where(r => r.Value.Count() > 1);
            //var tRemainings = grpTest.Where(r => r.Value.Count() > 1);
            //if (mRemainings.Count() > 0 && tRemainings.Count() > 0) {
            //    var groups = from mRem in mRemainings join tRem in tRemainings on mRem equals tRem select mRem;
            //    foreach (var item in groups) {
            //        ProcessGroup(mRemainings.SelectMany(row => row.Value).ToList(), tRemainings.SelectMany(row => row.Value).ToList(), newKeysIndexes, run + 0.01);
            //    }
            //}
            //}
            return comparedRows;
        }

        private Row MatchSingle(Row masterRow, List<Row> testRows) {
            Dictionary<int, List<int>> map = new Dictionary<int, List<int>>();
            int index = 0;
            foreach (var testRow in testRows) {
                map.Add(index, new List<int>());
                for (int columnIndex = 0; columnIndex < testRow.Data.Length; columnIndex++) {
                    if (masterRow.Data[columnIndex] != testRow.Data[columnIndex]) {
                        map[index].Add(columnIndex);
                    }
                }
                index++;
            }
            int min = map.Min(item => item.Value.Count);
            var bestMatch = map.Where(item => item.Value.Count == min).ToList();
            int matchedRowIndex = -1;
            if (bestMatch.Count == 1) {
                matchedRowIndex = bestMatch.First().Key;
                return testRows[matchedRowIndex];
            } else {
                var isSameColumns = min == bestMatch.SelectMany(item => item.Value).Distinct().Count();
                if (isSameColumns) {
                    matchedRowIndex = SameColumnsDiffHandler(bestMatch.Select(item => item.Value).First(), masterRow, testRows);
                } else {
                    //matchedRowIndex = DiffColumnsDiffHandler();
                    matchedRowIndex = 0;
                }              
                //File.WriteAllText(@"C:\Users\MSBZ\Desktop\mRow.txt", string.Join(";", masterRow.Data));
                //File.WriteAllLines(@"C:\Users\MSBZ\Desktop\testRows.txt", testRows.Select(i => string.Join(";", i.Data)));
                //File.WriteAllLines(@"C:\Users\MSBZ\Desktop\map.txt", map.Select(i => string.Join(";", i.Value)));
                return testRows[matchedRowIndex];
            }       
        }

        private int SameColumnsDiffHandler(List<int> diffColumns, Row masterRow, List<Row> testRows) {
            int index = -1;
            var queryStat = 
                    from col in diffColumns
                    join stat in BaseStat on col equals stat.Id
                    select stat;
            if(queryStat.All(item => item.IsString)) {
                index = 0;
            } else {
                var numFields = queryStat.Where(item => !item.IsString).Select(item => item.Id);
                var mRowNumFileds = masterRow.ColumnIndexIn(numFields).ToList();
                var tRowsNumFields = testRows.Select(item => item.ColumnIndexIn(numFields)).ToList();
                Dictionary<int, List<double>> map = new Dictionary<int, List<double>>();
                for (int rowsIndex = 0; rowsIndex < tRowsNumFields.Count; rowsIndex++) {               
                    map.Add(rowsIndex, new List<double>());
                    for (int columnsIndex = 0; columnsIndex < mRowNumFileds.Count; columnsIndex++) {
                        double mNum = 0;
                        var itemM = mRowNumFileds[columnsIndex];
                        double.TryParse(itemM, out mNum);
                        
                        double tNum = 0;
                        var itemT = tRowsNumFields[rowsIndex][columnsIndex];
                        double.TryParse(itemT, out tNum);

                        var diff = mNum - tNum;
                        map[rowsIndex].Add(diff);
                    }
                }
                var min = map.Min(item => item.Value.Sum());
                index = map.Where(item => item.Value.Sum() == min).First().Key;
            }
            return index;
        }

        private int DiffColumnsDiffHandler() {
            int index = 0;
            return index;
        }

        public void ApplyRowNumberInGroup(IEnumerable<Row> rows, List<int> compKeys) {
            var query = from r in rows
                        group r by r.GetValuesHashCode(compKeys)
                        into g
                        where g.Count() > 1
                        //orderby g.Select(r=>r.MaterialiseKey(orderBy))                       
                        select g;
            foreach (var group in query) {
                int RowNumber = 0;
                foreach (var row in group) {
                    row.GroupId = RowNumber++;
                }
            }
        }

        private ComparedRow Compare(Row masterRow, Row testRow) {
            ComparedRowSB.Clear();
            int rowDiff = 0;
            var idColumnsData = string.Join(Delimiter, masterRow.ColumnIndexIn(PivotKeysIndexes));
            for (int i = 0; i < masterRow.Data.Count(); i++) {
                if (masterRow.Data[i] == testRow.Data[i]) {
                    ComparedRowSB.Add("0");
                    //ComparedRowSB.Append(Delimiter);
                } else {
                    ComparedRowSB.Add(masterRow.Data[i] + " | " + testRow.Data[i]);
                    //ComparedRowSB.Add(" | ");
                    //ComparedRowSB.Add(testRow.Data.ElementAt(i));
                    //ComparedRowSB.Append(Delimiter);
                    rowDiff++;
                }
            }
            return new ComparedRow(0, masterRow.Id, testRow.Id, rowDiff, idColumnsData, ComparedRowSB.ToArray());
        }

        private IEnumerable<string> GetExtraRows(IWorkTable table, IEnumerable<int> comparedId) {
            var filter = table.Rows.Select(r => r.Id).Except(comparedId);
            return from r in table.Rows
                   join f in filter on r.Id equals f
                   select r.Id + table.Delimiter + table.Name + table.Delimiter + string.Join(table.Delimiter, r.Data);
        }

        private IEnumerable<ComparedRow> Match(IEnumerable<Row> masterRows, IEnumerable<Row> testRows, List<int> compKeys) {
            return from m in masterRows
                   join t in testRows on new { HashKey = m.GetValuesHashCode(compKeys), RowNumber = m.GroupId }
                   equals new { HashKey = t.GetValuesHashCode(compKeys), RowNumber = t.GroupId }
                   select Compare(m, t);
        }

        private List<int> AnalyseInGroup(List<ColumnSummary> columnsStat) {
            var uniqKey = columnsStat.FirstOrDefault(col => col.UniqMatchRate == 100);
            var clearedStats = columnsStat.Where(col => col.MatchingRate != 0 && !col.IsDouble).ToList();
            if (uniqKey != null) {
                return new List<int>() { uniqKey.Id };
            } else if (clearedStats.Count == 0) {
                return new List<int>();
            } else {
                var maxMatchingRate = clearedStats.Max(col => col.MatchingRate);
                var maxUniqMatchRate = clearedStats.Where(col => col.MatchingRate == maxMatchingRate).Max(col => col.UniqMatchRate);
                var compKeys = clearedStats.Where(col => col.MatchingRate == maxMatchingRate && col.UniqMatchRate == maxUniqMatchRate).Select(col => col.Id);
                return compKeys.ToList();
            }
        }

        private Dictionary<int, List<Row>> Group(IEnumerable<Row> rows, List<int> pivotFields) {
            return rows.GroupBy(col => col.GetValuesHashCode(pivotFields)).ToDictionary(group => group.Key, group => group.ToList());
        }

        private bool IsUsefulInCompositeKey(Dictionary<int, List<Row>> rows, List<int> pivotFields, int analysisColumn) {
            return rows.Any(grp => grp.Value.Select(row => row[analysisColumn]).Distinct().Count() > 1);
        }

        private List<int> AnalyseForPivotKey(IEnumerable<Row> sampleRows, List<ColumnSummary> columnsStat) {
            var uniqKey = columnsStat.FirstOrDefault(col => col.UniqMatchRate == 100);
            if (uniqKey != null) {
                return new List<int>() { uniqKey.Id };
            }
            var clearedStats = columnsStat.Where(col => !col.IsDouble && !col.HasNulls);
            var maxMatchingRate = clearedStats.Max(col => col.MatchingRate);
            var maxUniqMatchRate = clearedStats.Where(col => col.MatchingRate == maxMatchingRate).Max(col => col.UniqMatchRate);
            var mainPivotKey = clearedStats.Where(col => col.MatchingRate == maxMatchingRate && col.UniqMatchRate == maxUniqMatchRate).First().Id;
            var additionalKeys = clearedStats.Where(col => col.MatchingRate == maxMatchingRate && col.Id != mainPivotKey).Select(col => col.Id);
            List<int> compositeKey = new List<int>() { mainPivotKey };
            var groups = Group(sampleRows, compositeKey);
            foreach (var key in additionalKeys) {
                if (IsUsefulInCompositeKey(groups, compositeKey, key)) {
                    compositeKey.Add(key);
                    groups = Group(groups.Where(grp => grp.Value.Count() > 1).SelectMany(row => row.Value), compositeKey);
                }
            }
            return compositeKey;
        }

        //public async Task<Dictionary<int, HashSet<string>>> GetColumnsAsync(IEnumerable<Row> rows) {
        //    return await Task.Run(() => GetColumns(rows));
        //}

        private Dictionary<int, HashSet<string>> GetColumns(IEnumerable<Row> rows) {
            Dictionary<int, HashSet<string>> columns = new Dictionary<int, HashSet<string>>();
            var firstLine = rows.FirstOrDefault();
            var columnsCount = firstLine.Data.Length;
            for (int i = 0; i < columnsCount; i++) {
                HashSet<string> uniqVals = new HashSet<string>();
                columns.Add(i, uniqVals);
            }
            int index = 0;
            foreach (var row in rows) {
                foreach (var item in row.Data) {
                    columns[index++].Add(item);
                }
                index = 0;
            }
            return columns;
        }

        private List<ColumnSummary> GatherStatistics(IEnumerable<Row> masterRows, IEnumerable<Row> testRows) {
            var masterColumns = GetColumns(masterRows);
            var testColumns = GetColumns(testRows);
            var totalRowsCount = masterRows.Count() > testRows.Count() ? testRows.Count() : masterRows.Count();
            if (totalRowsCount == 0) {
                return new List<ColumnSummary>();
            }
            var columnsSummary = masterColumns.Join(testColumns,
                m => m.Key,
                t => t.Key,
                (m, t) => new ColumnSummary(m.Key, totalRowsCount, m.Value, t.Value)).ToList();
            return columnsSummary;
        }
    }
}
