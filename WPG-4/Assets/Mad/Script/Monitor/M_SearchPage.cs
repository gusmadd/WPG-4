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
        // reset status
        foreach (var s in slots)
            s.isCorrect = false;

        // acak index slot yang benar
        int correctIndex = Random.Range(0, slots.Count);
        slots[correctIndex].isCorrect = true;
        slots[correctIndex].spriteRenderer.sprite = correctSprite;

        // ambil 4 sprite salah acak dari pool
        List<Sprite> tempWrong = new List<Sprite>(wrongSprites);
        Shuffle(tempWrong);

        int wrongPointer = 0;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i == correctIndex) continue;

            slots[i].spriteRenderer.sprite = tempWrong[wrongPointer];
            wrongPointer++;
        }
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
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
        if (slot.isCorrect)
        {
            M_AudioManager.Instance?.PlayCursorClick();
            monitorManager.OpenPetshopFromResult();
        }
        else
        {
            // link salah belum bisa diklik
            M_AudioManager.Instance?.PlayCursorClick();
        }
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
