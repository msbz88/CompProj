using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public interface IFileReader {
        List<string> ReadLines(string filePath, int rowsToTake);
        Task<List<string>> ReadAllLinesAsync(string filePath, Encoding encoding, int bufferSize, int headersRow);
    }
}
