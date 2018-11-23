using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models.Interfaces;

namespace CompProj.Models {
    public class ImpConfig: IImpConfig {
        public string PathMasterFile { get; set; }
        public string PathTestFile { get; set; }
        public char Delimiter { get; set; }
        public int RowsToSkip { get; set; }
        public int HeadersRow { get; set; }
        public Encoding Encoding { get; set; }
        public int BufferSize { get; set; }

        public ImpConfig(string pathMasterFile, string pathTestFile, char delimiter, int rowsToSkip, int headersRow, Encoding encoding, int bufferSize) {
            PathMasterFile = pathMasterFile;
            PathTestFile = pathTestFile;
            Delimiter = delimiter;
            RowsToSkip = rowsToSkip;
            HeadersRow = headersRow;
            Encoding = encoding;
            BufferSize = bufferSize;
        }
    }
}
