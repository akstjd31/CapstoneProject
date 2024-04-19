using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapToggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(gameObject.transform.GetChild(0).gameObject.activeSelf)
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
