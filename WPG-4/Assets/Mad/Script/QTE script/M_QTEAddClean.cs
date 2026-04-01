using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_QTEAddClean : MonoBehaviour
{
    [Header("Ad Settings")]
    public List<GameObject> adPrefab;
    public int totalAds = 15;

    [Header("Spawn Area (World)")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Timer")]
    public float totalTime = 15f;

    [Header("Ad Scaling")]
    public int baseAds = 15;
    public int adsIncreasePerQTE = 3;
    public int maxAds = 30; // opsional

    [Header("Spawn Timing")]
    public float spawnInterval = 0.08f; // cepat, bisa diatur di Inspector

    float timer;
    bool isRunning = false;
    List<M_QTEPopUp> activeAds = new List<M_QTEPopUp>();

    void Start()
    {
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.QTE)
            return;

        int qteIndex = Mathf.Max(1, M_GameManager.Instance.qteCount);
        totalAds = baseAds + (qteIndex - 1) * adsIncreasePerQTE;
        if (maxAds > 0) totalAds = Mathf.Min(totalAds, maxAds);

        timer = totalTime;
        isRunning = true;

        // 🔥 AKTIFKAN TIMER UI
        UI_Script.Instance.StartTimer(totalTime);

        StartCoroutine(SpawnAdsRoutine());
    }

    void Update()
    {
        if (!isRunning) return;
        if (M_GameManager.Instance.currentState != M_GameManager.GameState.QTE)
            return;

        timer -= Time.unscaledDeltaTime;

        float speedMultiplier = Mathf.Lerp(2f, 1f, (float)activeAds.Count / totalAds);
        timer -= Time.unscaledDeltaTime * (speedMultiplier - 1f);

        // 🔥 UPDATE FILL TIMER
        UI_Script.Instance.UpdateTimer(timer, totalTime);

        if (timer <= 0f)
        {
            Fail();
        }
    }

    IEnumerator SpawnAdsRoutine()
    {
        for (int i = 0; i < totalAds; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            GameObject randomAd = adPrefab[Random.Range(0, adPrefab.Count)];
            GameObject obj = Instantiate(randomAd, pos, Quaternion.identity);

            M_QTEPopUp popup = obj.GetComponent<M_QTEPopUp>();
            if (popup != null)
            {
                popup.Init(this);
                activeAds.Add(popup);
            }

            if (spawnInterval > 0f)
                yield return new WaitForSecondsRealtime(spawnInterval);
        }
    }

    public void AdClosed(M_QTEPopUp popup)
    {
        activeAds.Remove(popup);

        if (activeAds.Count <= 0)
        {
            Success();
        }
    }

    void Success()
    {
        isRunning = false;

        // 🔥 MATIKAN TIMER UI
        UI_Script.Instance.StopTimer();

        StartCoroutine(SuccessRoutine());
    }

    IEnumerator SuccessRoutine()
    {
        yield return M_GameManager.Instance.QTESuccess();
        Destroy(gameObject);
    }

    void Fail()
    {
        isRunning = false;
        UI_Script.Instance?.StopTimer();

        CleanupAds();

        StartCoroutine(FailRoutine());
    }

    IEnumerator FailRoutine()
    {
        yield return StartCoroutine(M_GameManager.Instance.QTEFail());
        Destroy(gameObject);
    }

    public void CleanupAds()
    {
        for (int i = 0; i < activeAds.Count; i++)
        {
            if (activeAds[i] != null)
                Destroy(activeAds[i].gameObject);
        }
        activeAds.Clear();
    }
}