using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public event EventHandler OnButtonClicked;

    public void ProcessButtonClicked()
    {
        var handler = OnButtonClicked;
        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
