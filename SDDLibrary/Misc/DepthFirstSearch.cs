using SDDLibrary.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Misc
{
    public class DepthFirstSearch<TVertex, TEdge> : IGraphAlgorithms<TVertex, TEdge>
         where TEdge : IEdge<TVertex>
    {
        private Dictionary<TVertex, VertexColor> vertexColors;

        public Dictionary<TVertex, VertexColor> VertexColors
        {
            get { return vertexColors; }
        }

        #region Internal types
        internal struct SearchFrame
        {
            public readonly TVertex Vertex;
            public readonly IEnumerator<TEdge> Edges;

            public SearchFrame(TVertex vertex, IEnumerator<TEdge> edges)
            {
                Vertex = vertex;
                Edges = edges;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Aktivnost tokom prvog nailaska na nod (bojenje u sivo).
        /// </summary>
        public event Action<TVertex> DiscoverVertex;

        /// <summary>
        /// Aktivnost tokom analiziranja grane grafa.
        /// </summary>
        public event Action<TEdge> ExamineEdge;

        /// <summary>
        /// Aktivnost koju vrsimo nakon sto smo analizirali nod (bojenje u crno)
        /// i pripremili sve njegove otocne nod-ove za dalju analizu.
        /// </summary>
        public event Action<TVertex> FinishVertex;

        protected virtual void OnDiscoverVertex(TVertex node)
        {
            if (DiscoverVertex != null)
                DiscoverVertex(node);
        }

        protected virtual void OnExamineEdge(TEdge edge)
        {
            if (ExamineEdge != null)
                ExamineEdge(edge);
        }

        protected virtual void OnFinishVertex(TVertex node)
        {
            if (FinishVertex != null)
                FinishVertex(node);
        }
        #endregion

        public DepthFirstSearch()
        {
            vertexColors = new Dictionary<TVertex, VertexColor>();
        }

        public void Search(IDirectedGraph<TVertex, TEdge> graph, params TVertex[] roots)
        {
            Search(graph, (IEnumerable<TVertex>)roots);
        }

        public void Search(IDirectedGraph<TVertex, TEdge> graph, IEnumerable<TVertex> roots)
        {
            Initialize(graph);
            Stack<TVertex> stack = new Stack<TVertex>();

            if (roots == null || roots.Count() == 0)
                roots = graph.Vertices.Where(v => graph.InEdges(v).Count() == 0);

            foreach (var root in roots)
            {
                stack.Push(root);

                while (stack.Count > 0)
                {
                    TVertex current = stack.Peek();
                    vertexColors[current] = VertexColor.Gray;
                    OnDiscoverVertex(current);

                    foreach (TEdge e in graph.OutEdges(current))
                    {
                        OnExamineEdge(e);
                        TVertex v = e.Target;

                        if (vertexColors[v] == VertexColor.White)
                            stack.Push(v);
                    }

                    //Zavrsavamo sa nod-om, posto je moguce da postoji vise instanci istog noda
                    //koje nisu beli nodovi potrebno ih je izbaciti sa steka.
                    while (stack.Count > 0 && vertexColors[stack.Peek()] != VertexColor.White)
                    {
                        current = stack.Pop();
                        if (vertexColors[current] == VertexColor.Gray)
                        {
                            vertexColors[current] = VertexColor.Black;
                            OnFinishVertex(current);
                        }
                    }
                }
            }
        }

        public void Search2(IDirectedGraph<TVertex, TEdge> graph, IEnumerable<TVertex> roots)
        {
            Initialize(graph);
            Stack<SearchFrame> searchStack = new Stack<SearchFrame>();

            if (roots == null || roots.Count() == 0)
                roots = graph.Vertices.Where(v => graph.InEdges(v).Count() == 0);

            foreach (var root in roots)
            {
                vertexColors[root] = VertexColor.Gray;
                OnDiscoverVertex(root);
                searchStack.Push(new SearchFrame(root, graph.OutEdges(root).GetEnumerator()));

                while (searchStack.Count > 0)
                {
                    var currentFrame = searchStack.Pop();
                    var u = currentFrame.Vertex;
                    var edges = currentFrame.Edges;

                    while (edges.MoveNext())
                    {
                        TEdge e = edges.Current;
                        OnExamineEdge(e);
                        TVertex v = e.Target;

                        if (vertexColors[v] == VertexColor.White)
                        {
                            searchStack.Push(new SearchFrame(u, edges));
                            u = v;
                            vertexColors[u] = VertexColor.Gray;
                            OnDiscoverVertex(u);
                            edges = graph.OutEdges(u).GetEnumerator();
                        }
                    }

                    if (edges != null)
                        edges.Dispose();

                    vertexColors[u] = VertexColor.Black;
                    OnFinishVertex(u);
                }
            }
        }

        protected void Initialize(IGraph<TVertex, TEdge> graph)
        {
            vertexColors.Clear();
            foreach (TVertex u in graph.Vertices)
            {
                vertexColors.Add(u, VertexColor.White);
            }
        }
    }
}
