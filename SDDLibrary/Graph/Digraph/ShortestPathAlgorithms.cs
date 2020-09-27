using SDDLibrary.Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph.Digraph
{
    public class ShortestPathAlgorithms<TVertex, TEdge> : IGraphAlgorithms<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private PQMin<TVertex> minQueue;
        private TopologicalSort<TVertex, TEdge> topologicalSort;
        private Dictionary<TVertex, VertexColor> vertexColors;
        private Dictionary<TVertex, double> distances;

        public Dictionary<TVertex, VertexColor> VertexColors
        {
            get { return vertexColors; }
        }

        public Dictionary<TVertex, double> Distances
        {
            get { return distances; }
        }

        #region Event handlers
        /// <summary>
        /// Aktivnost tokom prvog nailaska na nod.
        /// </summary>
        public event Action<TVertex> ExamineVertex;

        /// <summary>
        /// Aktivnost prilikom osvezavanja parametara grane.
        /// </summary>
        public event Action<TEdge> EdgeRelaxed;

        /// <summary>
        /// Delegat za preuzimanje tezina grana u grafu.
        /// </summary>
        public Func<TEdge, double> EdgeWeights { get; private set; }
        
        protected virtual void OnExamineVertex(TVertex node)
        {
            if (ExamineVertex != null)
                ExamineVertex(node);
        }

        protected virtual void OnEdgeRelaxed(TEdge edge)
        {
            if (EdgeRelaxed != null)
                EdgeRelaxed(edge);
        }
        #endregion

        public ShortestPathAlgorithms(Func<TEdge, double> edgeWeights)
        {
            if (edgeWeights == null)
                throw new Exception("Delegate for extracting edge weights must be supplied.");

            EdgeWeights = edgeWeights;
            vertexColors = new Dictionary<TVertex, VertexColor>();
            minQueue = new PQMin<TVertex>(VertexWeightComparer);
            topologicalSort = new TopologicalSort<TVertex, TEdge>();
            distances = new Dictionary<TVertex, double>();
        }

        private int VertexWeightComparer(TVertex x, TVertex y)
        {
            return distances[x].CompareTo(distances[y]);
        }

        /// <summary>
        /// Efikasna implementacija algoritma za trazenje najkraceg puta u slucaju kada
        /// je graf usmeren i bez ciklusa. Sa druge strane graf moze da sadrzi negativne
        /// tezine grana.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="start"></param>
        public void DAG_shortest_path(IDirectedGraph<TVertex, TEdge> graph, TVertex start)
        {            
            IEnumerable<TVertex> topoList = topologicalSort.Search(graph, start);
            Initialize(graph, start);

            foreach (TVertex u in topoList)
            {
                vertexColors[u] = VertexColor.Black;
                OnExamineVertex(u);

                foreach (TEdge edge in graph.OutEdges(u))
                {
                    if (RelaxEdge(edge))
                    {
                        vertexColors[edge.Target] = VertexColor.Gray;
                        OnEdgeRelaxed(edge);
                    }
                }
            }
        }

        /// <summary>
        /// Dijsktra algoritam za trazenje najmanjeg rastojanja i odgovarajucih
        /// putanja. Graf mora imati sve pozitivne tezine grana ali moze da bude neusmeren
        /// i/ili da sadrzi cikluse (petlje).
        /// </summary>
        /// <param name="start"></param>
        public void Dijkstra_shortest_path(IDirectedGraph<TVertex, TEdge> graph, TVertex start)
        {
            //Inicijalizujemo kolekcije
            Initialize(graph, start);            
            minQueue.Clear();
            minQueue.Insert(start);

            while (minQueue.Count > 0)
            {
                TVertex u = minQueue.Remove();
                if (vertexColors[u] == VertexColor.Black)
                    continue;

                vertexColors[u] = VertexColor.Black;
                OnExamineVertex(u);

                foreach (TEdge edge in graph.OutEdges(u))
                {
                    if (RelaxEdge(edge))
                    {
                        vertexColors[edge.Target] = VertexColor.Gray;
                        minQueue.Insert(edge.Target);
                        OnEdgeRelaxed(edge);
                    }
                }
            }
        }

        /// <summary>
        /// Implementacija Bellman-Ford algoritma za pronalazenje najkracih putanja u
        /// tezinskom, orijentisanom ili neorijentisanom grafu sa ili bez ciklusa.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="start"></param>
        /// <returns>true u slucaju da je pronadjeno negativno pojacanje, inace false</returns>
        public bool BellmanFord(IDirectedGraph<TVertex, TEdge> graph, TVertex start)
        {
            //Inicijalizujemo kolekcije
            Initialize(graph, start);

            //Prolazimo kroz sve grane grafa |V|-1 puta i osvezavamo rastojanja izmedju nod-ova.
            //U slucaju da nema ciklusa sa kruznim pojacanjem, to ce da garantuje da su sve najkrace
            //putanje otkrivene (pogledati teorijsku osnovu algoritma).
            for(int i=0; i < graph.VertexCount - 1; i++)
            {
                foreach(TEdge edge in graph.Edges)
                {
                    OnExamineVertex(edge.Source);

                    if (RelaxEdge(edge))
                        OnEdgeRelaxed(edge);
                }
            }

            //Provera na negativne cikluse, ako se nakon |V|-1 iteracija javlja novo osvezenje 
            //grane u grafu, to je potvrda da postoje takvi ciklusi.
            foreach (TEdge edge in graph.Edges)
            {
                if (RelaxEdge(edge))
                    return true;
            }

            return false;
        }

        private bool RelaxEdge(TEdge edge)
        {
            double previousDistance = distances[edge.Target];
            double newDistance = distances[edge.Source] + EdgeWeights(edge);

            if (previousDistance > newDistance)
            {
                distances[edge.Target] = newDistance;
                return true;
            }
            return false;
        }

        protected void Initialize(IGraph<TVertex, TEdge> graph, TVertex start)
        {
            vertexColors.Clear();
            distances.Clear();
            //Inicijalizujemo kolekciju boja i rastojanja
            foreach (TVertex u in graph.Vertices)
            {
                vertexColors.Add(u, VertexColor.White);
                distances.Add(u, double.PositiveInfinity);
            }
            distances[start] = 0.0;
        }
    }
}
