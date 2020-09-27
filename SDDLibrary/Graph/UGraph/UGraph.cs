using SDDLibrary.GraphPlotting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph.UGraph
{
    public class UGraph<TVertex, TEdge> : IUndirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private IEqualityComparer<TVertex> vertexComparer;
        private Dictionary<TVertex, List<TEdge>> adjacentEdges;
        private int edgeCount;

        public IEqualityComparer<TVertex> VertexComparer
        {
            get { return vertexComparer; }
        }

        public UGraph(IEqualityComparer<TVertex> vertexComparer = null)
        {
            if (vertexComparer == null)
                this.vertexComparer = EqualityComparer<TVertex>.Default;
            else
                this.vertexComparer = vertexComparer;

            adjacentEdges = new Dictionary<TVertex, List<TEdge>>(vertexComparer);
            edgeCount = 0;
        }

        #region IGraph implementation
        public bool IsDirected
        {
            get
            {
                return false;
            }
        }

        public int VertexCount
        {
            get
            {
                return adjacentEdges.Count;
            }
        }

        public int EdgeCount
        {
            get
            {
                return edgeCount;
            }
        }

        public IEnumerable<TEdge> Edges
        {
            get
            {
                //For each edge there must be V1 vertex attached to that edge.
                foreach (var vertexAndEdges in adjacentEdges)
                {
                    foreach (var e in vertexAndEdges.Value)
                    {
                        if (vertexComparer.Equals(vertexAndEdges.Key, e.Source))
                            yield return e;
                    }
                }
            }
        }        

        public IEnumerable<TVertex> Vertices
        {
            get
            {
                return adjacentEdges.Keys;
            }
        }

        public TVertex MateOf(TVertex v, TEdge e)
        {
            if (vertexComparer.Equals(e.Source, v))
                return e.Target;
            else if (vertexComparer.Equals(e.Target, v))
                return e.Source;
            else
                throw new Exception("Edge doesn't contain supplied vertex.");
        }

        public virtual bool AddEdge(TEdge e)
        {
            if (e.Source == null || e.Target == null)
                throw new Exception("Edge must be connected to vertices on both ends.");

            if (ContainsEdge(e))
                return false;

            adjacentEdges[e.Source].Add(e);
            if (!vertexComparer.Equals(e.Source, e.Target))
                adjacentEdges[e.Target].Add(e);

            edgeCount++;
            return true;
        }

        public virtual bool AddVertex(TVertex v)
        {
            if (ContainsVertex(v))
                return false;

            adjacentEdges.Add(v, new List<TEdge>());
            return true;
        }
        
        public virtual bool AddVerticesAndEdge(TEdge e)
        {
            AddVertex(e.Source);
            AddVertex(e.Target);
            return AddEdge(e);
        }

        public virtual bool RemoveEdge(TEdge e)
        {
            bool removed = adjacentEdges[e.Source].Remove(e);

            if (removed)
            {
                edgeCount--;
                if (!vertexComparer.Equals(e.Source, e.Target))
                    adjacentEdges[e.Target].Remove(e);

                return true;
            }

            return false;
        }

        public virtual bool RemoveVertex(TVertex v)
        {
            if (!ContainsVertex(v))
                return false;

            var vertexEdges = adjacentEdges[v];
            edgeCount -= vertexEdges.Count;

            foreach (TEdge adjEdge in vertexEdges)
            {
                if (!vertexComparer.Equals(v, adjEdge.Source))
                    adjacentEdges[adjEdge.Source].Remove(adjEdge);
            }

            adjacentEdges.Remove(v);
            return true;
        }

        public bool ContainsEdge(TEdge e)
        {
            return ContainsEdge(e.Source, e.Target);
        }

        public bool ContainsEdge(TVertex source, TVertex target)
        {
            foreach(TEdge edge in adjacentEdges[source])
            {
                if (CompareEdge(edge, source, target))
                    return true;
            }
            return false;
        }

        public bool ContainsVertex(TVertex v)
        {
            return adjacentEdges.ContainsKey(v);
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge e)
        {
            foreach (TEdge edge in adjacentEdges[source])
            {
                if (CompareEdge(edge, source, target))
                {
                    e = edge;
                    return true;
                }
            }

            e = default(TEdge);
            return false;
        }

        private bool CompareEdge(TEdge e, TVertex source, TVertex target)
        {
            return vertexComparer.Equals(e.Source, source) ? vertexComparer.Equals(e.Target, target)
                : vertexComparer.Equals(e.Target, source) && vertexComparer.Equals(e.Source, target);
        }
        #endregion

        #region IUndirectedGraph implementation
        public IEnumerable<TEdge> AdjacentEdges(TVertex v)
        {
            return adjacentEdges[v];
        }

        public IEnumerable<TVertex> AdjacentVertices(TVertex v)
        {
            List<TEdge> adjEdges = adjacentEdges[v];
            return adjEdges.Select(e => vertexComparer.Equals(e.Source, v) ? e.Target : e.Source);
        }
        #endregion

        #region Visualization methods
        private GraphPlot<TVertex, TEdge> graphPlot;

        /// <summary>
        /// Za prikazivanje sadrzaja nod-a se poziva
        /// odgovarajuca implementacija metode ToString()
        /// u klasi V.
        /// </summary>
        public void PlotGraph(IDictionary<TVertex, VertexColor> colors = null, Func<TVertex, string> vertexDesc = null)
        {
            graphPlot = new GraphPlot<TVertex, TEdge>(this);
            graphPlot.Refresh(colors);
        }

        public void PlotRefresh(IDictionary<TVertex, VertexColor> colors = null)
        {
            if (graphPlot != null)
                graphPlot.Refresh(colors);
            else
                PlotGraph(colors);
        }
        #endregion
    }
}
