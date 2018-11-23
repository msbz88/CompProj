using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Views.Interfaces {
    public interface IImpConfigView {
        event EventHandler LoadEvent;
        char Delimiter { get; set; }
        int RowsToSkip { get; set; }
        int HeadersRow { get; set; }
        void ShowView();
        void Close();
        void DisplayFilePreview(List<string> content);
        string GetFilePath(string fileVersion);
    }
}
