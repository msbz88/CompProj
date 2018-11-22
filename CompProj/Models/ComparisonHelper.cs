using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class ComparisonHelper {
        IFileReader FileReader { get; set; }
        public List<string> MasterFileContent { get; private set;}
        public List<string> TestFileContent { get; private set;}
        public List<string> ExceptedMasterData { get; private set;}
        public List<string> ExceptedTestData { get; private set;}

        public ComparisonHelper(IFileReader fileReader) {
            FileReader = fileReader;
        }

        private async void PrepareComparison(string pathMasterFile, string pathTestFile, Encoding encoding, int bufferSize, int numRowsToSkip) {
            MasterFileContent = await FileReader.ReadAllLinesAsync(pathMasterFile, numRowsToSkip);
            TestFileContent = await FileReader.ReadAllLinesAsync(pathMasterFile, numRowsToSkip);
            ExceptedMasterData = ExceptAsync(MasterFileContent, TestFileContent);
            ExceptedTestData = ExceptAsync(TestFileContent, MasterFileContent);
        }

        private List<string> ExceptAsync(List<string> list1, List<string> list2) {
            return list1.Except(list2).ToList();
        }

    }
}
