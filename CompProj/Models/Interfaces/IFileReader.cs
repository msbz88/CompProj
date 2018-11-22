using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public interface IFileReader {
        Encoding Encoding { get; set; }
        int BufferSize { get; set; }
        List<string> PreviewFile(string filePath, int rowsToTake);
        Task<List<string>> ReadAllLinesAsync(string filePath, int rowsToSkip);
    }
}
