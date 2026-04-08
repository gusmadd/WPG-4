using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_SearchPage : MonoBehaviour
{
    [System.Serializable]
    public class LinkSlot
    {
        public SpriteRenderer spriteRenderer;
        public Collider2D collider;
        public bool isCorrect;
    }

    [Header("Slots (5)")]
    public List<LinkSlot> slots = new List<LinkSlot>();

    [Header("Sprites")]
    public Sprite correctSprite;
    public List<Sprite> wrongSprites = new List<Sprite>();

    [Header("References")]
    public M_MonitorManager monitorManager;

    public void GenerateResults()
    {
        foreach (var s in slots)
            s.isCorrect = false;

        int correctIndex = Random.Range(0, slots.Count);
        slots[correctIndex].isCorrect = true;

        if (slots[correctIndex].spriteRenderer != null)
            slots[correctIndex].spriteRenderer.sprite = correctSprite;

        List<Sprite> tempWrong = new List<Sprite>(wrongSprites);
        Shuffle(tempWrong);

        int wrongPointer = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i == correctIndex) continue;

            if (slots[i].spriteRenderer != null && wrongPointer < tempWrong.Count)
            {
                slots[i].spriteRenderer.sprite = tempWrong[wrongPointer];
                wrongPointer++;
            }
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (M_GameManager.Instance != null &&
            M_GameManager.Instance.currentState != M_GameManager.GameState.Gameplay)
            return;

        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (var s in slots)
        {
            if (s.collider != null && s.collider.OverlapPoint(mousePos))
            {
                OnClickLink(s);
                return;
            }
        }
    }

    void OnClickLink(LinkSlot slot)
    {
        M_AudioManager.Instance?.PlayCursorClick();
        M_PlayerController.Instance?.PlayTyping();

        if (slot.isCorrect)
        {
            if (monitorManager != null)
                monitorManager.OpenPetshopFromResult();
            return;
        }

        // LINK SALAH -> munculin ads
        if (monitorManager != null)
            monitorManager.ShowRandomAdsFromExternal();
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
