using CompProj.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Models.Interfaces;

namespace CompProj.Presenters {
    public class MainPresenter {
        IMainView MainView { get; set; }
        ImpConfigPresenter ImpConfigPresenter { get; set; }

        public MainPresenter(IMainView mainView, ImpConfigPresenter impConfigPresenter) {
            MainView = mainView;
            ImpConfigPresenter = impConfigPresenter;
            ImpConfigPresenter.StartImportEvent += OnStartImport;
            MainView.OpenFileEvent += OnOpenFile;
            MainView.Show();
        }

        public void OnOpenFile(object sender, EventArgs e) {
            ImpConfigPresenter.Run();
        }

        private void OnStartImport(object sender, EventArgs e) {
            ImpConfigPresenter.StartImportEvent -= OnStartImport;
            IFileReader fileReader = new FileReader();
            ComparisonHelper ComparisonHelper = new ComparisonHelper(fileReader, (IImpConfig)sender);
            ComparisonHelper.PrepareComparison();
        }

    }
}
