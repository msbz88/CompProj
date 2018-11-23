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
        IImpConfig ImportConfiguration { get; set; }
        IFileReader FileReader { get; set; }

        public MainPresenter(IMainView mainView, ImpConfigPresenter impConfigPresenter, IFileReader fileReader) {
            MainView = mainView;
            ImpConfigPresenter = impConfigPresenter;
            FileReader = fileReader;
            MainView.Show();
            MainView.OpenFileEvent += OnFileOpen;
        }

        private void OnFileOpen(object sender, EventArgs e) {
            ImpConfigPresenter.SetImportConfiguration();
            MainView.WriteMessage("Master file: " + ImportConfiguration.PathMasterFile);
            MainView.WriteMessage("Test file: " + ImportConfiguration.PathTestFile);
            ImpConfigPresenter.StartImportEvent += OnStartImport;
        }

        private void OnStartImport(object sender, EventArgs e) {
            ComparisonHelper ComparisonHelper = new ComparisonHelper(FileReader, ImportConfiguration);
            ComparisonHelper.PrepareComparison();
        }

    }
}
