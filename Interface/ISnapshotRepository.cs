using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interface
{
    public interface ISnapshotRepository
    {
        string RecognizeValueWithTesseract(string filePath);
        string RecognizeSymbol(string filePath, string type);
        
    }
}