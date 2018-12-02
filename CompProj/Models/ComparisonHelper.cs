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
        public List<string> MasterFileContent { get; private set;}
        public List<string> TestFileContent { get; private set;}
        public List<string> ExceptedMasterData { get; private set;}
        public List<string> ExceptedTestData { get; private set;}
        IImpConfig ImportConfiguration { get; set; }

        public ComparisonHelper(IFileReader fileReader, IImpConfig importConfiguration) {
            FileReader = fileReader;
            ImportConfiguration = importConfiguration;
        }

        public async void PrepareComparison() {
            MasterFileContent = await FileReader.ReadAllLinesAsync(ImportConfiguration.PathMasterFile, ImportConfiguration.Encoding, ImportConfiguration.BufferSize, ImportConfiguration.RowsToSkip);
            TestFileContent = await FileReader.ReadAllLinesAsync(ImportConfiguration.PathTestFile, ImportConfiguration.Encoding, ImportConfiguration.BufferSize, ImportConfiguration.RowsToSkip);
            ExceptedMasterData = ExceptAsync(MasterFileContent, TestFileContent);
            File.WriteAllLines( @"C:\Users\MSBZ\Desktop\[res.txt", ExceptedMasterData);
            ExceptedTestData = ExceptAsync(TestFileContent, MasterFileContent);
            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\]res.txt", ExceptedTestData);
        }

        private List<string> ExceptAsync(List<string> list1, List<string> list2) {
            return list1.Except(list2).ToList();
        }

    }
}
