using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Models.Interfaces;
using CompProj.Views.Interfaces;

namespace CompProj.Presenters {
    public class ImpConfigPresenter {
        public event EventHandler StartImportEvent;
        IFileReader FileReader { get; set; }
        IImpConfigView ImpConfigView { get; set; }

        public ImpConfigPresenter(IImpConfigView impConfigView, IFileReader fileReader) {
            ImpConfigView = impConfigView;
            FileReader = fileReader;
            ImpConfigView.LoadEvent += OnLoad;
        }

        public void SetImportConfiguration() {
            string pathMasterFile = ImpConfigView.GetFilePath("Master");
            string pathTestFile = ImpConfigView.GetFilePath("Test");
            
            List<string> previewContent = FileReader.ReadLines(pathMasterFile, 50);
            ImpConfigView.DisplayFilePreview(previewContent);
            ImpConfigView.ShowView();
            /*
            IImpConfig impConfig = new ImpConfig(
                pathMasterFile: pathMasterFile,
                pathTestFile: pathTestFile,
                delimiter: ImpConfigView.Delimiter,
                rowsToSkip: ImpConfigView.RowsToSkip,
                headersRow: ImpConfigView.HeadersRow,
                encoding: Encoding.ASCII,
                bufferSize: 5000
                );       

            return impConfig;*/
        }

        public void OnLoad(object sender, EventArgs e) {

            ImpConfigView.Close();
            StartImportEvent?.Invoke(sender, e);
        }
    }
}
