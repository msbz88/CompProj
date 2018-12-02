﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Views.Interfaces {
    public interface IImpConfigView {
        event EventHandler LoadEvent;
        string Delimiter { get; set; }
        string RowsToSkip { get; set; }
        string HeadersRow { get; set; }
        void ShowError(string error);
        void ShowView();
        void Close();
        void DisplayFilePreview(List<string> content);
        string GetFilePath(string fileVersion);
    }
}
