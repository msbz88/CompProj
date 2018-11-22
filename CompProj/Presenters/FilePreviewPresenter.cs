using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;
using CompProj.Views.Interfaces;

namespace CompProj.Presenters {
    public class FilePreviewPresenter {
        private IFilePreviewView FilePreviewView { get; set; }
        private List<string> PreviewContent { get; set;}

        public FilePreviewPresenter(IFilePreviewView fileReadView, List<string> previewContent) {
            FilePreviewView = fileReadView;
            PreviewContent = previewContent;
            FilePreviewView.ShowViewEvent += OnShowView;
        }

        public void OnShowView(object sender, EventArgs e) {
            FilePreviewView.PrintFileContent(PreviewContent);       
        }

    }
}
