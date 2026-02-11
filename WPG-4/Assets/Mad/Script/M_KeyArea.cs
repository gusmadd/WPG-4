using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_KeyArea : MonoBehaviour
{
    public string keyValue;                      // nilai tombol
    public M_SearchInput searchField;            // reference ke search input
    public M_KeyboardController keyboard;        // reference ke keyboard
    void OnMouseDown()
    {
        // CAPSLOCK
        if (keyValue == "CAPS")
        {
            keyboard.ToggleCaps();
            return;
        }

        // SPASI
        if (keyValue == "SPACE")
        {
            searchField.AddCharacter(" ");
            return;
        }

        // BACKSPACE / ENTER langsung kirim saja
        if (keyValue == "BACK" || keyValue == "ENTER")
        {
            searchField.AddCharacter(keyValue);
            return;
        }

        // HURUF
        if (keyboard.isCaps && keyValue.Length == 1)
        {
            searchField.AddCharacter(keyValue.ToUpper());
        }
        else
        {
            searchField.AddCharacter(keyValue.ToLower());
        }
    }
}
