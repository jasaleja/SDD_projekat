using System;
using System.Collections.Generic;
using System.Threading;
using SDDLibrary.Utils;
using GraphvizViewer;
using SDDLibrary.Trees;

namespace SDDLibrary.TreePlotting
{
    public class TreePlot<TNode>
        where TNode : IBinaryTree<TNode>
    {
        private GraphvizForm graphForm;
        private Thread plotThread;
        private bool formLoaded;
        private object syncObj;
        private Func<TNode, string> nodeDescription;
        private TNode tree;
        private Func<object, object, bool> nodeComparer;

        public TreePlot(TNode tree, Func<TNode, string> nodeDescription = null)
        {
            if (nodeDescription == null)
                nodeDescription = (v) => v.ToString();

            this.nodeDescription = nodeDescription;
            this.tree = tree;
            nodeComparer = (v1, v2) => EqualityComparer<TNode>.Default.Equals((TNode)v1, (TNode)v2);

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

        public void Refresh(ITreeAlgorithms<TNode> algorithm=null)
        {
            graphForm.SafeInvoke(d => 
            {
                var g = d.ViewModel.Graph;
                g.StartChanges();
                g.Clear();
                g.IsDirected = true;
                ConstructTreeGraph(algorithm);
                g.EndChanges();
            });
        }

        private void ConstructTreeGraph(ITreeAlgorithms<TNode> algorithm=null)
        {
            Stack<TNode> nodes = new Stack<TNode>();

            Action<TNode> nodeAction = (node) =>
            {
                AddNode(node, algorithm);
                nodes.Push(node);
            };

            Action<TNode> postAction = (node) =>
            {
                nodes.Pop();

                if (nodes.Count > 0)
                {
                    AddNewTreeEdge(nodes.Peek(), node, algorithm);
                }
            };

            var alg = new TreeAlgorithms<TNode>();
            alg.DiscoverNode += nodeAction;
            alg.FinishNode += postAction;
            alg.DFS(tree);
        }

        public void AddNode(TNode newNode, ITreeAlgorithms<TNode> algorithm=null)
        {
            var n1 = new Node(nodeDescription(newNode), newNode, nodeComparer);
            if (algorithm != null)
                n1.Color = algorithm.NodeColors[newNode].ToString();

            graphForm.SafeInvoke(d => d.ViewModel.Graph.AddVertex(n1));
        }

        public void AddNewTreeEdge(TNode source, TNode target, ITreeAlgorithms<TNode> algorithm=null)
        {
            graphForm.SafeInvoke(d =>
            {
                var n1 = new Node(nodeDescription(source), source, nodeComparer);
                var n2 = new Node(nodeDescription(target), target, nodeComparer);

                if (algorithm != null)
                {
                    n1.Color = algorithm.NodeColors[source].ToString();
                    n1.Color = algorithm.NodeColors[target].ToString();
                }

                //string edgeString = string.Format("{0}-{1} Connected", v1.ID, v2.ID);
                var e1 = new Graphviz4Net.Graphs.Edge<Node>(n1, n2, new Arrow());
                graphForm.ViewModel.Graph.AddEdgeAndVertices(e1);
            });
        }
    }
}
