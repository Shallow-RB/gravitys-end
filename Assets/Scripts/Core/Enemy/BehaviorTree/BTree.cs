using UnityEngine;

namespace BehaviorTree
{
    public abstract class BTree : MonoBehaviour
    {
        // The root node
        private Node _root = null;
        // Previous node state
        protected NodeState previousState;

        [HideInInspector]
        public bool state = false;

        private void Update()
        {
            // If there is a root, evaluate
            if (_root != null && state)
                _root.Evaluate();
        }
        
        public virtual void SetTree()
        {
            _root = SetupTree();
        }

        protected abstract Node SetupTree();
    }
}
