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
        public string CurrentFileName { get { return labelFileName.Text; } set { labelFileName.Text = value.ToString(); } }

        public FileForm() {
            InitializeComponent();
            DrawBorderForLoadButton(Color.Green);
            labelFileName.ForeColor = Color.Gray;
        }

        private void DrawBorderForLoadButton(Color color) {
            Button bt = Controls.Find("buttonLoad", true)[0] as Button;
            bt.FlatStyle = FlatStyle.Flat;
            bt.FlatAppearance.BorderColor = color;
            bt.FlatAppearance.BorderSize = 1;
        }

        public void ShowView() {
            ShowDialog();
        }

        private void InitializeInfoBox() {
            listViewFileContent.Location = new Point(3, 88);
            listViewFileContent.Height = 245;
            richTextBoxInfo.Width = 827;
            richTextBoxInfo.Height = 59;
        }

        public void DisplayFilePreview(List<string> headers, List<string[]> fileContent) {
            int columnsStart = 0;
            int columnsEnd = 17;
            listViewFileContent.BeginUpdate();
            listViewFileContent.Clear();
            for (columnsStart = 0; columnsStart < columnsEnd; columnsStart++) {
                listViewFileContent.Columns.Add("[" + columnsStart + "] " + headers[columnsStart]);
            }        
            listViewFileContent.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            ListViewItem[] listItems = fileContent.Select(line => new ListViewItem(line.ToList().GetRange(columnsStart, columnsEnd).ToArray())).ToArray();      
            listViewFileContent.Items.AddRange(listItems);
            listViewFileContent.EndUpdate();
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

        public void BlockLoad() {
            DrawBorderForLoadButton(Color.Red);
            buttonLoad.Enabled = false;
        }
    }
}
