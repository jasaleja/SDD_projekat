using SDDLibrary.Heap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph.UGraph
{
    public class ShortestPathAlgorithms<TVertex, TEdge> : IGraphAlgorithms<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private PQMin<TVertex> minQueue;
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
            distances = new Dictionary<TVertex, double>();
        }

        private int VertexWeightComparer(TVertex x, TVertex y)
        {
            return distances[x].CompareTo(distances[y]);
        }        

        /// <summary>
        /// Dijsktra algoritam za trazenje najmanjeg rastojanja i odgovarajucih
        /// putanja. Graf mora imati sve pozitivne tezine grana ali moze da bude neusmeren
        /// i/ili da sadrzi cikluse (petlje).
        /// </summary>
        /// <param name="start"></param>
        public void Dijkstra_shortest_path(IUndirectedGraph<TVertex, TEdge> graph, TVertex start)
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

                foreach (TEdge edge in graph.AdjacentEdges(u))
                {
                    TVertex v = graph.MateOf(u, edge);
                    if (RelaxEdge(u, v, edge))
                    {
                        vertexColors[edge.Target] = VertexColor.Gray;
                        minQueue.Insert(v);
                        OnEdgeRelaxed(edge);
                    }
                }
            }
        }

        private bool RelaxEdge(TVertex u, TVertex v, TEdge e)
        {
            double previousDistance = distances[v];
            double newDistance = distances[u] + EdgeWeights(e);

            if (previousDistance > newDistance)
            {
                distances[v] = newDistance;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Implementacija Bellman-Ford algoritma za pronalazenje najkracih putanja u
        /// tezinskom, orijentisanom ili neorijentisanom grafu sa ili bez ciklusa.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="start"></param>
        /// <returns>true u slucaju da je pronadjeno negativno pojacanje, inace false</returns>
        public bool BellmanFord(IUndirectedGraph<TVertex, TEdge> graph, TVertex start)
        {
            //Inicijalizujemo kolekcije
            Initialize(graph, start);
            bool relaxed = false;
            TVertex pivot = default(TVertex);

            //Prolazimo kroz sve grane grafa |V|-1 puta i osvezavamo rastojanja izmedju nod-ova.
            //U slucaju da nema ciklusa sa kruznim pojacanjem, to ce da garantuje da su sve najkrace
            //putanje otkrivene (pogledati teorijsku osnovu algoritma).            
            for (int i = 0; i < graph.VertexCount - 1; i++)
            {
                foreach (TEdge e in graph.Edges)
                {
                    //Kod neusmerenog grafa se postojanje negativnog ciklusa otkriva
                    //jednostavno proverom negativnosti grana.
                    if(EdgeWeights(e) < 0)
                        return true;

                    relaxed = RelaxEdge(e, out pivot);
                    OnExamineVertex(pivot);

                    if (relaxed)
                        OnEdgeRelaxed(e);
                }
            }

            return false;
        }

        private bool RelaxEdge(TEdge e, out TVertex pivot)
        {
            TVertex u = e.Source;
            TVertex v = e.Target;
            double previousDistance = distances[v];
            double newDistance = distances[u];

            if(previousDistance < newDistance)
            {
                Swap(ref previousDistance, ref newDistance);
                Swap(ref u, ref v);                
            }

            pivot = u;
            newDistance += EdgeWeights(e);

            if (previousDistance > newDistance)
            {
                distances[v] = newDistance;
                return true;
            }
            return false;
        }

        protected void Initialize(IUndirectedGraph<TVertex, TEdge> graph, TVertex start)
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

        private void Swap<K>(ref K x, ref K y)
        {
            K temp = x;
            x = y;
            y = temp;
        }
    }
}
