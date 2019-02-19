using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Models.Interfaces;
using CompProj.Views.Interfaces;

namespace CompProj.Presenters {
    public class ImpConfigPresenter {
        public EventHandler StartImportEvent { get; set; }
        IFileReader FileReader { get; set; }
        IImpConfigView ImpConfigView { get; set; }
        string PathMasterFile;
        string PathTestFile;

        public ImpConfigPresenter(IImpConfigView impConfigView, IFileReader fileReader) {
            ImpConfigView = impConfigView;
            ImpConfigView.FileLoadEvent += OnFileLoad;
            FileReader = fileReader;
        }

        public void Run() {
            PathMasterFile = ImpConfigView.GetFilePath("Master");
            PathTestFile = ImpConfigView.GetFilePath("Test");
            ShowPreview(PathMasterFile, 50);
            ImpConfigView.ShowView();
        }

        private void ShowPreview(string filePath, int rowsToShow) {
            List<string> previewContent = FileReader.ReadLines(filePath, rowsToShow);
            ImpConfigView.DisplayFilePreview(previewContent);
        }
        
        private ImpConfig SetImportConfiguration() {
            ImpConfig impConfig = new ImpConfig(
                pathMasterFile: PathMasterFile,
                pathTestFile: PathTestFile,
                delimiter: ImpConfigView.Delimiter == "t" ? '\t' : ImpConfigView.Delimiter[0],
                headersRow: int.Parse(ImpConfigView.HeadersRow),
                encoding: Encoding.ASCII,
                bufferSize: 10000
                );
                 return impConfig;
        }

        public void OnFileLoad(object sender, EventArgs e) {
            ImpConfig impConfig = SetImportConfiguration();
            IList<ValidationResult> errors = Validate(impConfig);
            if (errors.Any()) {
                 ImpConfigView.ShowError(CreateErrorString(errors));
            } else {
                ImpConfigView.FileLoadEvent -= OnFileLoad;
                ImpConfigView.Close();
                StartImportEvent?.Invoke(impConfig, e);             
            }
        }

        private string CreateErrorString(IList<ValidationResult> errors) {
            if (errors.Count == 1) {
                return errors[0].ErrorMessage;
            } else {
                StringBuilder errorBldr = new StringBuilder();
                foreach (var item in errors) {
                    errorBldr.Append(item.ErrorMessage);
                    errorBldr.Append(Environment.NewLine);
                }
                return errorBldr.ToString();
            }
        }

        private IList<ValidationResult> Validate(ImpConfig impConfig) {
            ValidationContext context = new ValidationContext(impConfig, null, null);
            IList<ValidationResult> errors = new List<ValidationResult>();
            Validator.TryValidateObject(impConfig, context, errors, true);
            return errors;
        }


    }
}
