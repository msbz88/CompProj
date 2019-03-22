using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CompProj.Models {
    public class FileConfiguration: IFileConfiguration {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path to Master file is required")]
        public string PathMasterFile { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path to Test file is required")]
        public string PathTestFile { get; set; }
        public string Delimiter { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path to Master file is required")]
        public int RowsToSkip { get; set; }
        public bool IsHeadersExist { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "HeadersRow is required")]
        public Encoding Encoding { get; set; }

        public FileConfiguration(string pathMasterFile, string pathTestFile, string delimiter, int rowsToSkip, bool isHeadersExist, Encoding encoding) {
            PathMasterFile = pathMasterFile;
            PathTestFile = pathTestFile;
            Delimiter = delimiter;
            RowsToSkip = rowsToSkip;
            IsHeadersExist = isHeadersExist;
            Encoding = encoding;
        }
    }
}
