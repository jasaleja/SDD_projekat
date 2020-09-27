using SDDLibrary.GraphPlotting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDDLibrary.Graph.Digraph
{
    public class Digraph<TVertex, TEdge> : IDirectedGraph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private IEqualityComparer<TVertex> vertexComparer;
        private Dictionary<TVertex, List<TEdge>> vertexOutEdges;
        private Dictionary<TVertex, List<TEdge>> vertexInEdges;        
        private int edgeCount;        

        public IEqualityComparer<TVertex> VertexComparer
        {
            get { return vertexComparer; }
        }

        public Digraph(IEqualityComparer<TVertex> vertexComparer = null)
        {
            if (vertexComparer == null)
                this.vertexComparer = EqualityComparer<TVertex>.Default;
            else
                this.vertexComparer = vertexComparer;
            
            vertexInEdges = new Dictionary<TVertex, List<TEdge>>(vertexComparer);
            vertexOutEdges = new Dictionary<TVertex, List<TEdge>>(vertexComparer);
        }

        #region IGraph implementation
        public bool IsDirected
        {
            get
            {
                return true;
            }
        }

        public int VertexCount
        {
            get
            {
                return vertexOutEdges.Count;
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
                foreach (var edges in vertexOutEdges.Values)
                    foreach (var e in edges)
                        yield return e;
            }
        }        

        public IEnumerable<TVertex> Vertices
        {
            get
            {
                return vertexOutEdges.Keys;
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
            
            vertexOutEdges[e.Source].Add(e);
            vertexInEdges[e.Target].Add(e);
            edgeCount++;
            return true;
        }

        public virtual bool AddVertex(TVertex v)
        {
            if (ContainsVertex(v))
                return false;

            vertexOutEdges.Add(v, new List<TEdge>());
            vertexInEdges.Add(v, new List<TEdge>());
            return true;
        }

        public bool AddVerticesAndEdge(TEdge e)
        {
            AddVertex(e.Source);
            AddVertex(e.Target);
            return AddEdge(e);
        }

        public virtual bool RemoveEdge(TEdge e)
        {
            if (!vertexOutEdges[e.Source].Remove(e))
                return false;

            vertexInEdges[e.Target].Remove(e);
            edgeCount--;
            return true;
        }

        public virtual bool RemoveVertex(TVertex v)
        {
            if (!ContainsVertex(v))
                return false;
            
            foreach(var outEdge in vertexOutEdges[v])
            {
                vertexInEdges[outEdge.Target].Remove(outEdge);
                edgeCount--;
            }

            foreach(var inEdge in vertexInEdges[v])
            {
                if (vertexOutEdges[inEdge.Source].Remove(inEdge))
                {
                    edgeCount--;
                }
            }

            vertexOutEdges.Remove(v);
            vertexInEdges.Remove(v);
            return true;
        }

        public bool ContainsEdge(TEdge e)
        {
            return ContainsEdge(e.Source, e.Target);
        }

        public bool ContainsEdge(TVertex vSource, TVertex vTarget)
        {
            List<TEdge> outEdges;
            List<TEdge> inEdges;
            if (!vertexOutEdges.TryGetValue(vSource, out outEdges) ||
                !vertexInEdges.TryGetValue(vTarget, out inEdges))
                return false;

            return outEdges.Intersect(inEdges).Count() == 1;
        }

        public bool ContainsVertex(TVertex v)
        {
            return vertexOutEdges.ContainsKey(v);
        }

        public bool TryGetEdge(TVertex source, TVertex target, out TEdge edge)
        {
            List<TEdge> edgeList;
            if (vertexOutEdges.TryGetValue(source, out edgeList))
            {
                foreach (var e in edgeList)
                {
                    if (vertexComparer.Equals(e.Target, target))
                    {
                        edge = e;
                        return true;
                    }
                }
            }
            edge = default(TEdge);
            return false;
        }

        #endregion

        #region IDirectedGraph implementation
        public IEnumerable<TEdge> InEdges(TVertex v)
        {
            return vertexInEdges[v];
        }

        public IEnumerable<TEdge> OutEdges(TVertex v)
        {
            return vertexOutEdges[v];
        }

        #endregion

        #region Visualization methods
        private GraphPlot<TVertex, TEdge> graphPlot;

        /// <summary>
        /// Za prikazivanje sadrzaja nod-a se poziva
        /// odgovarajuca implementacija metode ToString()
        /// u klasi V.
        /// </summary>
        public void PlotGraph(IDictionary<TVertex, VertexColor> colors = null, Func<TVertex, string> vertexDesc=null,
            Func<TEdge, string> edgeDesc = null, params KeyValuePair<string,string>[] graphvizAttributes)
        {
            graphPlot = new GraphPlot<TVertex, TEdge>(this, vertexDesc, edgeDesc);

            foreach (var attr in graphvizAttributes)
                graphPlot.GraphForm.SetAttribute(attr);

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
