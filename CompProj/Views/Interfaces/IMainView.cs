using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Views.Interfaces {
    public interface IMainView {
        event EventHandler OpenFileEvent;
        string GetFilePath(string fileVersion);
        void WriteMessage(string message);
        void Show();
    }
}
