using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TMP_Text narratorText;

    void Start()
    {
        narratorText.text = "Feeling abandoned in your owner's house?";
    }
}