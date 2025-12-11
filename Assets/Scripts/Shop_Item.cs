using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Item : MonoBehaviour
{
    public bool leftHanded = false;
    public bool clicked = false;
    public bool isOver = false;
    public bool clickable = false;

    private void OnMouseOver()
    {
        isOver = true;
        if (Input.GetMouseButtonDown(0))
        {
            clicked = true;
            leftHanded = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            clicked = true;
            leftHanded = false;
        }
    }
}

