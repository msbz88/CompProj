using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Views.Interfaces {
    public interface IFilePreviewView {
        event EventHandler LoadEvent;
        event EventHandler ShowViewEvent;
        void ShowView();
        void PrintFileContent(List<string> content);
    }
}
