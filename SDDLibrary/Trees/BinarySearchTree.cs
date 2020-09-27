using SDDLibrary.TreePlotting;
using SDDLibrary.Trees;
using System;
using System.Collections.Generic;

namespace SDDLibrary.Structures
{
    //Pojednostavljena konstrukcija algoritama za bst
    public class BSTAlgorithms<K, V> : TreeAlgorithms<BST<K, V>.BSTNode> where V : class { }

    public class BST<K, V> where V : class
    {
        #region Internal types
        public class BSTNode : IBinaryTree<BSTNode>
        {
            public BSTNode Left { get; set; }
            public BSTNode Right { get; set; }
            public V Value { get; set; }
            public K Key { get; set; }
            public int Size { get; set; }

            public BSTNode(K key, V value)
            {
                Size = 1;
                Value = value;
                Key = key;
            }
        }
        #endregion

        private BSTNode root;
        private Comparison<K> comparer;

        public BST(Comparison<K> comparer = null)
        {
            if (comparer == null)
                this.comparer = Comparer<K>.Default.Compare;
            else
                this.comparer = comparer;
        }

        public V Get(K key)
        {
            return Get(root, key);
        }

        private V Get(BSTNode x, K key)
        {   //Vrati vrednost vezanu za kljuc ako postoji, inace vratiti null.
            if (x == null)
                return null;

            int cmp = comparer(key, x.Key);

            if (cmp < 0)
                return Get(x.Left, key);
            else if (cmp > 0)
                return Get(x.Right, key);
            else
                return x.Value;
        }

        public void Put(K key, V val)
        {   //Pretraga za kljucem, krecemo od root elementa.
            root = Put(root, key, val);
        }

        private BSTNode Put(BSTNode x, K key, V val)
        {   //Promena vrednosti na val ako postoji nod sa kljucem key.
            //Inace, dodaje se novi nod sa kljucem key i vrednosti val.
            if (x == null)
                return new BSTNode(key, val);

            int cmp = comparer(key, x.Key);
            if (cmp < 0)
                x.Left = Put(x.Left, key, val);
            else if (cmp > 0)
                x.Right = Put(x.Right, key, val);
            else
                x.Value = val;

            x.Size = Size(x.Left) + Size(x.Right) + 1;
            return x;
        }

        private int Size(BSTNode node)
        {
            if (node == null)
                return 0;
            else
                return node.Size;
        }

        public int Size()
        {
            return root.Size;
        }

        #region Visualization
        private TreePlot<BSTNode> treePlot;

        /// <summary>
        /// Za prikazivanje sadrzaja nod-a se poziva
        /// odgovarajuca implementacija metode ToString() u klasi V.
        /// </summary>
        public void PlotTree(ITreeAlgorithms<BSTNode> algorithm=null, Func<BSTNode,string> nodePresenter=null)
        {
            if(nodePresenter==null)
            {
                nodePresenter = node =>
                {
                    string keyString = node.Key.ToString();
                    string valString = node.Value != null ? null : node.Value.ToString();
                    return keyString.Equals(valString) || string.IsNullOrEmpty(valString) ? keyString : string.Concat(node.Key, ":", node.Value);
                };
            }

            treePlot = new TreePlot<BSTNode>(root, nodePresenter);
            treePlot.Refresh(algorithm);
        }

        public void PlotRefresh(ITreeAlgorithms<BSTNode> algorithm = null, Func<BSTNode, string> nodePresenter = null)
        {
            if (treePlot != null)
                treePlot.Refresh(algorithm);
            else
                PlotTree(algorithm, nodePresenter);
        }    
        #endregion
    }
}
