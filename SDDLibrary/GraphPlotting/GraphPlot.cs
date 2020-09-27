using System;
using System.Collections.Generic;
using System.Threading;
using SDDLibrary.Utils;
using SDDLibrary.Graph;
using System.Linq;
using GraphvizViewer;

namespace SDDLibrary.GraphPlotting
{
    public class GraphPlot<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        private GraphvizForm graphForm;
        private Thread plotThread;
        private bool formLoaded;
        private object syncObj;
        private Func<TVertex, string> vertexDescription;
        private Func<TEdge, string> edgeDescription;
        private IGraph<TVertex, TEdge> graph;
        private Func<object, object, bool> vertexComparer;

        public GraphvizForm GraphForm
        {
            get { return graphForm; }
        }

        public GraphPlot(IGraph<TVertex, TEdge> graph, Func<TVertex, string> vertexDescription = null,
            Func<TEdge, string> edgeDescription = null)
        {
            if (vertexDescription == null)
                vertexDescription = (v) => v.ToString();

            //if (edgeDescription == null)
            //    edgeDescription = (e) => e.ToString();

            this.vertexDescription = vertexDescription;
            this.edgeDescription = edgeDescription;
            this.graph = graph;
            vertexComparer = (v1, v2) => graph.VertexComparer.Equals((TVertex)v1, (TVertex)v2);

            syncObj = new object();
            formLoaded = false;
            plotThread = new Thread(FormStarter);
            plotThread.SetApartmentState(ApartmentState.STA);
            //plotThread.IsBackground = true;
            plotThread.Start();

            lock (syncObj)
                while (!formLoaded)
                    Monitor.Wait(syncObj);
        }

        private void FormStarter()
        {
            graphForm = new GraphvizForm();
            graphForm.Load += PlotForm_Load;
            graphForm.ShowDialog();
        }

        private void PlotForm_Load(object sender, EventArgs e)
        {
            lock (syncObj)
            {
                formLoaded = true;
                Monitor.Pulse(syncObj);
            }
        }

        public void Refresh(IDictionary<TVertex,VertexColor> colors=null)
        {
            graphForm.SafeInvoke(d => 
            {
                var g = d.ViewModel.Graph;
                var attributesTemp = g.Attributes.ToArray();
                g.StartChanges();                
                g.Clear();

                foreach (var attr in attributesTemp)
                    g.Attributes.Add(attr);

                g.IsDirected = graph.IsDirected;                

                foreach (TEdge edge in graph.Edges)
                {
                    AddNewGraphEdge(edge, colors);
                }

                g.EndChanges();
            });
        }

        public void AddVertex(TVertex newVertex, IDictionary<TVertex, VertexColor> colors = null)
        {
            var v1 = new Node(vertexDescription(newVertex), newVertex, vertexComparer);
            if (colors != null)
                v1.Color = colors[newVertex].ToString();

            graphForm.SafeInvoke(d => d.ViewModel.Graph.AddVertex(v1));
        }

        public void AddNewGraphEdge(TEdge edge, IDictionary<TVertex, VertexColor> colors = null)
        {
            graphForm.SafeInvoke(d =>
            {
                var v1 = new Node(vertexDescription(edge.Source), edge.Source, vertexComparer);
                var v2 = new Node(vertexDescription(edge.Target), edge.Target, vertexComparer);

                if (colors != null)
                {
                    v1.Color = colors[edge.Source].ToString();
                    v2.Color = colors[edge.Target].ToString();
                }

                var e1 = new Graphviz4Net.Graphs.Edge<Node>(v1, v2, new Arrow());

                if(edgeDescription!=null)
                    e1.Attributes.Add("label", edgeDescription(edge));

                //if (edge.Value != null)
                //    e1.Attributes.Add("label",edge.Value);

                graphForm.ViewModel.Graph.AddEdgeAndVertices(e1);
            });
        }
    }
}
