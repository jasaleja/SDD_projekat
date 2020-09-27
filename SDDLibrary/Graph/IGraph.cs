using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph
{
    public delegate bool VertexEquality<TVertex>(TVertex v1, TVertex v2);

    public interface IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        IEqualityComparer<TVertex> VertexComparer { get; }
        bool IsDirected { get; }
        int VertexCount { get; }
        int EdgeCount { get; }
        IEnumerable<TEdge> Edges { get; }
        IEnumerable<TVertex> Vertices { get; }

        TVertex MateOf(TVertex v, TEdge e);
        bool AddEdge(TEdge e);
        bool AddVertex(TVertex v);
        bool AddVerticesAndEdge(TEdge e);
        bool RemoveEdge(TEdge e);
        bool RemoveVertex(TVertex v);
        bool ContainsEdge(TEdge e);
        bool ContainsEdge(TVertex source, TVertex target);
        bool ContainsVertex(TVertex v);
        bool TryGetEdge(TVertex source, TVertex target, out TEdge edge);        
    }

    public interface IUndirectedGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        IEnumerable<TEdge> AdjacentEdges(TVertex v);
        IEnumerable<TVertex> AdjacentVertices(TVertex v);
    }

    public interface IDirectedGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        IEnumerable<TEdge> InEdges(TVertex v);
        IEnumerable<TEdge> OutEdges(TVertex v);        
    }
}
