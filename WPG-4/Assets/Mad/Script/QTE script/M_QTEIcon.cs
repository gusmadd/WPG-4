using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_QTEIcon : MonoBehaviour
{
    Animator anim;
    M_QTESystem qteSystem;
    bool isClicked = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Init(M_QTESystem system)
    {
        qteSystem = system;

        if (anim != null)
            anim.SetTrigger("PlayIn");
    }

    void OnMouseDown()
    {
        M_AudioManager.Instance?.PlayCursorClick();
        if (isClicked) return;
        isClicked = true;

        qteSystem.IconClicked(this);

        StartCoroutine(PlayOutAndDestroy());
    }

    IEnumerator PlayOutAndDestroy()
    {
        if (anim != null)
            anim.SetTrigger("PlayOut");

        yield return new WaitForSecondsRealtime(0.45f); // sesuaikan durasi animasi out

        Destroy(gameObject);
    }
}
