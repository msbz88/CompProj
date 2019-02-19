using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void PrepareComparison() {
            perfCounter.Start();
            var MasterFileContent = Task.Run(()=>FileReaderNew(ImportConfiguration.PathMasterFile).ToList());
            var TestFileContent = Task.Run(() => FileReaderNew(ImportConfiguration.PathTestFile).ToList());
            Task.WaitAll(MasterFileContent, TestFileContent);
            perfCounter.Stop();
            timings.Add("Read two init files;" + perfCounter.ElapsedTimeMs);

            perfCounter.Start();
            var ExceptedMasterData = Task.Run(() => ExceptFiles(MasterFileContent.Result, TestFileContent.Result, ImportConfiguration.IsHeadersExist));
            var ExceptedTestData = Task.Run(() => ExceptFiles(TestFileContent.Result, MasterFileContent.Result, ImportConfiguration.IsHeadersExist));
            Task.WaitAll(ExceptedMasterData, ExceptedTestData);
            perfCounter.Stop();
            timings.Add("Except two files;" + perfCounter.ElapsedTimeMs);

            WorkTable master = new WorkTable("Master");
            WorkTable test = new WorkTable("Test");

            perfCounter.Start();
            var masterLoad = Task.Run(()=>master.LoadDataAsync(ExceptedMasterData.Result, ImportConfiguration.Delimiter, ImportConfiguration.IsHeadersExist));
            var testLoad = Task.Run(() => test.LoadDataAsync(ExceptedTestData.Result, ImportConfiguration.Delimiter, ImportConfiguration.IsHeadersExist));
            Task.WaitAll(masterLoad, testLoad);
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
            //perfCounter.Start();
            //List<string> res = new List<string>();
            //var master = FileReaderNew(ImportConfiguration.PathMasterFile).ToList();
            //var test = FileReaderNew(ImportConfiguration.PathTestFile).ToList();
            //var count = CountColums(ImportConfiguration.PathMasterFile);

            //Parallel.For(0, count, i => {
            //    var mColumn = GetColumn(master, i);
            //    var tColumn = GetColumn(test, i);
            //    var resM = Group(mColumn);
            //    var resT = Group(tColumn);
            //    var total = from m in resM join t in resT on m equals t select m;
            //    res.Add(total.Count().ToString());
            //});

            //perfCounter.Stop();
            //timings.Add("Process;" + perfCounter.ElapsedTimeMs);
            //File.WriteAllLines(@"C:\Users\MSBZ\Desktop\timings.txt", timings);
            //File.WriteAllLines(@"C:\Users\MSBZ\Desktop\count.txt", res);
        }

        private IEnumerable<string> GetColumn(IEnumerable<string[]> collection, int pos) {
            return collection.Select(r => r[pos]);
        }

        private List<string> FileReaderNew(string path) {
            return File.ReadLines(path).ToList();
        }

        private int CountColums(string path) {
            return File.ReadLines(path)
                .FirstOrDefault().Split(ImportConfiguration.Delimiter).Length;
        }

        private IEnumerable<string> Group(IEnumerable<string> colection) {
            return colection.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key);
        }


        private List<string> ExceptFiles(List<string> list1, List<string> list2, bool isHeadersExist) {
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
