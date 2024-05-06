using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class BakeNavMesh : MonoBehaviour
{
    public NavMeshSurface navMeshSurface = new NavMeshSurface();
    
    public void BakeNavigation()
    {
        navMeshSurface.BuildNavMesh();
    }
}
