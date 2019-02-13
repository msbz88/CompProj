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
        IImpConfig ImportConfiguration { get; set; }

        PerformanceCounter perfCounter = new PerformanceCounter();
        List<string> timings = new List<string>();

        public ComparisonHelper(IFileReader fileReader, IImpConfig importConfiguration) {
            FileReader = fileReader;
            ImportConfiguration = importConfiguration;
        }

        public async void PrepareComparison() {
            perfCounter.Start();
            var MasterFileContent = await FileReader.ReadAllLinesAsync(ImportConfiguration.PathMasterFile, ImportConfiguration.Encoding, ImportConfiguration.BufferSize, ImportConfiguration.HeadersRow);
            var TestFileContent = await FileReader.ReadAllLinesAsync(ImportConfiguration.PathTestFile, ImportConfiguration.Encoding, ImportConfiguration.BufferSize, ImportConfiguration.HeadersRow);
            perfCounter.Stop();
            timings.Add("Read two init files;" + perfCounter.ElapsedTimeMs);

            perfCounter.Start();
            var ExceptedMasterData = ExceptAsync(MasterFileContent, TestFileContent, ImportConfiguration.IsHeadersExist);
            var ExceptedTestData = ExceptAsync(TestFileContent, MasterFileContent, ImportConfiguration.IsHeadersExist);
            MasterFileContent.Clear();
            TestFileContent.Clear();
            perfCounter.Stop();
            timings.Add("Except two files;" + perfCounter.ElapsedTimeMs);

            WorkTable master = new WorkTable("Master");
            WorkTable test = new WorkTable("Test");

            perfCounter.Start();
            master.LoadDataAsync(ExceptedMasterData, ImportConfiguration.Delimiter, ImportConfiguration.IsHeadersExist);          
            test.LoadDataAsync(ExceptedTestData, ImportConfiguration.Delimiter, ImportConfiguration.IsHeadersExist);
            ExceptedMasterData.Clear();
            ExceptedTestData.Clear();
            perfCounter.Stop();
            timings.Add("Load two files to WorkTable;" + perfCounter.ElapsedTimeMs);

            //save tables
            //string mFile = @"C:\Users\MSBZ\Desktop\masterTable.txt";
            //string tFile = @"C:\Users\MSBZ\Desktop\testTable.txt";

            //master.SaveToFile(mFile);
            //test.SaveToFile(tFile);

            File.WriteAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);

            ComparisonProcessor comparisonProcessor = new ComparisonProcessor();
            comparisonProcessor.Execute(master, test);
    }

        private List<string> ExceptAsync(List<string> list1, List<string> list2, bool isHeadersExist) {
            List<string> result = new List<string>();
            if (isHeadersExist) {              
                result.Add(list1[0]);
                result.AddRange(list1.Skip(1).Except(list2.Skip(1)));
            } else {
                result.AddRange(list1.Except(list2));
            }
            return result;
        }

    }
}
