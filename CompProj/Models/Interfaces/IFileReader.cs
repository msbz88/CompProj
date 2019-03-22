using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public interface IFileReader {
        IEnumerable<string> ReadFile(string filePath, int skipRecords, Encoding encoding);
        IEnumerable<string> ReadFewLines(string filePath, int rowsToTake);
    }
}
