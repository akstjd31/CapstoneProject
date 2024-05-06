using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class BakeNavMesh : MonoBehaviour
{
    public NavMeshSurface navMeshSurface = new NavMeshSurface();
    
    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
