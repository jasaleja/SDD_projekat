using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Graph.Digraph
{
    public class TopologicalSort<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private DepthFirstSearch<TVertex, TEdge> dfs;
        private bool allowCyclicGraph;
        private Stack<TVertex> sortedVertices;

        public TopologicalSort(bool allowCyclicGraph = false)
        {
            dfs = new DepthFirstSearch<TVertex, TEdge>();
            dfs.ExamineEdge += BackEdge;
            dfs.FinishVertex += AddToSorted;
            sortedVertices = new Stack<TVertex>();
            this.allowCyclicGraph = allowCyclicGraph;
        }

        private void BackEdge(TEdge edge)
        {
            if (!allowCyclicGraph && dfs.VertexColors[ edge.Target]== VertexColor.Gray)
                throw new Exception(string.Format("Topological sort is marked to not allow cycles, which are present. Edge: {0}", edge));
        }

        private void AddToSorted(TVertex v)
        {
            sortedVertices.Push(v);
        }

        public IEnumerable<TVertex> Search(IDirectedGraph<TVertex, TEdge> graph, params TVertex[] roots)
        {
            return Search(graph, (IEnumerable<TVertex>)roots);
        }

        public IEnumerable<TVertex> Search(IDirectedGraph<TVertex, TEdge> graph, IEnumerable<TVertex> roots)
        {
            //Nova pretraga oslobadja bafer
            sortedVertices = new Stack<TVertex>(graph.VertexCount);

            //U slucaju da nije odredjen pocetni nod, krecemo od svih nodova redom
            if (roots == null || roots.Count() == 0)
                roots = graph.Vertices.Where(v => graph.InEdges(v).Count() == 0);

            //Poziv dfs da izracuna postorder redosled za sve cvorove grafa
            dfs.Search(graph, roots);
            return sortedVertices;
        }
    }
}
