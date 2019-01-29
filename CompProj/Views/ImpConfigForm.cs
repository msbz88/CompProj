using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CompProj.Views.Interfaces;

namespace CompProj.Views {
    public partial class ImpConfigForm : Form, IImpConfigView {
        public event EventHandler LoadEvent;

        public string Delimiter { get { return textBoxDelimiter.Text; } set { textBoxDelimiter.Text = value.ToString(); } }
        public string HeadersRow { get { return textBoxHeadersRow.Text; } set { textBoxHeadersRow.Text = value.ToString(); } }

        public ImpConfigForm() {
            InitializeComponent();
        }

        public void ShowView() {
            ShowDialog();
        }

        private void ButtonLoadClick(object sender, EventArgs e) {
            LoadEvent?.Invoke(sender, e);
        }

        public void DisplayFilePreview(List<string> fileContent) {
            foreach (var line in fileContent) {
                richTextBoxFileContent.AppendText(line + Environment.NewLine);
            } 
        }

        public string GetFilePath(string fileVersion) {
            OpenFileDialog openFileDialog = new OpenFileDialog {
               // InitialDirectory = @"D:\",
                Title = "Select " + fileVersion + " file",
                CheckFileExists = true,
                CheckPathExists = true,
                FilterIndex = 2,
                RestoreDirectory = true,
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                return openFileDialog.FileName;
            }
            return String.Empty;
        }

        public void ShowError(string error) {
            MessageBox.Show(error);
        }

    }
}
