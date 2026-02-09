using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_MonitorManager : MonoBehaviour
{
    public enum MonitorState
    {
        Off,
        LoadingIn,
        LoadingIdle,
        LoadingOut,
        On
    }

    [Header("State")]
    public MonitorState currentState = MonitorState.Off;

    [Header("References")]
    public Animator loadingAnimator;   // Animator di Layar loading

    [Header("Circle Spin")]
    public float circleSpinSpeed = 180f;
    public GameObject loadingCircle;   // object Loading (spinner)

    public GameObject screenOff;        // Screen_OFF
    public GameObject screenOn;         // Screen_ON

    [Header("Loading Visual")]
    public SpriteRenderer loadingRenderer;

    [Header("Timing")]
    public float delayBeforeIdle = 0.3f;
    public float loadingDuration = 2f;
    public float delayAfterOut = 0.1f;

    [Header("Colliders")]
    public Collider2D powerCollider;

    void Start()
    {
        currentState = MonitorState.Off;

        screenOff.SetActive(true);
        screenOn.SetActive(false);
        loadingCircle.SetActive(false);    // spinner mati
    }

    void Update()
    {
        if (loadingCircle.activeSelf)
        {
            loadingCircle.transform.Rotate(0, 0, -circleSpinSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (currentState == MonitorState.Off &&
                powerCollider.OverlapPoint(mousePos))
            {
                PowerOn();
            }
        }
    }

    public void PowerOn()
    {
        if (currentState != MonitorState.Off) return;
        StartCoroutine(PowerOnSequence());
    }

    IEnumerator PowerOnSequence()
    {
        // LOADING IN
        currentState = MonitorState.LoadingIn;
        loadingAnimator.SetTrigger("isIn");

        // Tunggu animasi IN selesai
        yield return new WaitUntil(() =>
            loadingAnimator.GetCurrentAnimatorStateInfo(0).IsName("in") &&
            loadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        // Delay kecil biar halus
        yield return new WaitForSeconds(0.1f);

        // LOADING IDLE
        currentState = MonitorState.LoadingIdle;
        loadingCircle.SetActive(true);

        yield return new WaitForSeconds(loadingDuration);

        // LOADING OUT
        currentState = MonitorState.LoadingOut;
        loadingCircle.SetActive(false);
        loadingAnimator.ResetTrigger("isIn");
        loadingAnimator.SetTrigger("isOut");
        yield return new WaitForSeconds(delayAfterOut);

        // MONITOR ON
        currentState = MonitorState.On;
        screenOff.SetActive(false);
        screenOn.SetActive(true);
    }
}
