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
using CompProj.Views.Interfaces;

namespace CompProj {
    public partial class MainForm : Form, IMainView {
        public event EventHandler OpenFileEvent;       

        public MainForm() {
            InitializeComponent();
        }

        private void ButtonOpenFilesClick(object sender, EventArgs e) {
            OpenFileEvent?.Invoke(sender, e);
        }

        public string GetFilePath(string fileVersion) {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                InitialDirectory = @"D:\",
                Title = "Select " + fileVersion + " file",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                return openFileDialog.FileName;            
            }
            return String.Empty;
        }

        public void WriteMessage(string messages) {
            richTextBoxMessages.AppendText(messages + Environment.NewLine);
        }

    }
}
