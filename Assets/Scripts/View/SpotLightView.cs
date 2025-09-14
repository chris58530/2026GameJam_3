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
    [SerializeField] private Vector2 spotLightMoveRangeX;
    [SerializeField] private Vector2 spotLightMoveRangeY;
    [SerializeField] private float spotLightMoveDuration = 1.0f;


    [Header("SpotLightUIImage")]
    [SerializeField] private Image discoBallImage;
    [SerializeField] private Image spotLightImage;
    private Vector3 originalScale;

    [Header("ColorSetting")]
    public ColorSetting[] colorSettings;
    public int round = 0;

    public bool isPlaying = false;
    public Action<GameColor> onColorChange;
    public Action showGlass;
    public void StartColorProgress(Action<GameColor> onColorChange, Action showGlass)
    {
        round = 0;
        this.onColorChange = onColorChange;
        this.showGlass = showGlass;
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
            discoBallImage.color = Color.white;
            showGlass?.Invoke();
            // 重置到原本大小，保持y和z不變
            Vector3 resetScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            spotLightImage.transform.localScale = resetScale;

            // 使用DOTween讓x軸縮放到0.05f，保持y和z軸不變
            float targetX = originalScale.x * 0.05f;
            spotLightImage.transform.DOScaleX(targetX, waitSec);

            // 開始持續移動SpotLight
            StartCoroutine(ContinuousMoveSpotLight(waitSec));

            yield return new WaitForSeconds(waitSec);
            onColorChange?.Invoke(currentColor.gameColor);
            discoBallImage.color = currentColor.color;
            spotLights.SetColor(currentColor.color);

            round++;
            yield return new WaitForSeconds(roundIdleTime);
        }
    }

    IEnumerator ContinuousMoveSpotLight(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            MoveSpotLight();

            // 等待這次移動完成，或者剩餘時間不足
            float waitTime = Mathf.Min(spotLightMoveDuration, duration - elapsedTime);
            yield return new WaitForSeconds(waitTime);

            elapsedTime += waitTime;
        }
    }

    public void MoveSpotLight()
    {
        if (spotLights == null) return;

        float targetX = UnityEngine.Random.Range(spotLightMoveRangeX.x, spotLightMoveRangeX.y);
        float targetY = UnityEngine.Random.Range(spotLightMoveRangeY.x, spotLightMoveRangeY.y);
        Vector3 targetPosition = new Vector3(targetX, targetY, spotLights.transform.position.z);

        spotLights.transform.DOMove(targetPosition, spotLightMoveDuration).SetEase(Ease.InOutSine);
    }


    public ColorSetting[] GetColorSettings()
    {
        return colorSettings;
    }

    public override void ResetView()
    {
        StopAllCoroutines();
        isPlaying = false;
    }
}

[Serializable]
public class ColorSetting
{
    public Color color;
    public GameColor gameColor;
}
