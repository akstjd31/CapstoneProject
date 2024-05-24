using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParent : MonoBehaviour
{
    public bool gridMap = false;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.transform.parent = FindParent();
    }

    private Transform FindParent()
    {
        Transform parent;
        if(gridMap)
        {
            parent = GameObject.Find("Grid").transform;
        }
        else
        {
            parent = GameObject.Find("Map").transform;
        }
        return parent;
    }
}