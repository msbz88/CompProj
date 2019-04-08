using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonProcessor /*: IComparisonProcessor */{
        public int MasterRowsCount { get; set; }
        public int TestRowsCount { get; set; }
        public int ActualRowsDiff { get; set; }       
        public int ComparedRowsCount { get; set; }
        public int ExtraRowsCount { get; set; }
        VerticalMatch verticalMatch;
        List<string> ComparedRowSB = new List<string>();
        PerformanceCounter PerfCounter;
        CompareTable CompareTable;
        IWorkTable MasterTable;
        IWorkTable TestTable;
        string Delimiter;
        public List<ColumnSummary> BaseStat;
        List<int> PivotKeysIndexes { get; set; }

        public ComparisonProcessor(PerformanceCounter perfCounter) {
            PerfCounter = perfCounter;
        }

        public CompareTable Execute(IWorkTable masterTable, IWorkTable testTable) {
            MasterTable = masterTable;
            TestTable = testTable;
            //var mm = masterTable.Rows.Where(item => item.Data[10] == "373522");
            //var tt = testTable.Rows.Where(item => item.Data[10] == "373522");
            Delimiter = masterTable.Delimiter;
            //gather base stat  
            PerfCounter.Start();
            BaseStat = GatherStatistics(masterTable.Rows, testTable.Rows);
            PerfCounter.Stop("Base Gather Stat");
            //analyse
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\baseStat.txt", BaseStat.Select(r => r.ToString()));
            PerfCounter.Start();
            PivotKeysIndexes = AnalyseForPivotKey(masterTable.Rows, BaseStat);
            PerfCounter.Stop("AnalyseForPivotKey");
            File.AppendAllText(@"C:\Users\MSBZ\Desktop\baseStat.txt", "baseKeyIndex: " + string.Join(";", masterTable.Headers.ColumnIndexIn(PivotKeysIndexes)));
            //group
            PerfCounter.Start();
            var groupsM = Group(masterTable.Rows, PivotKeysIndexes);
            var groupsT = Group(testTable.Rows, PivotKeysIndexes);
            PerfCounter.Stop("Base Group");
            //File.WriteAllText(@"C:\Users\MSBZ\Desktop\groupsM.txt", "Hash" + ";" + string.Join(";", masterTable.Headers.ColumnIndexIn(PivotKeysIndexes)) + Environment.NewLine);
            //File.AppendAllLines(@"C:\Users\MSBZ\Desktop\groupsM.txt", groupsM.SelectMany(item => item.Value.Select(it => it.GetValuesHashCode(PivotKeysIndexes) + ";" + string.Join(";", it.ColumnIndexIn(PivotKeysIndexes)))));
            //File.WriteAllText(@"C:\Users\MSBZ\Desktop\groupsT.txt", "Hash" + ";" + string.Join(";", masterTable.Headers.ColumnIndexIn(PivotKeysIndexes)) + Environment.NewLine);
            //File.AppendAllLines(@"C:\Users\MSBZ\Desktop\groupsT.txt", groupsT.SelectMany(item => item.Value.Select(it => it.GetValuesHashCode(PivotKeysIndexes) + ";" + string.Join(";", it.ColumnIndexIn(PivotKeysIndexes)))));
            //PerfCounter.Start();
            var compareHeaders = masterTable.Headers.ColumnIndexIn(PivotKeysIndexes).ToList();
            compareHeaders.AddRange(masterTable.Headers.Data);
            CompareTable = new CompareTable(masterTable, testTable, PivotKeysIndexes);
            //var uMasterRows = groupsM.Where(r => r.Value.Count() == 1).ToDictionary(item => item.Key, item => item.Value);
            //var uTestRows = groupsT.Where(r => r.Value.Count() == 1).ToDictionary(item => item.Key, item => item.Value);
            //CompareTable.Data.AddRange(Match(uMasterRows.SelectMany(item=>item.Value), uTestRows.SelectMany(item => item.Value), PivotKeysIndexes));
            //PerfCounter.Stop("Preparison");

            PerfCounter.Start();
            //var mRemainings = groupsM.Where(r => !uMasterRows.Keys.Contains(r.Key)).ToDictionary(item => item.Key, item => item.Value);
            //var tRemainings = groupsT.Where(r => !uTestRows.Keys.Contains(r.Key)).ToDictionary(item => item.Key, item => item.Value);

            verticalMatch = new VerticalMatch(BaseStat);

            //var test = masterTable.Rows.Where(item=>item.Data[117] == "CRT-U XTSE" && item.Data[225] == "20180727");
            //var test2 = groupsM.Values.SelectMany(item => item.Where(i=>i.Data[117] == "CRT-U XTSE" && i.Data[225] == "20180727").Select(i=>i.Id));

            //var test3 = testTable.Rows.Where(item => item.Data[117] == "CRT-U XTSE" && item.Data[225] == "20180727");
            //var test4 = groupsT.Values.SelectMany(item => item.Where(i => i.Data[117] == "CRT-U XTSE" && i.Data[225] == "20180727").Select(i => i.Id));

            //var test5 = groupsM.Where(item => item.Value.Select(i => i.Id).Contains(58)).First().Key;
            //var test6 = groupsT.Where(item => item.Value.Select(i => i.Id).Contains(25382)).First().Key;

            //File.AppendAllText(@"C:\Users\MSBZ\Desktop\ex.txt", test5);
            //File.AppendAllText(@"C:\Users\MSBZ\Desktop\ex.txt", "*****");
            //File.AppendAllText(@"C:\Users\MSBZ\Desktop\ex.txt", test6);

            var groups = from m in groupsM
                         join t in groupsT on m.Key equals t.Key
                         select ProcessGroup(m.Value, t.Value);

            var res = groups.SelectMany(r => r);
            if (res.Any()) {
                CompareTable.Data.AddRange(res);
            }
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
            FillSummary(masterTable.RowsCount, testTable.RowsCount, CompareTable.Data.Count, CompareTable.Extra.Count);
            PerfCounter.Start();
            CompareTable.SaveToFile(comparedRecordsFile);
            PerfCounter.Stop("Save comparison");
            PerfCounter.SaveAllResults();
            return CompareTable;
        }

        private List<ComparedRow> ProcessGroup(List<Row> masterRows, List<Row> testRows) {
            return verticalMatch.ProcessGroup(masterRows, testRows);
        }

        private void FillSummary(int mRowsCount, int tRowsCount, int compRowsCount, int extraRowsCount) {
            var actRowsDiff = mRowsCount >= tRowsCount ? mRowsCount - tRowsCount : tRowsCount - mRowsCount;
            MasterRowsCount = mRowsCount;
            TestRowsCount = tRowsCount;
            ActualRowsDiff = actRowsDiff;
            ComparedRowsCount = compRowsCount;
            ExtraRowsCount = extraRowsCount;
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
            //var idColumnsData = string.Join(Delimiter, masterRow.ColumnIndexIn(PivotKeysIndexes));
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
            return new ComparedRow(0, masterRow.Id, testRow.Id, rowDiff, "", ComparedRowSB.ToArray());
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

        private Dictionary<string, List<Row>> Group(IEnumerable<Row> rows, List<int> pivotFields) {
            return rows.GroupBy(col => string.Join(" | ",col.ColumnIndexIn(pivotFields))).ToDictionary(group => group.Key, group => group.ToList());
        }

        private bool IsUsefulInCompositeKey(Dictionary<string, List<Row>> rows, List<int> pivotFields, int analysisColumn) {
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

        public List<ColumnSummary> GatherStatistics(IEnumerable<Row> masterRows, IEnumerable<Row> testRows) {
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
            return columnsSummary.ToList();
        }
    }
}
