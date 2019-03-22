//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using CompProj.Models;
//using CompProj.Models.Interfaces;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using CompProj;

//namespace CompProjTest {
//    [TestClass]
//    public class ComperisonProcTest {
//        PerformanceCounter pc = new PerformanceCounter();
//        IWorkTable Master;
//        IWorkTable Test;

//        private void LoadTestData(List<string> masterInit, List<string> testInit) {
//            Master = new WorkTable("Master");
//            Test = new WorkTable("Test");
//            Master.LoadData(masterInit, ';', true);
//            Test.LoadData(testInit, ';', true);
//        }
  
//        [TestMethod]
//        public void TestForMirrors() {
//            //prepare
//            List<string> masterInit = new List<string>();
//            masterInit.Add("SecurityId;Portfolio;Bal");
//            masterInit.Add("S1;P1;1");
//            masterInit.Add("S2;P2;12");
//            masterInit.Add("S2;P2;123");
//            List<string> testInit = new List<string>();
//            testInit.Add("SecurityId;Portfolio;Bal");
//            testInit.Add("S1;P1;1");
//            testInit.Add("S2;P2;123");
//            testInit.Add("S2;P2;12");
//            LoadTestData(masterInit, testInit);
//            ComparisonProcessor compProc = new ComparisonProcessor(pc);
//            //expect
//            List<string> expectedCompared = new List<string>();
//            expectedCompared.Add("0;0;0");
//            expectedCompared.Add("0;0;0");
//            expectedCompared.Add("0;0;0");
//            List<string> expectedExtra = new List<string>();
//            //actual
//            var actualResult = compProc.Execute(Master, Test);
//            var actualCompared = actualResult.Data.Select(r => string.Join(";", r.Data).TrimEnd(';')).ToList();
//            var actualExtra = actualResult.Extra;
//            //compared
//            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
//            CollectionAssert.AreEqual(expectedCompared, actualCompared);
//            //extra
//            Assert.AreEqual(expectedExtra.Count, actualExtra.Count);
//            CollectionAssert.AreEqual(expectedExtra, actualExtra);
//        }

//        [TestMethod]
//        public void TestForZeroOne() {
//            //prepare
//            List<string> masterInit = new List<string>();
//            masterInit.Add("SecurityId;Portfolio;Bal;FRMN;CurVal");
//            masterInit.Add("S1;P1;12;1;5");
//            masterInit.Add("S1;P1;12;0;10");
//            List<string> testInit = new List<string>();
//            testInit.Add("SecurityId;Portfolio;Bal;FRMN;CurVal");
//            testInit.Add("S1;P1;12;0;5");
//            testInit.Add("S1;P1;12;1;10");
//            LoadTestData(masterInit, testInit);
//            ComparisonProcessor compProc = new ComparisonProcessor(pc);
//            //expect
//            List<string> expectedCompared = new List<string>();
//            expectedCompared.Add("0;0;0;1 | 0;0");
//            expectedCompared.Add("0;0;0;0 | 1;0");
//            List<string> expectedExtra = new List<string>();
//            //actual
//            var actualResult = compProc.Execute(Master, Test);
//            var actualCompared = actualResult.Data.Select(r => string.Join(";", r.Data).TrimEnd(';')).ToList();
//            var actualExtra = actualResult.Extra;
//            //compared
//            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
//            CollectionAssert.AreEqual(expectedCompared, actualCompared);
//            //extra
//            Assert.AreEqual(expectedExtra.Count, actualExtra.Count);
//            CollectionAssert.AreEqual(expectedExtra, actualExtra);
//        }

