using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models.Interfaces {
    public interface IImpConfig {
        string PathMasterFile { get; set; }
        string PathTestFile { get; set; }
        char Delimiter { get; set; }
        int RowsToSkip { get; set; }
        int HeadersRow { get; set; }
        Encoding Encoding { get; set; }
        int BufferSize { get; set; }
    }
}
