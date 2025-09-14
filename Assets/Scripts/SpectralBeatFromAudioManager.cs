using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class Beat : MonoBehaviour
{
    public int fftSize = 1024;
    public FFTWindow window = FFTWindow.BlackmanHarris;
    [Range(1.1f, 3f)] public float thresholdMul = 1.6f;
    [Range(16, 86)]  public int history = 43;  // 約 1 秒
    public int lowBin = 0, highBin = 128;      // 只抓低頻(踢鼓)可調

    public UnityEvent OnBeat;

    AudioSource src;
    float[] cur, prev;
    Queue<float> hist = new Queue<float>();
    float prevFlux, lastFlux;

    void Start()
    {
        src  = AudioManager.Instance?.BGMSource;
        cur  = new float[fftSize];
        prev = new float[fftSize];
        if (highBin <= 0 || highBin >= fftSize) highBin = Mathf.Min(128, fftSize - 1);
    }

    void Update()
    {
        if (src == null || !src.isPlaying) return;

        src.GetSpectrumData(cur, 0, window);

        float flux = 0f;
        for (int i = Mathf.Max(0, lowBin); i < Mathf.Min(highBin, fftSize); i++)
        {
            float v = cur[i] - prev[i];
            if (v > 0) flux += v;
            prev[i] = cur[i];
        }

        hist.Enqueue(flux);
        if (hist.Count > history) hist.Dequeue();

        float mean = 0f; foreach (var f in hist) mean += f;
        mean /= Mathf.Max(1, hist.Count);
        float th = mean * thresholdMul;

        if (hist.Count >= 3)
        {
            if (lastFlux > th && lastFlux > prevFlux && lastFlux > flux)
                OnBeat?.Invoke();
            prevFlux = lastFlux;
            lastFlux = flux;
        }
        else lastFlux = flux;
    }
}