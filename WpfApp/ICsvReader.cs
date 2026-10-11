using System.Collections.Generic;

namespace WpfApp
{
    // Contract for converting CSV text into a numeric dataset.
    public interface ICsvReader
    {
        List<double> Read(string csvText, bool hasHeader);
    }
}
