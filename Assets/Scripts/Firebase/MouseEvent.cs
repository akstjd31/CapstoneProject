using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
    private void OnMouseEnter()
    {
        Debug.Log($"OnMouseEnter : {gameObject.name}");
    }
    private void OnMouseUp()
    {
        Debug.Log($"OnMouseUp : {gameObject.name}");
    }
    private void OnMouseOver()
    {
        Debug.Log($"OnMouseOver : {gameObject.name}");
    }
    private void OnMouseDown()
    {
        Debug.Log($"OnMouseDown : {gameObject.name}");
    }
    private void OnMouseDrag()
    {
        Debug.Log($"OnMouseDrag : {gameObject.name}");
    }
    private void OnMouseExit()
    {
        Debug.Log($"OnMouseExit : {gameObject.name}");
    }
    private void OnMouseUpAsButton()
    {
        Debug.Log($"OnMouseUpAsButton : {gameObject.name}");
    }
}
