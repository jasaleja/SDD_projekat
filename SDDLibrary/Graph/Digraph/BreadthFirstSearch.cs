using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph.Digraph
{
    public class BreadthFirstSearch<TVertex, TEdge> : IGraphAlgorithms<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private Dictionary<TVertex, VertexColor> vertexColors;

        public Dictionary<TVertex, VertexColor> VertexColors
        {
            get { return vertexColors; }
        }

        #region Events
        /// <summary>
        /// Aktivnost tokom prvog nailaska na nod (bojenje u sivo).
        /// </summary>
        public event Action<TVertex> DiscoverVertex;

        /// <summary>
        /// Kod BFS pretrage interesantna je aktivnost kod izbora noda za sledeci koren pretrage.
        /// </summary>
        public event Action<TVertex> ExamineVertex;

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

        protected virtual void OnExamineVertex(TVertex node)
        {
            if (ExamineVertex != null)
                ExamineVertex(node);
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

        public BreadthFirstSearch()
        {
            vertexColors = new Dictionary<TVertex, VertexColor>();
        }

        public void Search(IDirectedGraph<TVertex, TEdge> graph, params TVertex[] roots)
        {
            Search(graph, (IEnumerable<TVertex>)roots);
        }

        public void Search(IDirectedGraph<TVertex, TEdge> graph, IEnumerable<TVertex> roots = null)
        {
            Initialize(graph);            

            if(roots == null || roots.Count() == 0)
                roots = graph.Vertices.Where(v => graph.InEdges(v).Count() == 0);

            foreach(var root in roots)
            {
                vertexColors[root] = VertexColor.Gray;
                OnDiscoverVertex(root);                
            }

            Queue<TVertex> queue = new Queue<TVertex>(roots);

            while (queue.Count > 0)
            {
                TVertex u = queue.Dequeue();
                OnExamineVertex(u);

                foreach (TEdge e in graph.OutEdges(u))
                {
                    TVertex v = e.Target;
                    OnExamineEdge(e);

                    if (vertexColors[v] == VertexColor.White)
                    {
                        vertexColors[v] = VertexColor.Gray;
                        OnDiscoverVertex(v);
                        queue.Enqueue(v);
                    }
                }

                vertexColors[u] = VertexColor.Black;
                OnFinishVertex(u);
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
