using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph
{
    public interface IGraphAlgorithms<TVertex, TEdge>
    {
        Dictionary<TVertex, VertexColor> VertexColors { get; }
    }

    public enum VertexColor
    {
        White = 0,
        Gray,
        Black
    }
}
