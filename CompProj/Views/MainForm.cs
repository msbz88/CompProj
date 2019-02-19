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
        }

        private void ButtonOpenFilesClick(object sender, EventArgs e) {
            OpenFileEvent?.Invoke(sender, e);
        }       

        public void WriteMessage(string messages) {
            richTextBoxMessages.AppendText(messages + Environment.NewLine);
        }
    }
}
