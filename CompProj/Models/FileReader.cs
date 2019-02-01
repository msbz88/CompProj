using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompProj.Models {
    public class FileReader: IFileReader {
        public async Task<List<string>> ReadAllLinesAsync(string filePath, Encoding encoding, int bufferSize, int headersRow) {
            return await Task.FromResult(File.ReadAllLines(filePath).ToList());        
        }

        public List<string> ReadLines (string filePath, int rowsToTake) {
            return File.ReadAllLines(filePath).Take(rowsToTake).ToList();
        }
    }
}
