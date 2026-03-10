using UnityEngine;
using UnityEngine.UI;

public class KeycapButton : MonoBehaviour
{
    public string keyValue;
    public SearchPageController searchPage;

    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();

        if (btn != null)
            btn.onClick.AddListener(OnKeyPress);
        else
            Debug.LogWarning("KeycapButton: Button component missing!");
    }

    void OnKeyPress()
    {
        if (searchPage != null)
        {
            searchPage.AddChar(keyValue);
        }
    }
}