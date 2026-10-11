using System.Collections.Generic;

namespace WpfApp
{
    // Contract for converting a numeric dataset into CSV text.
    public interface ICsvWriter
    {
        string Write(List<double> numbers, bool includeHeader);
    }
}
