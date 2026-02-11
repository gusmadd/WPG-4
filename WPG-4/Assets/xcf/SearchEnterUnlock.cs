using UnityEngine;

public class SearchEnterUnlock : MonoBehaviour
{
    public static SearchEnterUnlock Instance;

    [Header("Unlock Settings")]
    public string unlockWord = "meowser";

    [Header("Layers")]
    public GameObject meowserPageLayer;   // webpage layer
    public GameObject keyboardLayer;      // keyboard layer
    public GameObject searchPageLayer;    // page sebelum meowser

    private bool unlocked = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 🔒 AUTO HIDE DI AWAL GAME
        if (meowserPageLayer != null)
            meowserPageLayer.SetActive(false);

        // search page boleh tampil (kalau mau)
        if (searchPageLayer != null)
            searchPageLayer.SetActive(true);

        Debug.Log("🔒 Pages initialized (hidden)");
    }

    public void OnEnterPressed()
    {
        Debug.Log("🌐 ENTER RECEIVED");

        if (unlocked) return;

        if (KeyboardManager.Instance == null)
        {
            Debug.LogError("❌ KeyboardManager.Instance NULL");
            return;
        }

        string input = KeyboardManager.Instance.currentText
            .ToLower()
            .Trim();

        Debug.Log("⌨️ INPUT = [" + input + "]");

        if (input == unlockWord)
        {
            Debug.Log("✅ KEYWORD MATCH");
            Unlock();
        }
        else
        {
            Debug.Log("❌ WRONG KEYWORD");
        }
    }

    void Unlock()
    {
        unlocked = true;

        // hide old layers
        if (searchPageLayer != null)
            searchPageLayer.SetActive(false);

        if (keyboardLayer != null)
            keyboardLayer.SetActive(false);

        // show webpage
        if (meowserPageLayer != null)
            meowserPageLayer.SetActive(true);

        Debug.Log("😼🌐 MEOWSER WEBPAGE OPENED 🦈💙");
    }
}
