using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public event EventHandler OnButtonClicked;

    public AudioSource ButtonClick;

    public void ProcessButtonClicked()
    {
        ButtonClick.Play();

        var handler = OnButtonClicked;
        if(handler != null)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
