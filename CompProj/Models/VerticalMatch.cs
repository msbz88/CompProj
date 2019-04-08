using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class VerticalMatch {
        private List<ColumnSummary> BaseStat { get; set; }
        List<RowMap> MatchedRows { get; set; }
        List<ComparedRow> ComparedRows { get; set; }
        List<string> ComparedRowSB = new List<string>();

        public VerticalMatch(List<ColumnSummary> baseStat) {
            BaseStat = baseStat;
            MatchedRows = new List<RowMap>();
            ComparedRows = new List<ComparedRow>();
        }

        private List<RowMap> CreateMap(List<Row> masterRows, List<Row> testRows) {
            var map = new List<RowMap>();
            foreach (var mRow in masterRows) {
                foreach (var tRow in testRows) {
                    List<ColumnSummary> diffColumns = new List<ColumnSummary>();
                    for (int columnIndex = 0; columnIndex < tRow.Data.Length; columnIndex++) {
                        if (mRow.Data[columnIndex] != tRow.Data[columnIndex]) {
                            diffColumns.Add(BaseStat.Where(item => item.Id == columnIndex).First());
                        }
                    }
                    map.Add(new RowMap(mRow.Id, tRow.Id, diffColumns));
                }
            }
            return map;
        }

        private List<RowMap> CleanMap(List<RowMap> map) {
            return map
                .GroupBy(row => row.MasterRowID)
                .Select(grp => grp.Where(item => item.DiffColumns.Count == grp.Min(i => i.DiffColumns.Count)))
                .SelectMany(row => row).ToList();
        }

        public List<ComparedRow> ProcessGroup(List<Row> masterRows, List<Row> testRows) {
            MatchedRows.Clear();
            ComparedRows.Clear();
            var map = CreateMap(masterRows, testRows);
            while (map.Count > 0) {
                int min = map.Min(item => item.DiffColumns.Count);
                var rowsChunk = map.Where(item => item.DiffColumns.Count == min);

                IEnumerable<RowMap> checkM;
                IEnumerable<RowMap> uniqMatch;

                if (min == 0) {
                    checkM = rowsChunk.GroupBy(item => item.MasterRowID).Select(item => item.First());
                    uniqMatch = checkM.GroupBy(item => item.TestRowID).Select(item => item.First());
                    map = map.Where(item => !checkM.Contains(item)).ToList();
                } else {
                    checkM = rowsChunk.GroupBy(item => item.MasterRowID).Where(item => item.Count() == 1).SelectMany(item => item);
                    uniqMatch = checkM.GroupBy(item => item.TestRowID).Where(item => item.Count() == 1).SelectMany(item => item);
                    map = map.Where(item => !uniqMatch.Contains(item)).ToList();
                }

                MatchedRows.AddRange(uniqMatch);
                if (map.Count == 0) {
                    break;
                }else if (map.Count == 1) {
                    var single = map.First();
                    MatchedRows.Add(single);
                    map = map.Where(item => single != item).ToList();
                    break;
                } else {
                    var diffColumns = map.SelectMany(item => item.DiffColumns).Distinct().OrderByDescending(item => item.UniqMatchRate).Select(item => item.Id).ToList();

                    var orderedMasterRows = masterRows
                        .Where(item => map.Select(x => x.MasterRowID).Contains(item.Id))
                        .OrderByDescending(item => string.Join("", item.ColumnIndexIn(diffColumns)))
                        .Select(row => row.Id).ToList();
                    var orderedTestRows = testRows
                        .Where(item => map.Select(x => x.TestRowID).Contains(item.Id))
                        .OrderByDescending(item => string.Join("", item.ColumnIndexIn(diffColumns)))
                        .Select(row => row.Id).ToList();

                    var count = orderedMasterRows.Count > orderedTestRows.Count ? orderedTestRows.Count : orderedMasterRows.Count;
                    

                    for (int i = 0; i < count; i++) {
                        MatchedRows.Add(new RowMap(orderedMasterRows[i], orderedTestRows[i], new List<ColumnSummary>()));
                    }
                    map = map.Where(item => !MatchedRows.Contains(item)).ToList();
                }
            }
            foreach (var item in MatchedRows) {
                var masterRow = masterRows.Where(row => row.Id == item.MasterRowID).First();
                var testRow = testRows.Where(row => row.Id == item.TestRowID).First();
                ComparedRows.Add(Compare(masterRow, testRow));
            }
            return ComparedRows;
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




    }
}
