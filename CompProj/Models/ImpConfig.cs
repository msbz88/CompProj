using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CompProj.Models {
    public class ImpConfig: IImpConfig {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path to Master file is required")]
        public string PathMasterFile { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path to Test file is required")]
        public string PathTestFile { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Delimiter is required")]
        public char Delimiter { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path to Master file is required")]
        public int HeadersRow { get; set; }
        public bool IsHeadersExist { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "HeadersRow is required")]
        public Encoding Encoding { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "BufferSize is required")]
        public int BufferSize { get; set; }

        public ImpConfig(string pathMasterFile, string pathTestFile, char delimiter, int headersRow, Encoding encoding, int bufferSize) {
            PathMasterFile = pathMasterFile;
            PathTestFile = pathTestFile;
            Delimiter = delimiter;
            HeadersRow = headersRow;
            IsHeadersExist = headersRow >= 0 ? true: false;
            Encoding = encoding;
            BufferSize = bufferSize;
        }
    }
}