//        [TestMethod]
//        public void TestForRowNumber() {
//            //prepare
//            List<string> masterInit = new List<string>();
//            masterInit.Add("SecurityId;Portfolio;Bal;FRMN;CurVal");
//            masterInit.Add("S1;P1;12;0;10");
//            masterInit.Add("S1;P1;12;0;10");
//            List<string> testInit = new List<string>();
//            testInit.Add("SecurityId;Portfolio;Bal;FRMN;CurVal");
//            testInit.Add("S1;P1;12;1;10");
//            testInit.Add("S1;P1;12;1;10");
//            testInit.Add("S1;P1;12;1;10");
//            LoadTestData(masterInit, testInit);
//            ComparisonProcessor compProc = new ComparisonProcessor(pc);
//            //expect
//            List<string> expectedCompared = new List<string>();
//            expectedCompared.Add("0;0;0;0;0");
//            expectedCompared.Add("0;0;0;0;0");
//            List<string> expectedExtra = new List<string>();
//            expectedExtra.Add("S1;P1;12;1;10");
//            //actual
//            var actualResult = compProc.Execute(Master, Test);
//            var actualCompared = actualResult.Data.Select(r => string.Join(";", r.Data).TrimEnd(';')).ToList();
//            var actualExtra = actualResult.Extra;
//            //compared
//            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
//            CollectionAssert.AreEqual(expectedCompared, actualCompared);
//            //extra
//            Assert.AreEqual(expectedExtra.Count, actualExtra.Count);
//            CollectionAssert.AreEqual(expectedExtra, actualExtra);
//        }

//        [TestMethod]
//        public void TestForBothExtra() {
//            //prepare
//            List<string> masterInit = new List<string>();
//            masterInit.Add("SecurityId;Portfolio;Bal;FRMN");
//            masterInit.Add("S1;P1;12;1");
//            masterInit.Add("S2;P2;12;1");
//            masterInit.Add("S2;P1;12;1");
//            masterInit.Add("S5;P3;13;1");
//            List<string> testInit = new List<string>();
//            testInit.Add("SecurityId;Portfolio;Bal;FRMN");
//            testInit.Add("S1;P1;12;1");
//            testInit.Add("S2;P2;12;1");
//            testInit.Add("S2;P1;12;1");
//            testInit.Add("S6;P3;14;1");
//            LoadTestData(masterInit, testInit);
//            ComparisonProcessor compProc = new ComparisonProcessor(pc);
//            //expect
//            List<string> expectedCompared = new List<string>();
//            expectedCompared.Add("0;0;0;0");
//            expectedCompared.Add("0;0;0;0");
//            expectedCompared.Add("0;0;0;0");
//            List<string> expectedExtra = new List<string>();
//            expectedExtra.Add("S5;P3;13;1");
//            expectedExtra.Add("S6;P3;13;1");
//            //actual
//            var actualResult = compProc.Execute(Master, Test);
//            var actualCompared = actualResult.Data.Select(r => string.Join(";", r.Data).TrimEnd(';')).ToList();
//            var actualExtra = actualResult.Extra;
//            //compared
//            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
//            CollectionAssert.AreEqual(expectedCompared, actualCompared);
//            //extra
//            Assert.AreEqual(expectedExtra.Count, actualExtra.Count);
//            CollectionAssert.AreEqual(expectedExtra, actualExtra);
//        }

//        [TestMethod]
//        public void TestForWrongKey() {
//            //prepare
//            List<string> masterInit = new List<string>();
//            masterInit.Add("TransNo;SecurityId;Portfolio;Bal;FRMN");
//            masterInit.Add("3;S1;P1;10;1");
//            masterInit.Add("1;S2;P2;12;2");
//            masterInit.Add("2;S2;P1;12;1");
//            List<string> testInit = new List<string>();
//            testInit.Add("TransNo;SecurityId;Portfolio;Bal;FRMN");
//            testInit.Add("1;S1;P1;10;1");
//            testInit.Add("2;S2;P2;12;2");
//            testInit.Add("3;S2;P1;12;1");
//            LoadTestData(masterInit, testInit);
//            ComparisonProcessor compProc = new ComparisonProcessor(pc);
//            //expect
//            List<string> expectedCompared = new List<string>();
//            expectedCompared.Add("3 | 1;0;0;0;0");
//            expectedCompared.Add("1 | 2;0;0;0;0");
//            expectedCompared.Add("2 | 3;0;0;0;0");
//            List<string> expectedExtra = new List<string>();
//            //actual
//            var actualResult = compProc.Execute(Master, Test);
//            var actualCompared = actualResult.Data.Select(r => string.Join(";", r.Data).TrimEnd(';')).ToList();
//            var actualExtra = actualResult.Extra;
//            //compared
//            Assert.AreEqual(expectedCompared.Count, actualCompared.Count);
//            CollectionAssert.AreEqual(expectedCompared, actualCompared);
//            //extra
//            Assert.AreEqual(expectedExtra.Count, actualExtra.Count);
//            CollectionAssert.AreEqual(expectedExtra, actualExtra);
//        }



//    }
//}
