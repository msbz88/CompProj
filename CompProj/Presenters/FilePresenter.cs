using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Models.Interfaces;
using CompProj.Views.Interfaces;

namespace CompProj.Presenters {
    public class FilePresenter {
        public EventHandler StartImportEvent { get; set; }
        IFileReader FileReader { get; set; }
        IFileView FileView { get; set; }
        string Delimiter { get; set; }
        int HeadersRow { get; set; }
        public string PathMasterFile;
        public string PathTestFile;
        List<string[]> MasterPreviewContent { get; set; }
        List<string[]> TestPreviewContent { get; set; }

        public FilePresenter(IFileView impConfigView, IFileReader fileReader) {
            FileView = impConfigView;
            FileView.FileLoadEvent += OnFileLoad;
            FileReader = fileReader;
            MasterPreviewContent = new List<string[]>();
            TestPreviewContent = new List<string[]>();
        }

        public void Run() {
            PathMasterFile = FileView.GetFilePath("Master");
            if (!string.IsNullOrEmpty(PathMasterFile)) {
                PathTestFile = FileView.GetFilePath("Test");
                if (!string.IsNullOrEmpty(PathTestFile)) {                 
                    ShowPreview(65);
                    FileView.ShowView();
                }
            }
        }

        private void ShowPreview(int rowsToShow) {
            var mData = FileReader.ReadFewLines(PathMasterFile, rowsToShow);
            var tData = FileReader.ReadFewLines(PathTestFile, rowsToShow);
            var delimiter = FindDelimiter(mData);
            if (!string.IsNullOrEmpty(delimiter)) {
                Delimiter = delimiter;
                FileView.Delimiter = Delimiter == "\t" ? "\\t": Delimiter;
            } else {
                Delimiter = FileView.Delimiter;              
            }
            HeadersRow = FindDataBeginning(mData);
            FileView.HeadersRow = HeadersRow.ToString();
            if (!LoadPreview(mData.ToList(), tData.ToList())) {
                FileView.ShowError("Error: Files have different number of columns.");
                FileView.BlockLoad();
            }
            var headers = MasterPreviewContent.ElementAt(HeadersRow).ToList();
            if (HeadersRow > 0) {
                WriteInfoMessage("Automatically skipped rows:", mData.Take(HeadersRow).ToArray());
            }
            FileView.CurrentFileName = Path.GetFileName(PathMasterFile);
            FileView.DisplayFilePreview(headers, MasterPreviewContent.Skip(HeadersRow + 1).ToList());
        }

        private bool LoadPreview(List<string> mData, List<string> tData) {
            bool res = false;
            var rowsToShow = mData.Count > tData.Count ? mData.Count : tData.Count;
            for (int i = 0; i < rowsToShow; i++) {
                var mLine = mData[i].Split(new string[] { Delimiter }, StringSplitOptions.None);
                var tLine = tData[i].Split(new string[] { Delimiter }, StringSplitOptions.None);

                MasterPreviewContent.Add(mLine);
                TestPreviewContent.Add(tLine);

                if (mLine.Length == tLine.Length) {
                    res = true;
                } else {
                    res = false;
                }
            }
            return res;
        }

        private void WriteInfoMessage(string msg, string[] data) {
            StringBuilder sb = new StringBuilder();
            sb.Append(msg);
            foreach (var item in data) {
                sb.Append('\n');
                sb.Append(item);             
            }
            FileView.InfoBox = sb.ToString();
        }

        private string FindDelimiter(IEnumerable<string> fileContent) {
            List<char> delimiters = new List<char> { '\t', ';', ','};
            Dictionary<char, int> counts = delimiters.ToDictionary(key => key, value => 0);
            foreach (char c in delimiters) {
                counts[c] = fileContent.Sum(item=>item.Where(ch=>ch==c).Count());
            }
            var maxRepeated = counts.Max(item=>item.Value);
            return counts.Where(item=>item.Value==maxRepeated).Select(item=>item.Key).FirstOrDefault().ToString();
        }

        private bool IsCorrectDelimiter(IEnumerable<string> fileContent, string delimiter) {
            int ColumnsCount = 0;
            int ColumnsCountPrev = 0;
            foreach (var item in fileContent) {
                ColumnsCount = item.Split(new string[] { delimiter }, StringSplitOptions.None).Length;
                if (ColumnsCount != ColumnsCountPrev && (ColumnsCount != 0 && ColumnsCountPrev != 0)) {
                    return false;
                }
                ColumnsCountPrev = ColumnsCount;      
            }
            return true;
        }

        private int FindDataBeginning(IEnumerable<string> fileContent) {
            var data = fileContent.Select(line => line.Split(new string[] { Delimiter }, StringSplitOptions.None).Length);
            var max = data.Max();
            return data.ToList().IndexOf(max);
        }

        private FileConfiguration SetImportConfiguration() {
            FileConfiguration fileConfiguration = new FileConfiguration(
                pathMasterFile: PathMasterFile,
                pathTestFile: PathTestFile,
                delimiter: Delimiter,
                rowsToSkip: int.Parse(FileView.HeadersRow),
                isHeadersExist: true,
                encoding: Encoding.ASCII
                );
            return fileConfiguration;
        }

        public void OnFileLoad(object sender, EventArgs e) {
            FileConfiguration fileConfiguration = SetImportConfiguration();
            IList<ValidationResult> errors = Validate(fileConfiguration);
            if (errors.Any()) {
                FileView.ShowError(CreateErrorString(errors));
            } else {
                FileView.FileLoadEvent -= OnFileLoad;
                StartImportEvent?.Invoke(fileConfiguration, e);
                FileView.Close();
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

        private IList<ValidationResult> Validate(FileConfiguration impConfig) {
            ValidationContext context = new ValidationContext(impConfig, null, null);
            IList<ValidationResult> errors = new List<ValidationResult>();
            Validator.TryValidateObject(impConfig, context, errors, true);
            return errors;
        }
    }
}
