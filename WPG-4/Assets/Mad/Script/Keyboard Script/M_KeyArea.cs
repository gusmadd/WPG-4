using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_KeyArea : MonoBehaviour
{
    public string keyValue;
    public M_KeyboardController keyboard;

    void OnMouseDown()
    {
        if (keyboard == null)
        {
            Debug.LogError("Keyboard belum di-assign di " + gameObject.name);
            return;
        }
        

        keyboard.PressKey(keyValue);
    }
}
