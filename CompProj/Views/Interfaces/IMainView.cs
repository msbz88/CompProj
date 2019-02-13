using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompProj.Views.Interfaces {
    public interface IMainView {
        event EventHandler OpenFileEvent;
        void WriteMessage(string message);
        void Show();
        event FormClosingEventHandler FormClosing;
        void Dispose();
    }
}
