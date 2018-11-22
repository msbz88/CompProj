using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompProj.Models;

namespace CompProj.Presenters {
    public class FileReaderPresenter {
        private IFileReader FileReader {get;set;}

        public FileReaderPresenter(IFileReader fileReader) {
            FileReader = fileReader;
        }

        public void ReadFromFiles() {

        }
    }
}
