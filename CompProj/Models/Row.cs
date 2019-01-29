using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompProj {
    public class Row {
        public List<string> Columns { get; set; }
        public char Delimiter { get; set; }

        public Row(string data, char delimiter) {
            Delimiter = delimiter;
            Columns = data.Split(Delimiter).ToList();
        }

        public IEnumerable<string> GetMultipleColumnsByIndex(List<int> positions) {
            return Columns.Select((f, i) => new { f, i })
                .Where(x => positions.Contains(x.i))
                .Select(x => x.f);
        }

    }
}
