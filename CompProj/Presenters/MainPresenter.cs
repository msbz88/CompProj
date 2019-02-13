using CompProj.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Models.Interfaces;

namespace CompProj.Presenters {
    class MainPresenter {
        IMainView MainView { get; set; }
        ImpConfigPresenter ImpConfigPresenter { get; set; }

        public MainPresenter(IMainView mainView, ImpConfigPresenter impConfigPresenter) {
            MainView = mainView;
            ImpConfigPresenter = impConfigPresenter;
            MainView.Show();
            MainView.OpenFileEvent += OnOpenFile;
            MainView.FormClosing += OnFormClosing;
        }

        private void OnOpenFile(object sender, EventArgs e) {
            ImpConfigPresenter.StartImportEvent += OnStartImport;
            ImpConfigPresenter.Run();
        }

        private void OnStartImport(object sender, EventArgs e) {
            IFileReader fileReader = new FileReader();
            ComparisonHelper ComparisonHelper = new ComparisonHelper(fileReader, (IImpConfig)sender);
            ComparisonHelper.PrepareComparison();
        }

        private void OnFormClosing(object sender, EventArgs e) {
            MainView.OpenFileEvent -= OnOpenFile;
            ImpConfigPresenter.StartImportEvent -= OnStartImport;
            MainView.FormClosing -= OnFormClosing;
        }


    }
}
