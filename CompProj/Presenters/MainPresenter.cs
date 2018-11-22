using CompProj.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Presenters {
    class MainPresenter {
        private IMainView MainView { get; set; }
        private IFilePreviewView FilePreviewView { get; set; }
        public string PathMasterFile { get; set; }
        public string PathTestFile { get; set; }

        public MainPresenter(IMainView mainView, IFilePreviewView filePreviewView) {
            MainView = mainView;
            FilePreviewView = filePreviewView;
            MainView.Show();
            MainView.OpenFileEvent += OnFileOpen;
        }

        private void OnFileOpen(object sender, EventArgs e) {
            PathMasterFile = MainView.GetFilePath("Master");
            MainView.WriteMessage("Master file: " + PathMasterFile);
            PathTestFile = MainView.GetFilePath("Test");
            MainView.WriteMessage("Test file: " + PathTestFile);
            //change somehow to leave one responsibility for this presenter
            FilePreviewView.ShowView();
        }

    }
}
