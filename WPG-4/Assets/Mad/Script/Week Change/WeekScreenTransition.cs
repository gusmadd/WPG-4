using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WeekSceneTransition : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform imageToSlide;  // Gambar atau elemen UI yang akan digeser
    public float slideDuration = 1.5f;  // Durasi untuk transisi geser
    public Vector2 slideStartPosition = new Vector2(-1920f, 0f);  // Posisi awal (di luar layar kiri)
    public Vector2 slideEndPosition = new Vector2(0f, 0f);  // Posisi akhir (normal, di layar)

    void Start()
    {
        // Mulai transisi slide saat scene dibuka
        StartCoroutine(SlideInAnimation());
    }

    IEnumerator SlideInAnimation()
    {
        // Set posisi awal gambar di luar layar (kiri)
        imageToSlide.anchoredPosition = slideStartPosition;

        float elapsedTime = 0f;
        Vector2 startPos = imageToSlide.anchoredPosition;

        // Lerp (linear interpolation) untuk pergerakan dari kiri ke kanan
        while (elapsedTime < slideDuration)
        {
            imageToSlide.anchoredPosition = Vector2.Lerp(startPos, slideEndPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Pastikan objek berada di posisi akhir setelah animasi selesai
        imageToSlide.anchoredPosition = slideEndPosition;
    }
}