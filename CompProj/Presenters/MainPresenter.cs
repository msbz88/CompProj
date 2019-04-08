using CompProj.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Models.Interfaces;
using System.IO;

namespace CompProj.Presenters {
    public class MainPresenter {
        IMainView MainView { get; set; }
        IFileView FileView { get; set; }
        IFileReader FileReader { get; set; }
        FilePresenter FilePresenter { get; set; }

        public MainPresenter(IMainView mainView, IFileView fileView) {          
            MainView = mainView;
            FileView = fileView;
            MainView.OpenFileEvent += OnOpenFile;
            MainView.Show();           
        }

        public void OnOpenFile(object sender, EventArgs e) {
            FileReader = new FileReader();
            FilePresenter = new FilePresenter(FileView, FileReader);
            FilePresenter.StartImportEvent += async (s, ev) => await OnStartImport((IFileConfiguration)s);
            FilePresenter.Run();
        }

        private async Task OnStartImport(IFileConfiguration fileConfiguration) {
            MainView.WriteMessage("Master file", Path.GetFileName(FilePresenter.PathMasterFile));
            MainView.WriteMessage("Test file", Path.GetFileName(FilePresenter.PathTestFile));
            ComparisonHelper ComparisonHelper = new ComparisonHelper(FileReader, fileConfiguration);
            try {
                await Task.Run(() => ComparisonHelper.PrepareForComparison());
            } catch (Exception ex) {
                MainView.WriteMessage("Error", ex.Message);
                return;
            }
            MainView.WriteMessage("Master", ComparisonHelper.MasterRowsCount.ToString());
            MainView.WriteMessage("Test", ComparisonHelper.TestRowsCount.ToString());
            MainView.WriteMessage("Actual diff", ComparisonHelper.ActualRowsDiff.ToString());
            MainView.WriteMessage("Compared", ComparisonHelper.ComparedRowsCount.ToString());
            MainView.WriteMessage("Extra", ComparisonHelper.ExtraRowsCount.ToString());
        }
    }
}
