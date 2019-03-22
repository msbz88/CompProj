using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CompProj.Views.Interfaces;

namespace CompProj.Views {
    public partial class FileForm : Form, IFileView {
        public EventHandler FileLoadEvent { get; set; }
        public string InfoBox {
            get { return richTextBoxInfo.Text; }
            set {
                InitializeInfoBox();
                richTextBoxInfo.BackColor = Color.LightGray;
                richTextBoxInfo.Text = value.ToString();
                richTextBoxInfo.Select(0, richTextBoxInfo.Text.IndexOf('\n'));
                richTextBoxInfo.SelectionFont = new Font(richTextBoxInfo.Font, FontStyle.Bold);
                richTextBoxInfo.ScrollBars = RichTextBoxScrollBars.Vertical;
            }
        }
        public string Delimiter { get { return textBoxDelimiter.Text; } set { textBoxDelimiter.Text = value.ToString(); } }
        public string HeadersRow { get { return textBoxHeadersRow.Text; } set { textBoxHeadersRow.Text = value.ToString(); } }

        public FileForm() {
            InitializeComponent();
        }

        public void ShowView() {
            ShowDialog();
        }

        private void InitializeInfoBox() {
            listViewFileContent.Location = new Point(3, 72);
            listViewFileContent.Height = 270;
            richTextBoxInfo.Width = 827;
            richTextBoxInfo.Height = 59;
        }

        public void DisplayFilePreview(List<string> headers, List<string[]> fileContent) {
            listViewFileContent.Clear();              
            headers.ForEach(name => listViewFileContent.Columns.Add(name));
            listViewFileContent.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ListViewItem[] listItems = fileContent.Select(line => new ListViewItem(line)).ToArray();      
            listViewFileContent.Items.AddRange(listItems);
        }

        public string GetFilePath(string fileVersion) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                // InitialDirectory = @"D:\",
                openFileDialog.Title = "Select " + fileVersion + " file";
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;          
            if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                    return openFileDialog.FileName;
                }
                return string.Empty;
            }
        }

        public void ShowError(string error) {
            MessageBox.Show(error);
        }

        private void ButtonLoadClick(object sender, EventArgs e) {
            FileLoadEvent?.Invoke(sender, e);
        }
    }
}
