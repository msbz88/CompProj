using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CompProj.Presenters;
using CompProj.Views;
using CompProj.Models;
using System.Text;

namespace CompProj {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();

            MainForm mainForm = new MainForm();
            FilePreviewForm filePreviewForm = new FilePreviewForm();

            MainPresenter mainPresenter = new MainPresenter(mainForm, filePreviewForm);
            FileReader fileReader = new FileReader(Encoding.ASCII, 5000);
            FilePreviewPresenter filePreviewPresenter = new FilePreviewPresenter(filePreviewForm, fileReader.PreviewFile(mainPresenter.PathMasterFile, 50));
         }
    }
}
