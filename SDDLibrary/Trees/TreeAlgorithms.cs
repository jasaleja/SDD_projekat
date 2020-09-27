using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Trees
{
    public class TreeAlgorithms<TNode> : ITreeAlgorithms<TNode>
        where TNode : IBinaryTree<TNode>
    {
        private Dictionary<TNode, NodeColor> nodeColors;

        public Dictionary<TNode, NodeColor> NodeColors
        {
            get { return nodeColors; }
        }

        #region Event handlers
        /// <summary>
        /// Aktivnost koju vrsimo kod prvog nailaska na nod
        /// u odgovarajucoj pretrazi. U slucaju DFS ova aktivnost
        /// je preorder obilazak nod-a.
        /// </summary>
        public event Action<TNode> DiscoverNode;

        /// <summary>
        /// Aktivnost koju vrsimo nakon sto smo analizirali nod i pripremili
        /// sve njegove potomke za dalju analizu. Kod DFS ova aktivnost je
        /// postorder obilazak nod-a.
        /// </summary>
        public event Action<TNode> FinishNode;
        
        protected virtual void OnDiscoverNode(TNode node)
        {
            if (DiscoverNode != null)
                DiscoverNode(node);
        }

        protected virtual void OnFinishNode(TNode node)
        {
            if (FinishNode != null)
                FinishNode(node);
        }
        #endregion

        public TreeAlgorithms()
        {
            nodeColors = new Dictionary<TNode, NodeColor>();
        }

        public void DFS(TNode node)
        {
            Initialize(node);
            DFSInternal(node);
        }

        protected void DFSInternal(TNode node)
        {
            TNode leftChild = node.Left;
            TNode rightChild = node.Right;

            //Preorder akcija
            OnDiscoverNode(node);

            //Levo podstablo
            if (leftChild != null)
                DFSInternal(leftChild);

            //Desno podstablo
            if (rightChild != null)
                DFSInternal(rightChild);

            //Postorder akcija
            OnFinishNode(node);
        }

        public void Inorder(TNode node)
        {
            Initialize(node);
            InorderInternal(node);
        }

        protected void InorderInternal(TNode node)
        {
            TNode leftChild = node.Left;
            TNode rightChild = node.Right;            

            if (leftChild != null)
                InorderInternal(leftChild);

            OnDiscoverNode(node);

            if (rightChild != null)
                InorderInternal(rightChild);
        }

        public void BFS(TNode node)
        {
            Initialize(node);
            //Pocetni nod
            nodeColors[node] = NodeColor.Gray;

            //Red u koji smestamo nailazece nod-ove
            Queue<TNode> queue = new Queue<TNode>();
            queue.Enqueue(node);
            OnDiscoverNode(node);

            //Pretraga
            while (queue.Count > 0)
            {
                //Preuzimamo sledeci nod u bfs maniru
                TNode currentNode = queue.Dequeue();

                //Smestamo child nodove u red
                if (currentNode.Left != null)
                {
                    nodeColors[currentNode.Left] = NodeColor.Gray;
                    OnDiscoverNode(currentNode.Left);
                    queue.Enqueue(currentNode.Left);
                }

                if (currentNode.Right != null)
                {
                    nodeColors[currentNode.Right] = NodeColor.Gray;
                    OnDiscoverNode(currentNode.Right);
                    queue.Enqueue(currentNode.Right);
                }

                //Zavrsavamo sa nod-om
                nodeColors[currentNode] = NodeColor.Black;
                OnFinishNode(currentNode);
            }
        }

        public void Initialize(TNode root)
        {
            nodeColors.Clear();
            Stack<TNode> stack = new Stack<TNode>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                //Preuzimamo sledeci nod u bfs maniru
                TNode currentNode = stack.Pop();
                nodeColors[currentNode] = NodeColor.White;

                //Smestamo child nodove u red
                if (currentNode.Left != null)
                    stack.Push(currentNode.Left);

                if (currentNode.Right != null)
                    stack.Push(currentNode.Right);
            }
        }       
    }
}
