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
            IImpConfigView impConfigView = new ImpConfigForm();

            IFileReader fileReader = new FileReader();

            ImpConfigPresenter impConfigPresenter = new ImpConfigPresenter(impConfigView, fileReader);
            MainPresenter mainPresenter = new MainPresenter(mainView, impConfigPresenter, fileReader);

            Application.Run();


        }
    }
}
