using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // Untuk UI Image

public class M_CalendarAnimation : MonoBehaviour
{
    [Header("UI Image Settings")]
    public Image calendarImage;
    public Sprite[] calendarFrames;

    [Header("Timing Settings")]
    public float frameDelay = 0.2f;
    public float delayBeforeOut = 2f;

    private bool isPlayingOut = false;

    void Start()
    {
        M_AudioManager.Instance?.PlayInCalendar();
        StartCoroutine(PlayInAnimation());
    }

    IEnumerator PlayInAnimation()
    {
        if (calendarImage == null || calendarFrames == null || calendarFrames.Length == 0)
            yield break;

        for (int i = 0; i < calendarFrames.Length; i++)
        {
            calendarImage.sprite = calendarFrames[i];
            yield return new WaitForSeconds(frameDelay);
        }

        yield return new WaitForSeconds(delayBeforeOut);
        M_AudioManager.Instance?.PlayOutCalendar();
        yield return StartCoroutine(PlayOutAnimation());
    }

    IEnumerator PlayOutAnimation()
    {
        if (isPlayingOut) yield break;
        isPlayingOut = true;

        for (int i = calendarFrames.Length - 1; i >= 0; i--)
        {
            calendarImage.sprite = calendarFrames[i];
            yield return new WaitForSeconds(frameDelay);
        }

        isPlayingOut = false;
    }
}