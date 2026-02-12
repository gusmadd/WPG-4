using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_KeyArea : MonoBehaviour
{
    public string keyValue;
    public M_SearchInput searchField;
    void OnMouseDown()
    {
        searchField.AddCharacter(keyValue);
    }
}
