using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj {
    public class Row {
        public List<string> Cells { get; set; }
        public char Delimiter { get; set; }

        public Row(string data, char delimiter) {
            Delimiter = delimiter;
            Cells = data.Split(Delimiter).ToList();
        }

    }
}
