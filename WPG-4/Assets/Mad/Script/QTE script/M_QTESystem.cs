using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M_QTESystem : MonoBehaviour
{
    [Header("Icon")]
    public GameObject iconPrefab;

    [Header("Spawn Area (World)")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Spawn Timeline")]
    public int[] spawnAmounts = { 2, 4, 6, 8, 10 };
    public float[] spawnTimes = { 1, 3, 6, 9, 12 };

    [Header("Spawn Speed")]
    public float spawnBurstDelay = 0.08f;

    [Header("Timer")]
    public float totalDuration = 15f;

    float timer;
    bool isRunning = false;

    List<M_QTEIcon> activeIcons = new List<M_QTEIcon>();

    void Start()
    {
        timer = totalDuration;
        isRunning = true;

        UI_Script.Instance.StartTimer(totalDuration);

        StartCoroutine(SpawnTimeline());
    }

    void Update()
    {
        if (!isRunning) return;

        timer -= Time.unscaledDeltaTime;

        UI_Script.Instance.UpdateTimer(timer, totalDuration);

        if (timer <= 0f)
        {
            CheckResult();
        }
    }

    IEnumerator SpawnTimeline()
    {
        for (int i = 0; i < spawnTimes.Length; i++)
        {
            yield return new WaitForSecondsRealtime(
                i == 0 ? spawnTimes[i] : spawnTimes[i] - spawnTimes[i - 1]
            );

            StartCoroutine(SpawnBurst(spawnAmounts[i]));
        }
    }

    IEnumerator SpawnBurst(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnSingle();
            yield return new WaitForSecondsRealtime(spawnBurstDelay);
        }
    }

    void SpawnSingle()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        GameObject obj = Instantiate(iconPrefab, spawnPos, Quaternion.identity);

        M_QTEIcon icon = obj.GetComponent<M_QTEIcon>();
        icon.Init(this);

        activeIcons.Add(icon);
    }

    public void IconClicked(M_QTEIcon icon)
    {
        activeIcons.Remove(icon);
    }

    void CheckResult()
    {
        isRunning = false;

        UI_Script.Instance.StopTimer();

        if (activeIcons.Count == 0)
        {
            StartCoroutine(Success());
        }
        else
        {
            Fail();
        }
    }

    IEnumerator Success()
    {
        yield return M_GameManager.Instance.QTESuccess();
        Destroy(gameObject);
    }

    void Fail()
    {
        M_GameManager.Instance.GameOver();
    }
}