using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class SearchPageController : MonoBehaviour
{
    [Header("References")]
    public TMP_Text searchText;
    public GameObject keyboard;

    [Header("Pages")]
    public GameObject pawShopPage;
    public GameObject errorPage;

    [Header("Tutorial")]
    public TutorialManager tutorialManager;

    [Header("Settings")]
    public int maxCharacters = 20;
    public string defaultText = "Search...";

    private string currentText;
    private bool isTyping = false;
    private bool firstInput = true;
    private Collider2D col;
    private bool pageOpened = false;

    void Start()
    {
        currentText = defaultText;
        UpdateText();

        if (keyboard != null)
            keyboard.SetActive(false);

        if (pawShopPage != null)
            pawShopPage.SetActive(false);

        if (errorPage != null)
            errorPage.SetActive(false);

        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col != null && col.OverlapPoint(mousePos))
            {
                ClickSearchText();
            }
        }
    }

    public void ClickSearchText()
    {
        if (keyboard != null)
            keyboard.SetActive(true);

        isTyping = true;
    }

    public void AddChar(string c)
    {
        if (!isTyping || searchText == null) return;

        if (c == "BACK")
        {
            if (!firstInput && currentText.Length > 0)
                currentText = currentText.Substring(0, currentText.Length - 1);

            if (currentText.Length == 0)
                ResetDefault();

            UpdateText();
            return;
        }

        if (c == "ENTER")
        {
            Submit();
            return;
        }

        if (c == "SPACE")
        {
            if (firstInput)
            {
                currentText = "";
                firstInput = false;
            }

            if (currentText.Length < maxCharacters)
                currentText += " ";

            UpdateText();
            return;
        }

        if (currentText.Length < maxCharacters)
        {
            if (firstInput)
            {
                currentText = "";
                firstInput = false;
            }

            currentText += c;
        }

        UpdateText();
    }

    void UpdateText()
    {
        searchText.text = currentText;
    }

    void Submit()
{
    if (pageOpened) return; // 🔹 mencegah submit berkali-kali

    string query = currentText.ToLower().Trim();

    Debug.Log("Search: " + query);

    if (keyboard != null)
        keyboard.SetActive(false);

    isTyping = false;

    if (query == "pawshop.com")
{
    pageOpened = true;

    // tutup search page
    gameObject.SetActive(false);

    if (pawShopPage != null)
        pawShopPage.SetActive(true);

    if (tutorialManager != null)
        tutorialManager.OnSearchSuccess();
}
    else
    {
        if (errorPage != null)
            errorPage.SetActive(true);
    }
}

    void ResetDefault()
    {
        currentText = defaultText;
        firstInput = true;
    }

    public void CloseKeyboard()
    {
        if (keyboard != null)
            keyboard.SetActive(false);

        isTyping = false;
    }
}