using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CompProj.Presenters;
using CompProj.Views;
using CompProj.Views.Interfaces;
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

            IMainView mainView = new MainForm();
            IFileView fileView = new FileForm();
            MainPresenter mainPresenter = new MainPresenter(mainView, fileView);

            Application.Run();
        }
    }
}
