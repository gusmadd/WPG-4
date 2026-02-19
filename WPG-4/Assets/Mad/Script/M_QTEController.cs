using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_QTEController : MonoBehaviour
{
  public Animator animator;

    void Start()
    {
        animator.SetTrigger("PlayIn");
    }

    public void CloseQTE()
    {
        animator.SetTrigger("PlayOut");
    }
}
