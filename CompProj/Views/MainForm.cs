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
using CompProj.Presenters;
using CompProj.Views.Interfaces;

namespace CompProj {
    public partial class MainForm : Form, IMainView {
        public EventHandler OpenFileEvent { get; set; }

        public MainForm() {
            InitializeComponent();
            listViewInfo.Columns.Add("", 285);
            listViewInfo.Columns.Add("", 285);
        }

        private void ButtonOpenFilesClick(object sender, EventArgs e) {
            OpenFileEvent?.Invoke(sender, e);
        }       

        public void WriteMessage(string header, string message) {
            ListViewItem listItem = new ListViewItem(new[] { header, message });
            listViewInfo.Items.Add(listItem);
            //listViewInfo.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

    }
}
