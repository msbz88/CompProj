using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public interface IFileReader {
        Task<List<string>> ReadAllLinesAsync(string path, Encoding encoding, int bufferSize, int numRowsToSkip);
    }
}
