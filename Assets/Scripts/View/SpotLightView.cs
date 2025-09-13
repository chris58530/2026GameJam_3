using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class SpotLightView : IView
{
    [SerializeField] private GameObject root;
    [SerializeField] private AnimationCurve roundSpeedCurve;

    [Header("SpotLightObject")]
    [SerializeField] private SpotLight spotLights;


    [Header("SpotLightImage")]
    [SerializeField] private Image spotLightImage;

    [Header("ColorSetting")]
    public ColorSetting[] colorSettings;
    public int round = 0;

    public bool isPlaying = false;
    public Action<GameColor> onColorChange;
    public void StartColorProgress(Action<GameColor> action)
    {
        round = 0;
        onColorChange = action;
        StartCoroutine(PlayingCoroutine());
        Debug.Log("StartColorProgress");
    }

    IEnumerator PlayingCoroutine()
    {
        while (isPlaying)
        {
            float waitSec = roundSpeedCurve.Evaluate(round);
            yield return new WaitForSeconds(waitSec);
            round++;
            ColorSetting currentColor = colorSettings[UnityEngine.Random.Range(0, colorSettings.Length)];
            spotLights.SetColor(currentColor.color);
            spotLightImage.color = currentColor.color;
            onColorChange?.Invoke(currentColor.gameColor);
        }
    }

    public void EndColorProgress()
    {
        isPlaying = false;
    }

    public void Hide()
    {
        StopAllCoroutines();
    }

    public ColorSetting[] GetColorSettings()
    {
        return colorSettings;
    }

    public override void ResetView()
    {
        Hide();
    }
}

[Serializable]
public class ColorSetting
{
    public Color color;
    public GameColor gameColor;
}
