using UnityEngine;
using UnityEngine.AI;

namespace Core.StageGeneration.Stage
{
    public class NavMeshBaker : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            var navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();
            foreach (var navMeshSurface in navMeshSurfaces)
                navMeshSurface.BuildNavMesh();
        }
    }
}
