using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompProj.Models;

namespace CompProjTest {
    [TestClass]
    public class VerticalMatchTest {
        PerformanceCounter pc = new PerformanceCounter();
        IWorkTable Master;
        IWorkTable Test;

        private void LoadTestData(List<string> masterInit, List<string> testInit) {
            Master = new WorkTable("Master");
            Test = new WorkTable("Test");
            Master.LoadData(masterInit, ";", true);
            Test.LoadData(testInit, ";", true);
        }

        [TestMethod]
        public void TestMirrors() {
            //prepare
            List<string> masterInit = new List<string>();
            masterInit.Add("SecurityId;Portfolio;Bal");
            masterInit.Add("S1;P1;1");
            masterInit.Add("S1;P1;2");
            masterInit.Add("S1;P1;3");
            List<string> testInit = new List<string>();
            testInit.Add("SecurityId;Portfolio;Bal");
            testInit.Add("S1;P1;2");
            testInit.Add("S1;P1;3");
            testInit.Add("S1;P1;4");
            LoadTestData(masterInit, testInit);
            ComparisonProcessor compProc = new ComparisonProcessor(pc);
            var stat = compProc.GatherStatistics(Master.Rows, Test.Rows);
            var vm = new VerticalMatch(stat);
            //expect
            List<string> expectedCompared = new List<string>();
            expectedCompared.Add("2;1");
            expectedCompared.Add("3;2");
            expectedCompared.Add("1;3");
            //actual
            var actualResult = vm.ProcessGroup(Master.Rows, Test.Rows);
            var actualCompared = actualResult.Select(r => r.MasterRowID + ";" + r.TestRowID).ToList();
            //compared
            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
            CollectionAssert.AreEqual(expectedCompared, actualCompared);
        }

        [TestMethod]
        public void TestSameColumns() {
            //prepare
            List<string> masterInit = new List<string>();
            masterInit.Add("SecurityId;Portfolio;Bal;CurVal");
            masterInit.Add("S1;P1;1;111");
            masterInit.Add("S1;P1;2;222");
            masterInit.Add("S1;P1;3;333");
            List<string> testInit = new List<string>();
            testInit.Add("SecurityId;Portfolio;Bal;CurVal");
            testInit.Add("S1;P1;11;1111");
            testInit.Add("S1;P1;22;2222");
            testInit.Add("S1;P1;33;3333");
            LoadTestData(masterInit, testInit);
            ComparisonProcessor compProc = new ComparisonProcessor(pc);
            var stat = compProc.GatherStatistics(Master.Rows, Test.Rows);
            var vm = new VerticalMatch(stat);
            //expect
            List<string> expectedCompared = new List<string>();
            expectedCompared.Add("3;3");
            expectedCompared.Add("2;2");
            expectedCompared.Add("1;1");
            //actual
            var actualResult = vm.ProcessGroup(Master.Rows, Test.Rows);
            var actualCompared = actualResult.Select(r => r.MasterRowID + ";" + r.TestRowID).ToList();
            //compared
            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
            CollectionAssert.AreEqual(expectedCompared, actualCompared);
        }

        [TestMethod]
        public void TestMasterExtraRows() {
            //prepare
            List<string> masterInit = new List<string>();
            masterInit.Add("SecurityId;Portfolio;Bal");
            masterInit.Add("S1;P1;1");
            masterInit.Add("S1;P1;2");
            masterInit.Add("S1;P1;3");
            masterInit.Add("S1;P1;3");
            List<string> testInit = new List<string>();
            testInit.Add("SecurityId;Portfolio;Bal");
            testInit.Add("S1;P1;2");
            testInit.Add("S1;P1;3");
            testInit.Add("S1;P1;4");
            LoadTestData(masterInit, testInit);
            ComparisonProcessor compProc = new ComparisonProcessor(pc);
            var stat = compProc.GatherStatistics(Master.Rows, Test.Rows);
            var vm = new VerticalMatch(stat);
            //expect
            List<string> expectedCompared = new List<string>();
            expectedCompared.Add("2;1");
            expectedCompared.Add("3;2");
            expectedCompared.Add("1;3");
            //actual
            var actualResult = vm.ProcessGroup(Master.Rows, Test.Rows);
            var actualCompared = actualResult.Select(r => r.MasterRowID + ";" + r.TestRowID).ToList();
            //compared
            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
            CollectionAssert.AreEqual(expectedCompared, actualCompared);
        }

        [TestMethod]
        public void TestTestExtraRows() {
            //prepare
            List<string> masterInit = new List<string>();
            masterInit.Add("SecurityId;Portfolio;Bal");
            masterInit.Add("S1;P1;1");
            masterInit.Add("S1;P1;2");
            masterInit.Add("S1;P1;3");
            List<string> testInit = new List<string>();
            testInit.Add("SecurityId;Portfolio;Bal");
            testInit.Add("S1;P1;2");
            testInit.Add("S1;P1;3");
            testInit.Add("S1;P1;4");
            testInit.Add("S1;P1;4");
            LoadTestData(masterInit, testInit);
            ComparisonProcessor compProc = new ComparisonProcessor(pc);
            var stat = compProc.GatherStatistics(Master.Rows, Test.Rows);
            var vm = new VerticalMatch(stat);
            //expect
            List<string> expectedCompared = new List<string>();
            expectedCompared.Add("2;1");
            expectedCompared.Add("3;2");
            expectedCompared.Add("1;3");
            //actual
            var actualResult = vm.ProcessGroup(Master.Rows, Test.Rows);
            var actualCompared = actualResult.Select(r => r.MasterRowID + ";" + r.TestRowID).ToList();
            //compared
            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
            CollectionAssert.AreEqual(expectedCompared, actualCompared);
        }
    }
}
