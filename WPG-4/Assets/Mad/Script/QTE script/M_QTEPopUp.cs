using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_QTEPopUp : MonoBehaviour
{
    M_QTEAdClean manager;
    bool isClosed = false;

    public void Init(M_QTEAdClean qteManager)
    {
        manager = qteManager;
    }

    void OnMouseDown()
    {
        if (!M_NoiseSystem.Instance.isQTEActive) return;
        if (isClosed) return;

        CloseAd();
    }

    void CloseAd()
    {
        isClosed = true;

        manager.AdClosed(this);

        Destroy(gameObject);
    }
}
