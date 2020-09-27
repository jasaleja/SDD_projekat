using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.Trees
{
    public interface ITreeAlgorithms<TNode>
    {
        Dictionary<TNode, NodeColor> NodeColors { get; }
    }

    public enum NodeColor
    {
        White = 0,
        Gray,
        Black
    }
}
