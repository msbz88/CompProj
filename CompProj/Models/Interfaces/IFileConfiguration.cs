using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models.Interfaces {
    public interface IFileConfiguration {
        string PathMasterFile { get; set; }
        string PathTestFile { get; set; }
        string Delimiter { get; set; }
        int RowsToSkip { get; set; }
        bool IsHeadersExist { get; set; }
        Encoding Encoding { get; set; }
    }
}
