using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    float timerForDoubleClick = 0.0f;
    float delay = 0.3f;
    bool isDoubleClick = false;

    public event EventHandler OnDoubleClick;

    void Update()
    {
        if (isDoubleClick == true)
        {
            timerForDoubleClick += Time.deltaTime;
        }


        if (timerForDoubleClick >= delay)
        {
            timerForDoubleClick = 0.0f;
            isDoubleClick = false;
        }

    }

    void OnMouseOver()
    {
        if (Input.GetButtonDown("Fire1") && isDoubleClick == false)
        {
            // Debug.Log("Mouse clicked once");
            isDoubleClick = true;
        }
    }

    void OnMouseDown()
    {
        if (isDoubleClick == true && timerForDoubleClick < delay)
        {
            // Debug.Log("ISWORKING!!!!");

            // Notify others of double click.
            var handler = OnDoubleClick;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
