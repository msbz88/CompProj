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

        PerformanceCounter perfCounter = new PerformanceCounter();
        List<string> timings = new List<string>();

        public ComparisonHelper(IFileReader fileReader, IImpConfig importConfiguration) {
            FileReader = fileReader;
            ImportConfiguration = importConfiguration;
        }

        public async void PrepareComparison() {
            perfCounter.Start();
            MasterFileContent = await FileReader.ReadAllLinesAsync(ImportConfiguration.PathMasterFile, ImportConfiguration.Encoding, ImportConfiguration.BufferSize, ImportConfiguration.HeadersRow);
            TestFileContent = await FileReader.ReadAllLinesAsync(ImportConfiguration.PathTestFile, ImportConfiguration.Encoding, ImportConfiguration.BufferSize, ImportConfiguration.HeadersRow);
            perfCounter.Stop();
            timings.Add("Read two init files;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);

            perfCounter.Start();
            ExceptedMasterData = ExceptAsync(MasterFileContent, TestFileContent, ImportConfiguration.IsHeadersExist);
            ExceptedTestData = ExceptAsync(TestFileContent, MasterFileContent, ImportConfiguration.IsHeadersExist);
            perfCounter.Stop();
            timings.Add("Except two files;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);

            File.WriteAllText(@"C:\Users\MSBZ\Desktop\[res.txt", MasterFileContent[0]);
            File.AppendAllLines( @"C:\Users\MSBZ\Desktop\[res.txt", ExceptedMasterData);        

            File.WriteAllText(@"C:\Users\MSBZ\Desktop\]res.txt", TestFileContent[0]);
            File.AppendAllLines(@"C:\Users\MSBZ\Desktop\]res.txt", ExceptedTestData);

            WorkTable master = new WorkTable();
            WorkTable test = new WorkTable();

            perfCounter.Start();
            master.LoadDataAsync(ExceptedMasterData, ImportConfiguration.Delimiter, ImportConfiguration.IsHeadersExist);          
            test.LoadDataAsync(ExceptedTestData, ImportConfiguration.Delimiter, ImportConfiguration.IsHeadersExist);
            perfCounter.Stop();
            timings.Add("Load two files to WorkTable;" + perfCounter.ElapsedTimeMs + ";" + perfCounter.UsedMemory);

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
