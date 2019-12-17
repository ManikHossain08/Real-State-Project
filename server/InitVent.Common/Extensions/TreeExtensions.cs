using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Extensions
{
    public static class TreeExtensions
    {
        public static IEnumerable<TNode> WalkTree<TNode>(this TNode root, Func<TNode, IEnumerable<TNode>> childSelector, bool postOrder = false)
        {
            //return Walk(new[] { root }, childSelector, postOrder);

            Func<TNode, IEnumerable<TNode>> emptySelector = (node => new TNode[0]);

            if (postOrder)
                return WalkTree(root, emptySelector, childSelector);
            else
                return WalkTree(root, childSelector, emptySelector);
        }

        public static IEnumerable<TNode> WalkTree<TNode>(this TNode root, Func<TNode, IEnumerable<TNode>> preChildSelector, Func<TNode, IEnumerable<TNode>> postChildSelector)
        {
            var preChildren = preChildSelector(root).SelectMany(node => WalkTree(node, preChildSelector, postChildSelector));
            var postChildren = postChildSelector(root).SelectMany(node => WalkTree(node, preChildSelector, postChildSelector));

            return preChildren.Append(root).Concat(postChildren);
        }
    }
}
