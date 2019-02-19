using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Views.Interfaces {
    public interface IImpConfigView {
        EventHandler FileLoadEvent { get; set; }
        string Delimiter { get; set; }
        string HeadersRow { get; set; }
        void ShowError(string error);
        void ShowView();
        void Close();
        void DisplayFilePreview(List<string> content);
        string GetFilePath(string fileVersion);
    }
}
