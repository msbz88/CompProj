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
    public partial class FilePreviewForm : Form, IFilePreviewView {
        public event EventHandler LoadEvent;
        public event EventHandler ShowViewEvent;

        public string Delimiter { get { return textBoxDelimiter.Text; } set { textBoxDelimiter.Text = value.ToString(); } }
        public string SkipRows { get { return textBoxSkipRows.Text; } set { textBoxSkipRows.Text = value.ToString(); } }
        public string HeadersRow { get { return textBoxHeadersRow.Text; } set { textBoxHeadersRow.Text = value.ToString(); } }

        public FilePreviewForm() {
            InitializeComponent();
        }

        public void ShowView() {
            ShowDialog();
            ShowViewEvent?.Invoke(this, null);
        }

        private void ButtonLoadClick(object sender, EventArgs e) {
            LoadEvent?.Invoke(sender, e);
        }

        public void PrintFileContent(List<string> content) {
            foreach (var line in content) {
                richTextBoxFileContent.AppendText(line + Environment.NewLine);
            } 
        }
    }
}
