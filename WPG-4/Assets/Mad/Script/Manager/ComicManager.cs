using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicManager : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;  // Animator untuk comic

    [Header("Animation Settings")]
    public float sceneDuration = 3f; // Durasi untuk menampilkan setiap scene
    public float sceneTransitionDuration = 1f; // Durasi transisi antara scene (in/out)

    public string nextSceneName = "WeekScene";  // Nama scene berikutnya (misalnya "WeekScene")

    void Start()
    {
        // Mulai animasi komik
        StartCoroutine(PlayComicScenes());
    }

    IEnumerator PlayComicScenes()
    {
        animator.SetTrigger("Scene1In");
        yield return new WaitForSeconds(sceneDuration);
        animator.SetTrigger("Scene1Out");
        yield return new WaitForSeconds(sceneTransitionDuration);
        animator.SetTrigger("Scene2In");
        yield return new WaitForSeconds(sceneDuration); 
        animator.SetTrigger("Scene2Out");
        yield return new WaitForSeconds(sceneTransitionDuration);
        animator.SetTrigger("Scene3In");
        yield return new WaitForSeconds(sceneDuration); 
        animator.SetTrigger("Scene3Out");
        yield return new WaitForSeconds(sceneTransitionDuration);
        animator.SetTrigger("Scene4In");
        yield return new WaitForSeconds(sceneDuration);
        // Mainkan animasi "Scene4Out"
        animator.SetTrigger("Scene4Out");

        // Tunggu animasi selesai sebelum memuat scene berikutnya
        // Pastikan ada waktu yang cukup agar animasi selesai (misalnya 1 detik)
        yield return new WaitForSeconds(1f);  // Durasi animasi NextScene

        // Memuat scene berikutnya menggunakan SceneManager
        SceneManager.LoadScene(nextSceneName);
    }
}