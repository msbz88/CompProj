using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompProj.Models;
using CompProj.Models.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace CompProjTest {
    [TestClass]
    public class ComperisonProcTest {
        IWorkTable Master;
        IWorkTable Test;

        public void PrepareTestData() {
            List<string> masterInit = new List<string>();
            masterInit.Add("SecurityId;Portfolio;Bal");
            masterInit.Add("S1;P1;10");
            masterInit.Add("S2;P2;100");
            masterInit.Add("S3;P3;1000.10");
            masterInit.Add("S4;P4;10");
            List<string> testInit = new List<string>();
            testInit.Add("SecurityId;Portfolio;Bal");
            testInit.Add("S1;P1;10");
            testInit.Add("S2;P2;100");
            testInit.Add("S3;P3;1000.10");
            LoadTestData(masterInit, testInit);
        }

        private void LoadTestData(List<string> masterInit, List<string> testInit) {
            Master = new WorkTable();
            Test = new WorkTable();
            Master.LoadDataAsync(masterInit, ';', true);
            Test.LoadDataAsync(masterInit, ';', true);
        }

        [TestMethod]
        public void TestNumberOfRecords() {
            PrepareTestData();
            IComparisonProcessor compProc = new ComparisonProcessor();
            var expected = 4;
            var actual = compProc.Execute(Master, Test).Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestContent() {
            PrepareTestData();
            IComparisonProcessor compProc = new ComparisonProcessor();
            List<string> expected = new List<string>();
            expected.Add("SecurityId;Portfolio;Bal");
            expected.Add("0;0;0");
            expected.Add("0;0;0");
            expected.Add("0;0;0");
            List<string> actual = compProc.Execute(Master, Test);
            CollectionAssert.AreEqual(expected, actual);
        }



    }
}
