using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class SpotLightView : IView
{
    [SerializeField] private GameObject root;
    [SerializeField] private AnimationCurve roundSpeedCurve;
    [SerializeField] private float roundIdleTime;
    [Header("SpotLightObject")]
    [SerializeField] private SpotLight spotLights;


    [Header("SpotLightUIImage")]
    [SerializeField] private Image discoBallImage;
    [SerializeField] private Image spotLightImage;
    private Vector3 originalScale;

    [Header("ColorSetting")]
    public ColorSetting[] colorSettings;
    public int round = 0;

    public bool isPlaying = false;
    public Action<GameColor> onColorChange;
    public void StartColorProgress(Action<GameColor> action)
    {
        round = 0;
        onColorChange = action;
        isPlaying = true;

        if (spotLightImage != null)
            originalScale = spotLightImage.transform.localScale;

        StartCoroutine(PlayingCoroutine());
        Debug.Log("StartColorProgress");
    }

    IEnumerator PlayingCoroutine()
    {
        while (isPlaying)
        {
            float waitSec = roundSpeedCurve.Evaluate(round);

            ColorSetting currentColor = colorSettings[UnityEngine.Random.Range(0, colorSettings.Length)];
            spotLights.SetColor(Color.white);
            spotLightImage.color = currentColor.color;
            onColorChange?.Invoke(currentColor.gameColor);
            discoBallImage.color = Color.white;

            // 重置到原本大小，保持y和z不變
            Vector3 resetScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            spotLightImage.transform.localScale = resetScale;

            // 使用DOTween讓x軸縮放到0.05f，保持y和z軸不變
            float targetX = originalScale.x * 0.05f;
            spotLightImage.transform.DOScaleX(targetX, waitSec);

            yield return new WaitForSeconds(waitSec);
            discoBallImage.color = currentColor.color;
            spotLights.SetColor(currentColor.color);

            round++;
            yield return new WaitForSeconds(roundIdleTime);
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
