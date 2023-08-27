using System.Collections.Generic;

namespace BehaviorTree
{
    // State of an Node
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        // State of the node
        protected NodeState state;

        // Parent of the node, can be null
        public Node parent;
        // Children of the node
        protected List<Node> children = new List<Node>();

        // If there are no children, it can't be a parent
        public Node()
        {
            parent = null;
        }

        // If there are children, it can be a parent
        public Node(List<Node> children)
        {
            // Attach the children to the parent
            foreach (Node child in children)
                _Attach(child);
        }

        // Attach children to the node parameter
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        // Each derived-Node can implement its own evaluation function
        // and have an unique role in the behavior tree
        public virtual NodeState Evaluate() => NodeState.FAILURE; // (Update)
    }
}
