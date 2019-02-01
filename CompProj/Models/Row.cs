using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompProj {
    public class Row {
        public int Id { get; set; }
        public List<string> Columns { get; set; }
        public char Delimiter { get; set; }

        public Row(int id, string data, char delimiter) {
            Id = id;
            Delimiter = delimiter;
            Columns = data.Split(Delimiter).ToList();
        }

        public IEnumerable<string> GetMultipleColumnsByIndex(List<int> positions) {
            return Columns.Select((f, i) => new { f, i })
                .Where(x => positions.Contains(x.i))
                .Select(x => x.f);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(Delimiter);
            foreach (var item in Columns) {
                sb.Append(item);
                sb.Append(Delimiter);
            }
            return sb.ToString().TrimEnd(Delimiter);
        }

    }
}
