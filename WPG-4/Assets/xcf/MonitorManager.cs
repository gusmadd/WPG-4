using System.Collections;
using UnityEngine;

public class MonitorManager : MonoBehaviour
{
    public static MonitorManager Instance;

    [Header("Monitor Objects")]
    public GameObject monitorScreen;   // mesh / canvas monitor
    public GameObject bootScreen;      // layar boot
    public GameObject desktopScreen;   // desktop utama
    public GameObject offScreen;       // layar mati

    [Header("Boot Settings")]
    public float bootTime = 2f;

    bool isOn = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetMonitorOff();
    }

    // =========================
    // POWER CONTROL
    // =========================

    public void PowerOn()
    {
        if (isOn) return;

        isOn = true;
        StartCoroutine(BootSequence());
    }

    public void PowerOff()
    {
        isOn = false;

        if (bootScreen != null) bootScreen.SetActive(false);
        if (desktopScreen != null) desktopScreen.SetActive(false);
        if (offScreen != null) offScreen.SetActive(true);
    }

    // =========================
    // BOOT SEQUENCE
    // =========================

    IEnumerator BootSequence()
    {
        if (offScreen != null) offScreen.SetActive(false);

        if (bootScreen != null)
            bootScreen.SetActive(true);

        yield return new WaitForSeconds(bootTime);

        if (bootScreen != null)
            bootScreen.SetActive(false);

        if (desktopScreen != null)
            desktopScreen.SetActive(true);
    }

    // =========================
    // MONITOR STATES
    // =========================

    public void SetMonitorOff()
    {
        if (bootScreen != null) bootScreen.SetActive(false);
        if (desktopScreen != null) desktopScreen.SetActive(false);
        if (offScreen != null) offScreen.SetActive(true);
    }

    public void ShowDesktop()
    {
        if (!isOn) return;

        if (bootScreen != null) bootScreen.SetActive(false);
        if (desktopScreen != null) desktopScreen.SetActive(true);
    }

    public bool IsMonitorOn()
    {
        return isOn;
    }
}