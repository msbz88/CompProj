using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompProj {
    public partial class Form1 : Form {
        string pathMasterFile = @"C:\Users\msbz\Desktop\Sharp_extracts\[SCD_IFACTIF_RESULTS_IFRS.csv";
        string pathTestFile = @"C:\Users\msbz\Desktop\Sharp_extracts\]SCD_IFACTIF_RESULTS_IFRS.csv";
        string pathCompareFile = @"C:\Users\msbz\Desktop\Sharp_extracts\Comparison.csv";
        List<string> Headers;

        public Form1() {
            InitializeComponent();
            Test(';');
        }

        public void Test(char delimiter) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long memory = GC.GetTotalMemory(true);
            richTextBox1.AppendText("Memory used before: " + ConvertBytesToMegabytes(memory).ToString() + "\n");

            List<string> initMaster = File.ReadAllLines(pathMasterFile).ToList();
            List<string> initTest = File.ReadAllLines(pathTestFile).ToList();

            Headers = initMaster[0].Split(';').ToList();

            List<string> uMaster = initMaster.Except(initTest).ToList();
            List<Row> tableMaster = new List<Row>();
            foreach (var item in uMaster) {
                Row row = new Row(item, delimiter);
                tableMaster.Add(row);
            }

            List<string> uTest = initTest.Except(initMaster).ToList();
            List<Row> tableTest = new List<Row>();
            foreach (var item in uTest) {
                Row row = new Row(item, delimiter);
                tableTest.Add(row);
            }

            AppendText(initMaster[0]);

            var query = from m in tableMaster
                        join t in tableTest on m.Cells[GetValue("SECURITY_ID")] equals t.Cells[GetValue("SECURITY_ID")]
                        select m.Compare(t);

            foreach (var item in query) {
                AppendText(item);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            richTextBox1.AppendText("Time: " + elapsedMs.ToString() + "\n");
            memory = GC.GetTotalMemory(true);
            richTextBox1.AppendText("Memory used after: " + ConvertBytesToMegabytes(memory).ToString().ToString());
        }

        public double ConvertBytesToMegabytes(long bytes) {
            return Math.Round((bytes / 1024f) / 1024f,2);
        }

        public void AppendText(string data) {
            using (StreamWriter sw = File.AppendText(pathCompareFile)) {
                sw.WriteLine(data);
            }
        }

        private int GetValue(string name) {
            return Headers.FindIndex(item=>item==name);
        }

    }
}
