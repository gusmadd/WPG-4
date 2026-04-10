using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // Untuk UI Image

public class M_CalendarAnimation : MonoBehaviour
{
    [Header("UI Image Settings")]
    public Image calendarImage;  // Image komponen untuk kalender
    public Sprite[] calendarFrames;  // Array untuk 5 frame kalender

    [Header("Timing Settings")]
    public float frameDelay = 0.2f;  // Waktu antara pergantian frame
    public float delayBeforeOut = 2f;  // Waktu delay sebelum animasi keluar

    private bool isPlayingOut = false;

    void Start()
    {
        // Mainkan SFX muncul kalender
        // Mulai animasi masuk
        StartCoroutine(PlayInAnimation());
    }

    IEnumerator PlayInAnimation()
    {
        // Mainkan animasi masuk dengan mengganti sprite frame
        for (int i = 0; i < 9; i++)
        {
            calendarImage.sprite = calendarFrames[i];  // Ganti sprite ke frame ke-i
            yield return new WaitForSeconds(frameDelay);  // Tunggu sesuai frameDelay
        }

        // Setelah animasi masuk selesai, tunggu 2 detik, kemudian animasi keluar
        yield return new WaitForSeconds(delayBeforeOut);

        // Mainkan animasi keluar dengan frame terbalik
        StartCoroutine(PlayOutAnimation());
    }

    IEnumerator PlayOutAnimation()
    {
        isPlayingOut = true;

        for (int i = 8; i >= 0; i--)  // Mulai dari frame terakhir dan berbalik
        {
            calendarImage.sprite = calendarFrames[i];  // Ganti sprite ke frame terbalik
            yield return new WaitForSeconds(frameDelay);  // Tunggu sesuai frameDelay
        }

        isPlayingOut = false;  // Animasi keluar selesai
    }
}