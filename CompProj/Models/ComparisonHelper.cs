using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models.Interfaces;

namespace CompProj.Models {
    public class ComparisonHelper {
        IFileReader FileReader { get; set; }
        IFileConfiguration FileConfiguration { get; set; }
        PerformanceCounter perfCounter = new PerformanceCounter();

        public ComparisonHelper(IFileReader fileReader, IFileConfiguration fileConfiguration) {
            FileReader = fileReader;
            FileConfiguration = fileConfiguration;
        }

        public async Task PrepareForComparison() {

            perfCounter.Start();
            var masterFileContent = FileReader.ReadFile(FileConfiguration.PathMasterFile, FileConfiguration.RowsToSkip, FileConfiguration.Encoding);
            var testFileContent = FileReader.ReadFile(FileConfiguration.PathTestFile, FileConfiguration.RowsToSkip, FileConfiguration.Encoding);        
            perfCounter.Stop("Read two init files");

            perfCounter.Start();
            var exceptedMasterData = Except(masterFileContent, testFileContent);
            var exceptedTestData = Except(testFileContent, masterFileContent);
            perfCounter.Stop("Except files");

            IWorkTable masterTable = new WorkTable("Master");
            IWorkTable testTable = new WorkTable("Test");
          
            if (exceptedMasterData.Any() && exceptedTestData.Any()) {
                perfCounter.Start();
                masterTable.LoadData(exceptedMasterData, FileConfiguration.Delimiter, FileConfiguration.IsHeadersExist);
                testTable.LoadData(exceptedTestData, FileConfiguration.Delimiter, FileConfiguration.IsHeadersExist);
                perfCounter.Stop("Load two files to WorkTable");
                ComparisonProcessor comparisonProcessor = new ComparisonProcessor(perfCounter);
                await comparisonProcessor.Execute(masterTable, testTable);
            } else {
                perfCounter.SaveAllResults();
            }
        }

        private IEnumerable<string> Except(IEnumerable<string> dataFirst, IEnumerable<string> dataSecond) {
            IEnumerable<string> headersLine = Enumerable.Empty<string>();
            if (FileConfiguration.IsHeadersExist) {
                headersLine = dataFirst.Take(1);
            }
            var uniqLines = new HashSet<int>(dataSecond.Select(line=>line.GetHashCode()));
            return headersLine.Concat(dataFirst.Where(x => !uniqLines.Contains(x.GetHashCode())));
        }

        //private List<int> ColumnsToExclude(List<ColumnSummary> columnsSummary) {
        //    List<int> result = new List<int>();
        //    var maxUniqRate = columnsSummary.Where(item => !item.HasNulls && !item.IsString).Max(item => item.UniquenessRate);
        //    var minMatchRate = columnsSummary.Where(item => item.UniquenessRate == maxUniqRate).Min(item => item.MatchingRate);
        //    result = columnsSummary.Where(item => item.UniquenessRate == maxUniqRate && item.MatchingRate == minMatchRate).Select(item=>item.Id).ToList();
        //    result.AddRange(columnsSummary.Where(item=>item.MatchingRate<10).Select(item=>item.Id));
        //    return result;
        //}

       
    }
}
