using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GHAnimScript : MonoBehaviour
{
    [Header("Target")]
    public SpriteRenderer targetRenderer;

    [Header("Frames")]
    public Sprite[] frames;

    [Header("Playback")]
    public float frameRate = 12f;
    public bool playOnEnable = true;
    public bool loop = true;
    public bool resetToFirstFrameOnStop = true;

    int currentFrame = 0;
    float timer = 0f;
    bool isPlaying = false;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        ApplyFrame(0);
    }

    void OnEnable()
    {
        if (playOnEnable)
            Play();
        else
            ApplyFrame(0);
    }

    void Update()
    {
        if (!isPlaying) return;
        if (frames == null || frames.Length == 0) return;
        if (targetRenderer == null) return;
        if (frameRate <= 0f) return;

        timer += Time.unscaledDeltaTime;

        float frameDuration = 1f / frameRate;

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Length - 1;
                    isPlaying = false;
                }
            }

            ApplyFrame(currentFrame);
        }
    }

    public void Play()
    {
        if (frames == null || frames.Length == 0) return;
        isPlaying = true;
    }

    public void PlayFromStart()
    {
        if (frames == null || frames.Length == 0) return;

        currentFrame = 0;
        timer = 0f;
        ApplyFrame(currentFrame);
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
        timer = 0f;

        if (resetToFirstFrameOnStop)
        {
            currentFrame = 0;
            ApplyFrame(currentFrame);
        }
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void SetFrame(int index)
    {
        if (frames == null || frames.Length == 0) return;
        if (index < 0 || index >= frames.Length) return;

        currentFrame = index;
        ApplyFrame(currentFrame);
    }

    void ApplyFrame(int index)
    {
        if (targetRenderer == null) return;
        if (frames == null || frames.Length == 0) return;
        if (index < 0 || index >= frames.Length) return;

        targetRenderer.sprite = frames[index];
    }

}
