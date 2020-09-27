using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Trees
{
    public interface IBinaryTree<TNode>
        where TNode : IBinaryTree<TNode>
    {
        TNode Left { get; }
        TNode Right { get; }
        int Size { get; }
    }
}
