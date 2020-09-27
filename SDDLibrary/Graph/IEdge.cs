using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph
{
    public interface IEdge<TVertex>
    {
        TVertex Source { get; }
        TVertex Target { get; }
        string Value { get; set; }
    }
}
