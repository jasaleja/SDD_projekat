using SDDLibrary.TreePlotting;
using SDDLibrary.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Structures
{
    //Pojednostavljena konstrukcija algoritama za binarno stablo
    public class BTAlgorithms<V> : TreeAlgorithms<BinaryTree<V>> { }

    public class BinaryTree<V>:IBinaryTree<BinaryTree<V>>
    {
        public BinaryTree<V> Parent { get; private set; }        
        public BinaryTree<V> Left { get; private set; }
        public BinaryTree<V> Right { get; private set; }
        public V Value { get; set; }        
        public int Size { get; private set; }

        public int Depth()
        {
            var parent = this;
            int depth = -1;
            while (parent != null)
            {
                depth++;
                parent = parent.Parent;
            }
            return depth;
        }

        public BinaryTree(V value)
        {
            Size = 1;
            Value = value;
        }

        public BinaryTree<V> AddLeft(BinaryTree<V> left)
        {
            if (Left != null)
            {
                throw new Exception("Left node already exists.");
            }
            else
            {
                Left = left;
                Left.Parent = this;

                //Osvezavamo velicine podstabala
                var parent = this;
                while (parent != null)
                {
                    parent.Size += left.Size;
                    parent = parent.Parent;
                }
                return left;
            }
        }

        public BinaryTree<V> AddLeft(V value)
        {
            return AddLeft(new BinaryTree<V>(value));
        }

        public virtual BinaryTree<V> AddRight(BinaryTree<V> right)
        {
            if (Right != null)
            {
                throw new Exception("Right node already exists.");
            }
            else
            {
                Right = right;
                right.Parent = this;

                //Osvezavamo velicine podstabala
                var parent = this;
                while (parent != null)
                {
                    parent.Size += right.Size;
                    parent = parent.Parent;
                }
                return right;
            }
        }        

        public BinaryTree<V> AddRight(V value)
        {
            return AddRight(new BinaryTree<V>(value));
        }

        #region Visualization
        private TreePlot<BinaryTree<V>> treePlot;

        /// <summary>
        /// Za prikazivanje sadrzaja nod-a se poziva
        /// odgovarajuca implementacija metode ToString()
        /// u klasi V.
        /// </summary>
        public void PlotTree(ITreeAlgorithms<BinaryTree<V>> algorithm = null, Func<BinaryTree<V>, string> nodePresenter = null)
        {
            if (nodePresenter == null)
                nodePresenter = node => node.Value.ToString();

            treePlot = new TreePlot<BinaryTree<V>>(this, nodePresenter);
            treePlot.Refresh(algorithm);
        }

        public void PlotRefresh(ITreeAlgorithms<BinaryTree<V>> algorithm = null, Func<BinaryTree<V>, string> nodePresenter = null)
        {
            if (treePlot != null)
                treePlot.Refresh(algorithm);
            else
                PlotTree(algorithm, nodePresenter);
        }
        #endregion
    }
}
